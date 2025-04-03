using NOVA.Abstract;
using NOVA.Utility;

namespace NOVA.Implementations.Modulated.Periodic
{
    /// <summary>
    /// Triangle waveform
    /// </summary>
    /// <param name="frequency">Frequency of the waveform in Hz</param>
    /// <param name="amplitude">Amplitude of the waveform in [0, 1] range</param>
    /// <param name="offset">Offset of the waveform in [0, 1] range</param>
    /// <remarks>
    /// If offset and amplitude sum is greater than one offset is adjusted to ensure the sum is 1
    /// </remarks>
    public sealed class TriangleWaveform(double frequency, double amplitude = WaveformMath.WAVEFORM_MAXIMUM_VALUE,
        double offset = WaveformMath.WAVEFORM_MINIMUM_VALUE)
        : FAOWaveform(frequency, amplitude, offset)
    {
        public override double CalculateValueAt(double time)
        {
            // Calculate period
            double period = WaveformMath.FrequencyToPeriod(Frequency);

            // Calculate if the waveform is in the first or second half of the period
            double timeInPeriod = WaveformMath.TimeInCycle(time, period);
            double halfPeriod = period / 2;

            // If the waveform is in the second half of the period, invert the value
            return timeInPeriod <= halfPeriod
                ? Amplitude * timeInPeriod / halfPeriod + Offset
                : Amplitude * (period - timeInPeriod) / halfPeriod + Offset;
        }
    }
}