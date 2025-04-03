using NOVA.Abstract;

namespace NOVA.Implementations.Modulated.Randomized
{
    /// <summary>
    /// Randomizes waveform - randomizes amplitude while keeping the offset constant
    /// </summary>
    /// <param name="updateFrequency">Update frequency of the waveform in Hz</param>
    /// <param name="minAmplitude">Minimum amplitude of the waveform in [0, 1] range</param>
    /// <param name="maxAmplitude">Maximum amplitude of the waveform in [0, 1] range</param>
    /// <remarks>
    /// <ul>
    /// <li>Offset cannot be higher than 1 - maxAmplitude, will be adjusted if necessary.</li>
    /// <li>If the sum of generated amplitude and offset is greater than 1, the amplitude is adjusted to ensure the sum is 1.</li>
    /// <li>Setting minimum amplitude above maximum amplitude will result in clamping the minimum amplitude to the maximum amplitude.</li>
    /// <li>Setting maximum amplitude below minimum amplitude will result in clamping the maximum amplitude to the minimum amplitude.</li>
    /// <li>If new amplitude and offset sum is greater than 1, the offset is adjusted to ensure the sum is 1.</li>
    /// </ul>
    /// </remarks>
    public sealed class RandomizedWaveform(double updateFrequency,
        double minAmplitude = 0,
        double maxAmplitude = 1,
        double offset = 0
    )
        : Waveform
    {
        private Random RandomNumberGenerator { get; } = new();

        /// <summary>
        /// Current offset of the waveform in [0, 1] range
        /// </summary>
        private double Offset { get; set; } = Math.Min(Math.Clamp(offset, 0, 1),
            1 - Math.Clamp(Math.Max(minAmplitude, maxAmplitude), 0, 1));

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
        public double UpdateFrequency { get; private set; } = Math.Max(0.001, updateFrequency);

        /// <summary>
        /// Minimum amplitude of the waveform in [0, 1] range
        /// </summary>
        public double MinAmplitude { get; private set; } = Math.Clamp(minAmplitude, 0, maxAmplitude);

        /// <summary>
        /// Maximum amplitude of the waveform in [0, 1] range
        /// </summary>
        public double MaxAmplitude { get; private set; } = Math.Clamp(maxAmplitude, minAmplitude, 1);

        /// <summary>
        /// Sets the amplitude range of the waveform
        /// </summary>
        /// <param name="minAmplitude">Minimum amplitude in [0, 1] range</param>
        /// <param name="maxAmplitude">Maximum amplitude in [0, 1] range</param>
        public void SetAmplitudeRange(double minAmplitude, double maxAmplitude)
        {
            MinAmplitude = Math.Clamp(minAmplitude, 0, maxAmplitude);
            MaxAmplitude = Math.Clamp(maxAmplitude, minAmplitude, 1);

            // Ensure offset and amplitude sum is less than 1
            if (Offset + MaxAmplitude > 1) Offset = 1 - MaxAmplitude;
            if (Offset + MinAmplitude > 1) Offset = 1 - MinAmplitude;
        }

        /// <summary>
        /// Sets the minimum amplitude of the waveform
        /// </summary>
        /// <param name="minAmplitude">Minimum amplitude in [0, 1] range</param>
        public void SetMinimumAmplitude(double minAmplitude)
        {
            MinAmplitude = Math.Clamp(minAmplitude, 0, MaxAmplitude);

            // Ensure offset and amplitude sum is less than 1
            if (Offset + MinAmplitude > 1) Offset = 1 - MinAmplitude;
        }

        /// <summary>
        /// Sets the maximum amplitude of the waveform
        /// </summary>
        /// <param name="maxAmplitude">Maximum amplitude in [0, 1] range</param>
        public void SetMaximumAmplitude(double maxAmplitude)
        {
            MaxAmplitude = Math.Clamp(maxAmplitude, MinAmplitude, 1);

            // Ensure offset and amplitude sum is less than 1
            if (Offset + MaxAmplitude > 1) Offset = 1 - MaxAmplitude;
        }

        /// <summary>
        /// Sets the update frequency of the waveform
        /// </summary>
        /// <param name="updateFrequency">Update frequency in Hz</param>
        public void SetUpdateFrequency(double updateFrequency)
        {
            UpdateFrequency = Math.Max(WaveformAPI.MINIMUM_FREQUENCY, updateFrequency);
        }


        public override double CalculateValueAt(double time)
        {
            // Check if enough time has passed since the last update
            UpdateTimer += DeltaTime;
            if (UpdateTimer >= 1000 / UpdateFrequency)
            {
                // Reset timer
                UpdateTimer = 0;

                // Perform chaotic update - randomize offset and amplitude
                CurrentAmplitude = RandomNumberGenerator.NextDouble() * (MaxAmplitude - MinAmplitude) +
                                   MinAmplitude;
            }

            // Return the current amplitude with the offset, ensuring the sum is within [0, 1] range
            return Math.Clamp(CurrentAmplitude + Offset, 0, 1);
        }
    }
}