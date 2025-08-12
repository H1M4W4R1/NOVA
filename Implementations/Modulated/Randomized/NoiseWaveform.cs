using System.Runtime.CompilerServices;
using NOVA.Abstract;
using NOVA.Utility;

namespace NOVA.Implementations.Modulated.Randomized
{
    /// <summary>
    ///     A noise waveform implementation that generates random values within specified amplitude and offset ranges.
    ///     The waveform updates its value at minimum intervals of one millisecond or the WaveformAPI tick rate,
    ///     whichever is longer. This creates a stepped noise pattern rather than continuous random values.
    /// </summary>
    /// <param name="amplitude">The peak deviation of the waveform from its offset, clamped to [0, 1] range</param>
    /// <param name="offset">The DC offset of the waveform, clamped to ensure amplitude + offset ≤ 1</param>
    public sealed class NoiseWaveform(double amplitude, double offset = 0) : Waveform
    {
        /// <summary>
        ///     The timestamp of the last value calculation, used to determine when to generate a new random value
        /// </summary>
        private double _lastCalculatedTime;

        /// <summary>
        ///     The last calculated random value, cached until the next update interval
        /// </summary>
        private double _lastCalculatedValue;

        /// <summary>
        ///     Gets the current amplitude of the waveform.
        ///     The amplitude determines the range of random values generated, where 1.0 represents full scale.
        ///     Value is always clamped to [0, 1] range.
        /// </summary>
        public double Amplitude { get; private set; } = WaveformMath.ClampAmplitude(amplitude);

        /// <summary>
        ///     Gets the current offset of the waveform.
        ///     The offset shifts all generated values by this amount.
        ///     Value is clamped to ensure amplitude + offset ≤ 1.
        /// </summary>
        public double Offset { get; private set; } = WaveformMath.ClampOffset(offset, amplitude);

        /// <summary>
        ///     Updates the amplitude of the waveform and adjusts offset if necessary to maintain valid ranges.
        ///     Both amplitude and offset will be clamped to ensure they remain within valid ranges and their sum ≤ 1.
        /// </summary>
        /// <param name="amplitude">The new amplitude value in [0, 1] range</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetAmplitude(double amplitude)
        {
            // Clamp amplitude to valid range and adjust offset if needed
            Amplitude = WaveformMath.ClampAmplitude(amplitude);
            Offset = WaveformMath.ClampOffset(Offset, Amplitude);
        }

        /// <summary>
        ///     Updates the offset of the waveform while ensuring it remains within valid range relative to current amplitude.
        ///     The offset will be clamped to ensure amplitude + offset ≤ 1.
        /// </summary>
        /// <param name="offset">The new offset value in [0, 1] range</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetOffset(double offset) => Offset = WaveformMath.ClampOffset(offset, Amplitude);

        /// <summary>
        ///     Calculates the current value of the waveform at the specified time.
        ///     Generates a new random value if the minimum update interval has elapsed since last calculation.
        /// </summary>
        /// <param name="time">The current time in seconds</param>
        /// <returns>A random value in [offset, offset+amplitude] range that remains constant for each update interval</returns>
        public override double[] CalculateValuesAt(double time)
        {
            // Regenerate value if minimum update interval has elapsed
            if (Math.Abs(time - _lastCalculatedTime) > WaveformMath.MINIMUM_USABLE_PERIOD)
            {
                // Generate new random value scaled by amplitude and shifted by offset
                _lastCalculatedValue = Random.Shared.NextDouble() * Amplitude + Offset;
                _lastCalculatedTime = time;
            }
            
            CurrentValues[0] =  _lastCalculatedValue;
            return CurrentValues;
        }
    }
}