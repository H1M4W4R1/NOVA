using System.Runtime.CompilerServices;
using NOVA.Abstract;
using NOVA.Abstract.Interfaces;
using NOVA.Utility;

namespace NOVA.Implementations.Modulated.Periodic
{
    /// <summary>
    /// Rectangular waveform with a trapezoidal shape <br/>
    /// <ul>
    /// <li>Minimum value is offset</li>
    /// <li>Maximum value is offset + amplitude</li>
    /// </ul>
    /// </summary>
    /// <param name="rampUpTime">Time in ms to ramp up from minimum to maximum value</param>
    /// <param name="keepMaxTime">Time in ms to keep maximum value</param>
    /// <param name="rampDownTime">Time in ms to ramp down from maximum to minimum value</param>
    /// <param name="keepMinTime">Time in ms to keep minimum value</param>
    /// <param name="amplitude">Amplitude of the waveform in [0, 1] range</param>
    /// <param name="offset">Offset of the waveform in [0, 1] range</param>
    /// <remarks>
    /// The sum of amplitude and offset must be less than or equal to 1. 
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
        /// Period of the waveform in ms
        /// </summary>
        public double Period => rampUpTime + keepMaxTime + rampDownTime + keepMinTime;

        /// <summary>
        /// Frequency of the waveform in Hz
        /// </summary>
        public double Frequency => WaveformMath.PeriodToFrequency(Period);

        /// <summary>
        /// Amplitude of the waveform in [0, 1] range
        /// </summary>
        public double Amplitude { get; private set; } = WaveformMath.ClampAmplitude(amplitude);

        /// <summary>
        /// Offset of the waveform in [0, 1] range
        /// </summary>
        public double Offset { get; private set; } = WaveformMath.ClampOffset(offset, amplitude);

        /// <summary>
        /// Sets the frequency of the waveform
        /// </summary>
        /// <param name="amplitude">Amplitude in [0, 1] range</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetAmplitude(double amplitude)
        {
            Amplitude = WaveformMath.ClampAmplitude(amplitude);
            Offset = WaveformMath.ClampOffset(Offset, Amplitude);
        }

        /// <summary>
        /// Sets the offset of the waveform
        /// </summary>
        /// <param name="offset">Offset of the waveform in [0, 1] range</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetOffset(double offset) => Offset = WaveformMath.ClampOffset(offset, Amplitude);

        public override double CalculateValueAt(double time)
        {
            // Calculate time in period
            double timeInPeriod = WaveformMath.TimeInCycle(time, Period);

            // Calculate value based on time in period
            if (timeInPeriod < rampUpTime) // Ramp-up, triangle
                return Offset + Amplitude * (timeInPeriod / rampUpTime);
            if (timeInPeriod < rampUpTime + keepMaxTime) // Keep max, constant value
                return Offset + Amplitude;
            if (timeInPeriod < rampUpTime + keepMaxTime + rampDownTime) // Ramp-down, inverted triangle
                return Offset + Amplitude * (WaveformMath.WAVEFORM_MAXIMUM_VALUE - (timeInPeriod - rampUpTime - keepMaxTime) / rampDownTime);
            return Offset; // Keep min, constant value
        }
    }
}