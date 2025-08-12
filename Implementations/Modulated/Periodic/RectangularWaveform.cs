using System.Runtime.CompilerServices;
using NOVA.Abstract;
using NOVA.Utility;

namespace NOVA.Implementations.Modulated.Periodic
{
    /// <summary>
    ///     Represents a rectangular (square) waveform generator with configurable duty cycle.
    ///     This waveform produces periodic rectangular pulses with adjustable fill factor.
    ///     Inherits from FAOWaveform to support frequency, amplitude and offset parameters.
    /// </summary>
    /// <param name="frequency">
    ///     The frequency of the waveform in Hertz (Hz). Determines how many complete cycles occur per
    ///     second.
    /// </param>
    /// <param name="amplitude">The peak amplitude of the waveform, normalized to range [0, 1]. Defaults to maximum value (1).</param>
    /// <param name="offset">The vertical offset of the waveform, normalized to range [0, 1]. Defaults to minimum value (0).</param>
    /// <param name="fillFactor">
    ///     The duty cycle ratio (0-1) determining the proportion of time the waveform is at maximum
    ///     amplitude. Default is 0.5 (50% duty cycle).
    /// </param>
    /// <remarks>
    ///     The waveform alternates between maximum amplitude (amplitude + offset) and minimum value (offset)
    ///     based on the specified fill factor. A fill factor of 0.5 produces a perfect square wave.
    /// </remarks>
    public sealed class RectangularWaveform(double frequency,
        double amplitude = WaveformMath.WAVEFORM_MAXIMUM_VALUE,
        double offset = WaveformMath.WAVEFORM_MINIMUM_VALUE,
        double fillFactor = 0.5
    )
        : FAOWaveform(frequency, amplitude, offset)
    {
        /// <summary>
        ///     Gets or sets the fill factor (duty cycle) of the waveform.
        ///     Value is clamped to [0, 1] range where:
        ///     - 0 means always at minimum value (offset)
        ///     - 1 means always at maximum value (amplitude + offset)
        ///     - 0.5 means equal time at minimum and maximum (standard square wave)
        /// </summary>
        private double FillFactor { get; set; } = Math.Clamp(fillFactor, 0, 1);

        /// <summary>
        ///     Updates the fill factor (duty cycle) of the waveform.
        /// </summary>
        /// <param name="fillFactor">The new fill factor value in range [0, 1]. Values outside this range will be clamped.</param>
        /// <remarks>
        ///     This method is aggressively inlined for performance optimization.
        ///     Changing the fill factor immediately affects subsequent waveform calculations.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFillFactor(double fillFactor) => FillFactor = Math.Clamp(fillFactor, 0, 1);

        /// <summary>
        ///     Calculates the waveform's value at a specific point in time.
        /// </summary>
        /// <param name="time">The time point in milliseconds at which to evaluate the waveform.</param>
        /// <returns>
        ///     Returns amplitude + offset when within the active portion of the cycle (determined by fill factor),
        ///     otherwise returns just the offset value.
        /// </returns>
        /// <remarks>
        ///     The calculation follows these steps:
        ///     1. Converts frequency to period in milliseconds
        ///     2. Calculates the current position within the waveform's period
        ///     3. Returns high value if within fill factor portion, low value otherwise
        /// </remarks>
        public override double[] CalculateValuesAt(double time)
        {
            // Calculate period
            double period = WaveformMath.FrequencyToPeriod(Frequency);

            // Calculate time in period
            double timeInPeriod = WaveformMath.TimeInCycle(time, period);

            // If the waveform is in the second half of the period, invert the value
            if (timeInPeriod <= period * FillFactor)
            {
                CurrentValues[0] = Amplitude + Offset;
                return CurrentValues;
            }

            CurrentValues[0] = Offset;
            return CurrentValues;
        }
    }
}