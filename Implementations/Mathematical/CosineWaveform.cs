using NOVA.Abstract;

namespace NOVA.Implementations.Mathematical
{
    /// <summary>
    /// Cosine waveform
    /// </summary>
    /// <param name="frequency">Frequency of the waveform in Hz</param>
    /// <param name="amplitude">Amplitude of the waveform in [0, 1] range</param>
    /// <param name="offset">Offset of the waveform in [0, 1] range</param>
    public sealed class CosineWaveform(double frequency, double amplitude = 1, double offset = 0)
        : FAOWaveform(frequency, amplitude, offset)
    {
        public override double CalculateValueAt(double time)
            => (Math.Cos(2 * Math.PI * Frequency * time / 1000) * Amplitude + Amplitude) / 2 + Offset;
    }
}