using NOVA.Abstract;

namespace NOVA.Implementations.Modulated
{
    /// <summary>
    /// Waveform with modulated amplitude - user can specify the amplitude of the waveform
    /// </summary>
    /// <param name="startAmplitude">Initial amplitude of the waveform in [0, 1] range</param>
    public sealed class ScalableWaveform(double startAmplitude) : Waveform
    {
        /// <summary>
        /// Amplitude of the waveform
        /// </summary>
        private double Amplitude { get; set; } = Math.Clamp(startAmplitude, 0, 1);
        
        /// <summary>
        /// Sets the amplitude of the waveform
        /// </summary>
        /// <param name="amplitude">Amplitude of the waveform in [0, 1] range</param>
        public void SetAmplitude(double amplitude) => Amplitude = Math.Clamp(amplitude, 0, 1);

        public override double CalculateValueAt(double time) => Amplitude;
    }
}