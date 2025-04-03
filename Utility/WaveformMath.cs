namespace NOVA.Utility
{
    public static class WaveformMath
    {
        /// <summary>
        /// Resolution of the waveform in milliseconds
        /// </summary>
        public const int WAVEFORM_RESOLUTION = 4;

        /// <summary>
        /// Minimum value of the waveform 
        /// </summary>
        public const double WAVEFORM_MINIMUM_VALUE = 0;

        /// <summary>
        /// Maximum value of the waveform 
        /// </summary>
        public const double WAVEFORM_MAXIMUM_VALUE = 1;

        /// <summary>
        /// Minimum period of the waveform in milliseconds
        /// </summary>
        public const double MINIMUM_PERIOD = WAVEFORM_RESOLUTION * 2;

        /// <summary>
        /// Minimum usable period of the waveform in milliseconds
        /// </summary>
        public const double MINIMUM_USABLE_PERIOD = WAVEFORM_RESOLUTION * 10;

        /// <summary>
        /// Minimum frequency of the waveform in Hz
        /// </summary>
        public const double MINIMUM_FREQUENCY = 1000 / MINIMUM_PERIOD;

        /// <summary>
        /// Minimum usable frequency of the waveform in Hz
        /// </summary>
        public const double MINIMUM_USABLE_FREQUENCY = 1000 / MINIMUM_USABLE_PERIOD;

        /// <summary>
        /// Clamps the offset of the waveform to ensure it is in [0, 1] range and does not exceed 1 - amplitude.
        /// </summary>
        /// <param name="offset">Offset of the waveform that should be in [0, 1] range</param>
        /// <param name="amplitude">Amplitude of the waveform that should be in [0, 1] range</param>
        /// <returns>Clamped offset in [0, 1] range</returns>
        public static double ClampOffset(double offset, double amplitude = 0) =>
            Math.Min(Math.Clamp(offset, WAVEFORM_MINIMUM_VALUE, WAVEFORM_MINIMUM_VALUE),
                WAVEFORM_MAXIMUM_VALUE - Math.Clamp(amplitude, WAVEFORM_MINIMUM_VALUE, WAVEFORM_MAXIMUM_VALUE));

        /// <summary>
        /// Clamps the offset of the waveform to ensure it is in [0, 1] range and does not exceed 1 - minAmplitude or maxAmplitude.
        /// </summary>
        /// <param name="offset">Offset of the waveform that should be in [0, 1] range</param>
        /// <param name="minAmplitude">Minimum amplitude of the waveform that should be in [0, 1] range</param>
        /// <param name="maxAmplitude">Maximum amplitude of the waveform that should be in [0, 1] range</param>
        /// <returns>Clamped offset in [0, 1] range</returns>
        public static double ClampOffset(double offset, double minAmplitude, double maxAmplitude)
        {
            offset = ClampOffset(offset, maxAmplitude);
            offset = ClampOffset(offset, minAmplitude);
            return offset;
        }

        /// <summary>
        /// Clamps the amplitude of the waveform to ensure it is in [0, 1] range.
        /// </summary>
        /// <param name="amplitude">Amplitude of the waveform that should be in [0, 1] range</param>
        /// <returns>Clamped amplitude in [0, 1] range</returns>
        public static double ClampAmplitude(double amplitude) =>
            Math.Clamp(amplitude, WAVEFORM_MINIMUM_VALUE, WAVEFORM_MAXIMUM_VALUE);
        
        /// <summary>
        /// Clamps the amplitude range of the waveform to ensure it is in [0, 1] range and returns a tuple of min and max amplitude.
        /// </summary>
        /// <param name="minAmplitude">Minimum amplitude of the waveform that should be in [0, 1] range</param>
        /// <param name="maxAmplitude">Maximum amplitude of the waveform that should be in [0, 1] range</param>
        /// <returns>Tuple of clamped min and max amplitude in [0, 1] range</returns>
        public static (double, double) ClampAmplitudeRange(double minAmplitude, double maxAmplitude)
        {
            // Ensure min and max amplitude are in [0, 1] range
            minAmplitude = ClampAmplitude(minAmplitude);
            maxAmplitude = ClampAmplitude(maxAmplitude);

            // Swap min and max amplitude if min is greater than max
            if (minAmplitude > maxAmplitude)
            {
                (minAmplitude, maxAmplitude) = (maxAmplitude, minAmplitude);
            }

            // Return clamped min and max amplitude
            return (minAmplitude, maxAmplitude);
        }

        /// <summary>
        /// Ensures the frequency of the waveform is greater than or equal to the minimum frequency.
        /// </summary>
        /// <param name="frequency">Frequency of the waveform that should be greater than or equal to the minimum frequency</param>
        /// <returns>Clamped frequency that is greater than or equal to the minimum frequency</returns>
        public static double ClampFrequency(double frequency) =>
            Math.Max(frequency, MINIMUM_FREQUENCY);
    }
}