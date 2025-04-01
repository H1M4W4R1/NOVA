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
    public abstract class FAOWaveform(double frequency, double amplitude, double offset) : WaveformBase
    {
        private const double MIN_FREQUENCY = 0.001;

        /// <summary>
        /// Frequency of the waveform in Hz
        /// </summary>
        protected double Frequency { get; private set; } = Math.Max(frequency, MIN_FREQUENCY);

        /// <summary>
        /// Amplitude of the waveform in [0, 1] range
        /// </summary>
        protected double Amplitude { get; private set; } = Math.Clamp(amplitude, 0, 1);

        /// <summary>
        /// Offset of the waveform in [0, 1] range
        /// </summary>
        protected double Offset { get; private set; } = Math.Min(Math.Clamp(offset, 0, 1), 1 - Math.Clamp(amplitude, 0, 1));

        /// <summary>
        /// Sets the frequency of the waveform
        /// </summary>
        /// <param name="frequency">Frequency in Hz</param>
        public void SetFrequency(double frequency) => Frequency = Math.Max(frequency, MIN_FREQUENCY);

        /// <summary>
        /// Sets the amplitude of the waveform
        /// </summary>
        /// <param name="amplitude">Amplitude in [0, 1] range</param>
        /// <remarks>
        /// If the sum of amplitude and offset is greater than 1, the offset is adjusted to ensure the sum is 1
        /// </remarks>
        public void SetAmplitude(double amplitude)
        {
            // Ensure amplitude is in [0, 1] range
            amplitude = Math.Clamp(amplitude, 0, 1);

            // Ensure amplitude + offset is in [0, 1] range
            if (amplitude + Offset > 1) Offset = 1 - amplitude;

            // Set amplitude
            Amplitude = amplitude;
        }

        /// <summary>
        /// Sets the offset of the waveform
        /// </summary>
        /// <param name="offset">Offset in [0, 1] range</param>
        /// <remarks>
        /// If the sum of amplitude and offset is greater than 1, the amplitude is adjusted to ensure the sum is 1
        /// </remarks>
        public void SetOffset(double offset)
        {
            // Ensure offset is in [0, 1] range
            offset = Math.Clamp(offset, 0, 1);

            // Ensure amplitude + offset is in [0, 1] range
            if (Amplitude + offset > 1) Amplitude = 1 - offset;

            // Set offset
            Offset = offset;
        }
    }
}