using System.Runtime.CompilerServices;
using NOVA.Abstract;
using NOVA.Abstract.Interfaces;
using NOVA.Utility;

namespace NOVA.Implementations.Modulated.Periodic
{
    /// <summary>
    ///     Represents a trapezoidal waveform with customizable ramp and hold times.
    ///     The waveform consists of four distinct phases:
    ///     1. Ramp-up from minimum to maximum value
    ///     2. Hold at maximum value
    ///     3. Ramp-down from maximum to minimum value
    ///     4. Hold at minimum value
    /// </summary>
    /// <param name="rampUpTime">
    ///     Time in milliseconds for the waveform to rise from minimum to maximum value. Must be
    ///     non-negative.
    /// </param>
    /// <param name="keepMaxTime">Time in milliseconds to maintain the maximum value. Must be non-negative.</param>
    /// <param name="rampDownTime">
    ///     Time in milliseconds for the waveform to fall from maximum to minimum value. Must be
    ///     non-negative.
    /// </param>
    /// <param name="keepMinTime">Time in milliseconds to maintain the minimum value. Must be non-negative.</param>
    /// <param name="amplitude">
    ///     Peak-to-peak amplitude of the waveform, normalized to [0, 1] range. Defaults to maximum value
    ///     (1).
    /// </param>
    /// <param name="offset">DC offset of the waveform, normalized to [0, 1] range. Defaults to minimum value (0).</param>
    /// <remarks>
    ///     <para>
    ///         The waveform's total period is the sum of all phase times: rampUpTime + keepMaxTime + rampDownTime +
    ///         keepMinTime.
    ///     </para>
    ///     <para>The sum of amplitude and offset must be ≤ 1 to ensure values stay within the valid [0, 1] range.</para>
    ///     <para>All time parameters must be non-negative. Zero values are allowed for any phase.</para>
    /// </remarks>
    public sealed class TrapezoidalWaveform(double rampUpTime,
        double keepMaxTime,
        double rampDownTime,
        double keepMinTime,
        double amplitude = WaveformMath.WAVEFORM_MAXIMUM_VALUE,
        double offset = WaveformMath.WAVEFORM_MINIMUM_VALUE
    ) : Waveform, IPeriodicWaveform
    {
        /// <summary>
        ///     Gets the total period of the waveform in milliseconds.
        ///     Calculated as the sum of all phase durations.
        /// </summary>
        public double Period => rampUpTime + keepMaxTime + rampDownTime + keepMinTime;

        /// <summary>
        ///     Gets the frequency of the waveform in Hertz (Hz).
        ///     Calculated as the reciprocal of the period.
        /// </summary>
        public double Frequency => WaveformMath.PeriodToFrequency(Period);

        /// <summary>
        ///     Gets or sets the amplitude of the waveform, normalized to [0, 1] range.
        ///     The setter automatically clamps the value to valid range and adjusts offset if needed.
        /// </summary>
        public double Amplitude { get; private set; } = WaveformMath.ClampAmplitude(amplitude);

        /// <summary>
        ///     Gets or sets the DC offset of the waveform, normalized to [0, 1] range.
        ///     The setter automatically clamps the value to ensure amplitude + offset ≤ 1.
        /// </summary>
        public double Offset { get; private set; } = WaveformMath.ClampOffset(offset, amplitude);

        /// <summary>
        ///     Sets a new amplitude value for the waveform.
        ///     The value is clamped to [0, 1] range and may adjust the offset to maintain validity.
        /// </summary>
        /// <param name="amplitude">New amplitude value in [0, 1] range.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void SetAmplitude(double amplitude)
        {
            Amplitude = WaveformMath.ClampAmplitude(amplitude);
            Offset = WaveformMath.ClampOffset(Offset, Amplitude);
        }

        /// <summary>
        ///     Sets a new offset value for the waveform.
        ///     The value is clamped to ensure amplitude + offset ≤ 1.
        /// </summary>
        /// <param name="offset">New offset value in [0, 1] range.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void SetOffset(double offset)
            => Offset = WaveformMath.ClampOffset(offset, Amplitude);

        /// <summary>
        ///     Calculates the waveform's value at a specific point in time.
        /// </summary>
        /// <param name="time">Time in milliseconds at which to evaluate the waveform.</param>
        /// <returns>The waveform's normalized value [0, 1] at the specified time.</returns>
        /// <remarks>
        ///     The calculation follows these steps:
        ///     1. Normalizes the input time to the current waveform period
        ///     2. Determines which phase (ramp up, hold max, ramp down, hold min) the time falls into
        ///     3. Computes the appropriate interpolated value for that phase
        /// </remarks>
        public override double[] CalculateValuesAt(double time)
        {
            // Calculate time in period
            double timeInPeriod = WaveformMath.TimeInCycle(time, Period);

            // Calculate value based on time in period
            if (timeInPeriod < rampUpTime) // Ramp-up, triangle
                CurrentValues[0] = Offset + Amplitude * (timeInPeriod / rampUpTime);
            else if (timeInPeriod < rampUpTime + keepMaxTime) // Keep max, constant value
                CurrentValues[0] = Offset + Amplitude;
            else if (timeInPeriod < rampUpTime + keepMaxTime + rampDownTime) // Ramp-down, inverted triangle
                CurrentValues[0] = Offset + Amplitude * (WaveformMath.WAVEFORM_MAXIMUM_VALUE -
                                                         (timeInPeriod - rampUpTime - keepMaxTime) / rampDownTime);
            else
                CurrentValues[0] = Offset; // Keep min, constant value

            return CurrentValues;
        }
    }
}