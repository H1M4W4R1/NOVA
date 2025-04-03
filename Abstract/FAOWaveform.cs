using NOVA.Abstract.Interfaces;
using NOVA.Utility;

namespace NOVA.Abstract
{
    /// <summary>
    /// Waveform with modulated frequency, amplitude and offset
    /// Used mostly for mathematical functions
    /// </summary>
    /// <param name="frequency">Frequency of the waveform in Hz, minimum 0.001Hz</param>
    /// <param name="amplitude">Amplitude of the waveform in [0, 1] range</param>
    /// <param name="offset">Offset of the waveform in [0, 1] range</param>
    /// <remarks>
    /// If the sum of amplitude and offset is greater than 1, the offset is adjusted to ensure the sum is 1
    /// </remarks>
    public abstract class FAOWaveform(double frequency, double amplitude, double offset) : Waveform,
        IPeriodicWaveform
    {
        /// <summary>
        /// Frequency of the waveform in Hz
        /// </summary>
        public double Frequency { get; private set; } = WaveformMath.ClampFrequency(frequency);
        
        /// <summary>
        /// Period of the waveform in milliseconds
        /// </summary>
        public double Period => 1000 / Frequency;

        /// <summary>
        /// Amplitude of the waveform in [0, 1] range
        /// </summary>
        protected double Amplitude { get; private set; } = WaveformMath.ClampAmplitude(amplitude);

        /// <summary>
        /// Offset of the waveform in [0, 1] range
        /// </summary>
        protected double Offset { get; private set; } = WaveformMath.ClampOffset(offset, amplitude);

        /// <summary>
        /// Sets the frequency of the waveform
        /// </summary>
        /// <param name="frequency">Frequency in Hz</param>
        public void SetFrequency(double frequency) => Frequency = WaveformMath.ClampFrequency(frequency);

        /// <summary>
        /// Sets the amplitude of the waveform
        /// </summary>
        /// <param name="amplitude">Amplitude in [0, 1] range</param>
        public void SetAmplitude(double amplitude)
        {
            // Ensure amplitude and offset is in [0, 1] range
            // and their sum is less or equal to 1
            Amplitude = WaveformMath.ClampAmplitude(amplitude);
            Offset = WaveformMath.ClampOffset(Offset, Amplitude);
        }
        
        /// <summary>
        /// Sets the offset of the waveform
        /// </summary>
        /// <param name="offset">Offset in [0, 1] range</param>
        public void SetOffset(double offset) => Offset = WaveformMath.ClampOffset(offset, Amplitude);

    }
}