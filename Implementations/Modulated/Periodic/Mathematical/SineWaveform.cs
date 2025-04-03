using NOVA.Abstract;
using NOVA.Utility;

namespace NOVA.Implementations.Modulated.Periodic.Mathematical
{
    /// <summary>
    /// Sinusoidal waveform
    /// </summary>
    /// <param name="frequency">Frequency of the waveform in Hz</param>
    /// <param name="amplitude">Amplitude of the waveform in [0, 1] range</param>
    /// <param name="offset">Offset of the waveform in [0, 1] range</param>
    public sealed class SineWaveform(double frequency,
        double amplitude = WaveformMath.WAVEFORM_MAXIMUM_VALUE,
        double offset = WaveformMath.WAVEFORM_MINIMUM_VALUE
    )
        : FAOWaveform(frequency, amplitude, offset)
    {
        public override double CalculateValueAt(double time)
            => (Math.Sin(2 * Math.PI * Frequency * time / WaveformMath.ONE_SECOND) * Amplitude + Amplitude) / 2 + Offset;
    }
}