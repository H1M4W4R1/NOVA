using NOVA.Abstract;

namespace NOVA.Implementations.Modulated.Periodic.Mathematical
{
    /// <summary>
    /// Sinusoidal waveform
    /// </summary>
    /// <param name="frequency">Frequency of the waveform in Hz</param>
    /// <param name="amplitude">Amplitude of the waveform in [0, 1] range</param>
    /// <param name="offset">Offset of the waveform in [0, 1] range</param>
    public sealed class SineWaveform(double frequency, double amplitude = 1, double offset = 0)
        : FAOWaveform(frequency, amplitude, offset)
    {
        public override double CalculateValueAt(double time)
            => (Math.Sin(2 * Math.PI * Frequency * time / 1000) * Amplitude + Amplitude) / 2 + Offset;
    }
}