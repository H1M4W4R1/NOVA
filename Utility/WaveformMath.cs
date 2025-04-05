using System.Runtime.CompilerServices;

namespace NOVA.Utility
{
    public static class WaveformMath
    {
        /// <summary>
        /// One second in milliseconds, used for frequency and period calculations.
        /// </summary>
        public const int ONE_SECOND = 1000;

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
        /// Maximum frequency of the waveform in Hz
        /// </summary>
        public const double MAXIMUM_FREQUENCY = ONE_SECOND / MINIMUM_PERIOD;

        /// <summary>
        /// Maximum usable frequency of the waveform in Hz
        /// </summary>
        public const double MAXIMUM_USABLE_FREQUENCY = ONE_SECOND / MINIMUM_USABLE_PERIOD;

        /// <summary>
        /// Constant value used to indicate that the waveform should loop.
        /// </summary>
        public const int LOOP_WAVEFORM = -1;

        /// <summary>
        /// Clamps the offset of the waveform to ensure it is in [0, 1] range and does not exceed 1 - amplitude.
        /// </summary>
        /// <param name="offset">Offset of the waveform that should be in [0, 1] range</param>
        /// <param name="amplitude">Amplitude of the waveform that should be in [0, 1] range</param>
        /// <returns>Clamped offset in [0, 1] range</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ClampOffset(double offset, double amplitude = WAVEFORM_MINIMUM_VALUE) =>
            Math.Min(Math.Clamp(offset, WAVEFORM_MINIMUM_VALUE, WAVEFORM_MAXIMUM_VALUE),
                WAVEFORM_MAXIMUM_VALUE - Math.Clamp(amplitude, WAVEFORM_MINIMUM_VALUE, WAVEFORM_MAXIMUM_VALUE));

        /// <summary>
        /// Clamps the offset of the waveform to ensure it is in [0, 1] range and does not exceed 1 - minAmplitude or maxAmplitude.
        /// </summary>
        /// <param name="offset">Offset of the waveform that should be in [0, 1] range</param>
        /// <param name="minAmplitude">Minimum amplitude of the waveform that should be in [0, 1] range</param>
        /// <param name="maxAmplitude">Maximum amplitude of the waveform that should be in [0, 1] range</param>
        /// <returns>Clamped offset in [0, 1] range</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static double ClampAmplitude(double amplitude) =>
            Math.Clamp(amplitude, WAVEFORM_MINIMUM_VALUE, WAVEFORM_MAXIMUM_VALUE);

        /// <summary>
        /// Clamps the amplitude range of the waveform to ensure it is in [0, 1] range and returns a tuple of min and max amplitude.
        /// </summary>
        /// <param name="minAmplitude">Minimum amplitude of the waveform that should be in [0, 1] range</param>
        /// <param name="maxAmplitude">Maximum amplitude of the waveform that should be in [0, 1] range</param>
        /// <param name="useSafeClamping">If true, the min amplitude is clamped to max amplitude rather than swapped</param>
        /// <returns>Tuple of clamped min and max amplitude in [0, 1] range</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static (double, double) ClampAmplitudeRange(
            double minAmplitude,
            double maxAmplitude,
            bool useSafeClamping = true)
        {
            // Ensure min and max amplitude are in [0, 1] range
            minAmplitude = ClampAmplitude(minAmplitude);
            maxAmplitude = ClampAmplitude(maxAmplitude);

            // Swap min and max amplitude if min is greater than max or
            // clamp min to max if useSafeClamping is true
            if (useSafeClamping)
                minAmplitude = Math.Min(minAmplitude, maxAmplitude);
            else if (minAmplitude > maxAmplitude) (minAmplitude, maxAmplitude) = (maxAmplitude, minAmplitude);

            // Return clamped min and max amplitude
            return (minAmplitude, maxAmplitude);
        }

        /// <summary>
        /// Ensures the frequency of the waveform is greater than or equal to the minimum frequency.
        /// </summary>
        /// <param name="frequency">Frequency of the waveform that should be greater than or equal to the minimum frequency</param>
        /// <returns>Clamped frequency that is greater than or equal to the minimum frequency</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static double ClampFrequency(double frequency) =>
            Math.Min(frequency, MAXIMUM_FREQUENCY);

        /// <summary>
        /// Converts frequency to period in milliseconds.
        /// </summary>
        /// <param name="frequency">Frequency of the waveform in Hz</param>
        /// <returns>Period of the waveform in milliseconds</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static double FrequencyToPeriod(double frequency)
        {
            frequency = ClampFrequency(frequency);
            return ONE_SECOND / frequency;
        }

        /// <summary>
        /// Converts period to frequency in Hz.
        /// </summary>
        /// <param name="period">Period of the waveform in milliseconds</param>
        /// <returns>Frequency of the waveform in Hz</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static double PeriodToFrequency(double period)
        {
            period = Math.Max(period, MINIMUM_PERIOD);
            return ONE_SECOND / period;
        }

        /// <summary>
        /// Calculates the time in the cycle based on the current time and period.
        /// </summary>
        /// <param name="currentTime">Current time in milliseconds</param>
        /// <param name="period">Period of the waveform in milliseconds</param>
        /// <returns>Time in the cycle in milliseconds</returns>
        /// <remarks>
        /// This method ensures that the time in the cycle is always positive and within the range of [0, period).
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double TimeInCycle(double currentTime, double period)
        {
            // Safety checks
            if (period <= 0) return period;
            if (currentTime <= 0) return 0;

            // Calculate the time in the cycle
            return currentTime % period;
        }
    }
}