using System.Runtime.CompilerServices;
using NOVA.Abstract;
using NOVA.Utility;

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
        private double Amplitude { get; set; } = WaveformMath.ClampAmplitude(startAmplitude);
        
        /// <summary>
        /// Sets the amplitude of the waveform
        /// </summary>
        /// <param name="amplitude">Amplitude of the waveform in [0, 1] range</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetAmplitude(double amplitude) => Amplitude = WaveformMath.ClampAmplitude(amplitude);

        public override double CalculateValueAt(double time) => Amplitude;
    }
}