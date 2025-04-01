using NOVA.Abstract;

namespace NOVA.Implementations
{
    /// <summary>
    /// Constant waveform set to a specific value, always same value.
    /// </summary>
    /// <param name="amplitude">Amplitude of the waveform in [0, 1] range</param>
    public sealed class ConstantWaveform(double amplitude) : WaveformBase
    {
        public override double CalculateValueAt(double time) => amplitude;
    }
}