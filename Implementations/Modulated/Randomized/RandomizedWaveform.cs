using System.Runtime.CompilerServices;
using NOVA.Abstract;
using NOVA.Utility;

namespace NOVA.Implementations.Modulated.Randomized
{
    /// <summary>
    ///     Implements a randomized waveform generator that produces values by randomly varying amplitude
    ///     while maintaining a constant offset. The waveform updates at a specified frequency.
    /// </summary>
    /// <param name="updateFrequency">Frequency at which the waveform updates its random value, in Hertz (Hz)</param>
    /// <param name="amplitude">Maximum amplitude of the waveform, normalized to [0, 1] range</param>
    /// <param name="offset">DC offset of the waveform, normalized to [0, 1] range</param>
    /// <remarks>
    ///     The offset is automatically constrained such that offset + amplitude ≤ 1 to prevent clipping.
    ///     All input parameters are automatically clamped to their valid ranges.
    /// </remarks>
    public sealed class RandomizedWaveform(double updateFrequency,
        double amplitude = WaveformMath.WAVEFORM_MAXIMUM_VALUE,
        double offset = WaveformMath.WAVEFORM_MINIMUM_VALUE
    )
        : Waveform
    {
        /// <summary>
        ///     Gets or sets the current DC offset of the waveform.
        ///     Value is automatically clamped to ensure offset + amplitude ≤ 1.
        /// </summary>
        /// <value>Normalized offset value in range [0, 1]</value>
        private double Offset { get; set; } = WaveformMath.ClampOffset(offset, amplitude);

        /// <summary>
        ///     Gets or sets the current amplitude of the waveform.
        ///     This value is randomly updated at the specified frequency.
        /// </summary>
        /// <value>Normalized amplitude value in range [0, amplitude]</value>
        private double CurrentAmplitude { get; set; }

        /// <summary>
        ///     Tracks time elapsed since last waveform update.
        /// </summary>
        /// <value>Time in milliseconds</value>
        private double UpdateTimer { get; set; }

        /// <summary>
        ///     Gets the frequency at which the waveform updates its random value.
        /// </summary>
        /// <value>Update frequency in Hertz (Hz)</value>
        public double UpdateFrequency { get; private set; } = WaveformMath.ClampFrequency(updateFrequency);

        /// <summary>
        ///     Gets the maximum amplitude of the waveform.
        /// </summary>
        /// <value>Normalized maximum amplitude in range [0, 1]</value>
        public double Amplitude { get; private set; } = WaveformMath.ClampAmplitude(amplitude);

        /// <summary>
        ///     Sets a new maximum amplitude for the waveform.
        /// </summary>
        /// <param name="maxAmplitude">New maximum amplitude in [0, 1] range</param>
        /// <remarks>
        ///     Automatically clamps the input value and adjusts offset if necessary.
        ///     Uses aggressive inlining for performance optimization.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetAmplitude(double maxAmplitude)
        {
            Amplitude = WaveformMath.ClampAmplitude(maxAmplitude);
            Offset = WaveformMath.ClampOffset(Offset, Amplitude);
        }

        /// <summary>
        ///     Sets a new update frequency for the waveform.
        /// </summary>
        /// <param name="updateFrequency">New update frequency in Hertz (Hz)</param>
        /// <remarks>
        ///     Automatically clamps the input value to valid range.
        ///     Uses aggressive inlining for performance optimization.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUpdateFrequency(double updateFrequency)
        {
            UpdateFrequency = WaveformMath.ClampFrequency(updateFrequency);
        }

        /// <summary>
        ///     Sets a new DC offset for the waveform.
        /// </summary>
        /// <param name="offset">New offset value in [0, 1] range</param>
        /// <remarks>
        ///     Automatically clamps the input value to ensure offset + amplitude ≤ 1.
        ///     Uses aggressive inlining for performance optimization.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetOffset(double offset) => Offset = WaveformMath.ClampOffset(offset, Amplitude);

        /// <summary>
        ///     Calculates the current waveform value at the specified time.
        /// </summary>
        /// <param name="time">Current time (unused in this implementation)</param>
        /// <returns>Current waveform value in [0, 1] range</returns>
        /// <remarks>
        ///     Updates the random amplitude at the specified frequency and returns
        ///     the sum of current amplitude and offset, clamped to [0, 1] range.
        /// </remarks>
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