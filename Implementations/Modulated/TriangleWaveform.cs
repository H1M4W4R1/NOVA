using NOVA.Abstract;

namespace NOVA.Implementations.Modulated
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
    public sealed class TriangleWaveform(double frequency, double amplitude = 1, double offset = 0)
        : FAOWaveform(frequency, amplitude, offset)
    {
        public override double CalculateValueAt(double time)
        {
            // Calculate period
            double period = 1000 / Frequency;

            // Calculate if the waveform is in the first or second half of the period
            double timeInPeriod = time % period;
            double halfPeriod = period / 2;

            // If the waveform is in the second half of the period, invert the value
            return timeInPeriod <= halfPeriod
                ? Amplitude * timeInPeriod / halfPeriod + Offset
                : Amplitude * (period - timeInPeriod) / halfPeriod + Offset;
        }
    }
}