using NOVA.Abstract;
using NOVA.Utility;

namespace NOVA.Implementations.Modulated.Randomized
{
    /// <summary>
    /// Randomizes waveform - randomizes amplitude while keeping the offset constant
    /// </summary>
    /// <param name="updateFrequency">Update frequency of the waveform in Hz</param>
    /// <param name="amplitude">Maximum amplitude of the waveform in [0, 1] range</param>
    /// <remarks>
    /// Offset cannot be higher than 1 - maxAmplitude, will be adjusted if necessary.
    /// </remarks>
    public sealed class RandomizedWaveform(double updateFrequency,
        double amplitude = WaveformMath.WAVEFORM_MAXIMUM_VALUE,
        double offset = WaveformMath.WAVEFORM_MINIMUM_VALUE
    )
        : Waveform
    {
        /// <summary>
        /// Current offset of the waveform in [0, 1] range
        /// </summary>
        private double Offset { get; set; } = WaveformMath.ClampOffset(offset, amplitude);

        /// <summary>
        /// Current amplitude of the waveform in [0, 1] range
        /// </summary>
        private double CurrentAmplitude { get; set; }

        /// <summary>
        /// Update timer in milliseconds
        /// </summary>
        private double UpdateTimer { get; set; }

        /// <summary>
        /// Update frequency of the waveform in Hz
        /// </summary>
        public double UpdateFrequency { get; private set; } = WaveformMath.ClampFrequency(updateFrequency);

        /// <summary>
        /// Minimum amplitude of the waveform in [0, 1] range
        /// </summary>
        public double Amplitude { get; private set; } = WaveformMath.ClampAmplitude(amplitude);

        /// <summary>
        /// Sets the maximum amplitude of the waveform
        /// </summary>
        /// <param name="maxAmplitude">Maximum amplitude in [0, 1] range</param>
        public void SetAmplitude(double maxAmplitude)
        {
            Amplitude = WaveformMath.ClampAmplitude(maxAmplitude);
            Offset = WaveformMath.ClampOffset(Offset, Amplitude);
        }

        /// <summary>
        /// Sets the update frequency of the waveform
        /// </summary>
        /// <param name="updateFrequency">Update frequency in Hz</param>
        public void SetUpdateFrequency(double updateFrequency)
        {
            UpdateFrequency = WaveformMath.ClampFrequency(updateFrequency);
        }

        public override double CalculateValueAt(double time)
        {
            // Check if enough time has passed since the last update
            UpdateTimer += DeltaTime;
            if (UpdateTimer >= WaveformMath.FrequencyToPeriod(UpdateFrequency))
            {
                // Reset timer
                UpdateTimer = 0;

                // Perform chaotic update - randomize offset and amplitude
                CurrentAmplitude = Random.Shared.NextDouble() * Amplitude;
            }

            // Return the current amplitude with the offset, ensuring the sum is within [0, 1] range
            return Math.Clamp(CurrentAmplitude + Offset, WaveformMath.WAVEFORM_MINIMUM_VALUE,
                WaveformMath.WAVEFORM_MAXIMUM_VALUE);
        }
    }
}