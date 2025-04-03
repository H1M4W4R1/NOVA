using System.Runtime.CompilerServices;
using NOVA.Abstract;
using NOVA.Utility;

namespace NOVA.Implementations.Modulated.Periodic
{
    /// <summary>
    /// Sawtooth waveform
    /// </summary>
    /// <param name="frequency">Frequency of the waveform in Hz</param>
    /// <param name="amplitude">Amplitude of the waveform in [0, 1] range</param>
    /// <param name="offset">Offset of the waveform in [0, 1] range</param>
    /// <param name="inverted">Determines if the waveform is inverted - starts high and ramps down</param>
    /// <remarks>
    /// If offset and amplitude sum is greater than one offset is adjusted to ensure the sum is 1
    /// </remarks>
    public sealed class SawtoothWaveform(double frequency,
        double amplitude = WaveformMath.WAVEFORM_MAXIMUM_VALUE,
        double offset = WaveformMath.WAVEFORM_MINIMUM_VALUE,
        bool inverted = false
    ) : FAOWaveform(frequency, amplitude, offset)
    {
        /// <summary>
        /// Determines if the waveform is inverted
        /// </summary>
        private bool Inverted { get; set; } = inverted;

        /// <summary>
        /// Inverts the waveform sawtooth direction
        /// </summary>
        /// <param name="inverted">True if the waveform should be inverted, false otherwise</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetInverted(bool inverted) => Inverted = inverted;

        public override double CalculateValueAt(double time)
        {
            // Calculate period
            double period = WaveformMath.FrequencyToPeriod(Frequency);

            // Calculate time in period
            double timeInPeriod = WaveformMath.TimeInCycle(time, period);

            // Calculate ramp-up value
            double rampUpValue = Amplitude * timeInPeriod / period;

            // If the waveform is inverted, invert the ramp-up value and return otherwise return the ramp-up value
            return (Inverted ? Amplitude - rampUpValue : rampUpValue) + Offset;
        }
    }
}