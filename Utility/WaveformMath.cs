using System.Runtime.CompilerServices;

namespace NOVA.Utility
{
    /// <summary>
    /// Provides mathematical operations and constants for waveform generation and manipulation.
    /// Contains methods for clamping values, converting between frequency/period, and calculating waveform timing.
    /// </summary>
    public static class WaveformMath
    {
        /// <summary>
        /// Constant representing one second in milliseconds (1000ms).
        /// Used as a base for frequency and period calculations.
        /// </summary>
        public const int ONE_SECOND = 1000;

        /// <summary>
        /// The resolution of generated waveforms in milliseconds.
        /// Determines the minimum time step for waveform calculations.
        /// </summary>
        public const int WAVEFORM_RESOLUTION = 4;

        /// <summary>
        /// The minimum normalized value for waveform parameters (0.0).
        /// </summary>
        public const double WAVEFORM_MINIMUM_VALUE = 0;

        /// <summary>
        /// The maximum normalized value for waveform parameters (1.0).
        /// </summary>
        public const double WAVEFORM_MAXIMUM_VALUE = 1;

        /// <summary>
        /// The absolute minimum period for waveforms in milliseconds.
        /// Calculated as twice the waveform resolution to ensure at least two sample points per cycle.
        /// </summary>
        public const double MINIMUM_PERIOD = WAVEFORM_RESOLUTION * 2;

        /// <summary>
        /// The minimum recommended period for practical waveform generation in milliseconds.
        /// Provides better waveform quality with 10 sample points per cycle.
        /// </summary>
        public const double MINIMUM_USABLE_PERIOD = WAVEFORM_RESOLUTION * 10;

        /// <summary>
        /// The maximum theoretical frequency in Hertz based on MINIMUM_PERIOD.
        /// Represents the absolute upper frequency limit for waveform generation.
        /// </summary>
        public const double MAXIMUM_FREQUENCY = ONE_SECOND / MINIMUM_PERIOD;

        /// <summary>
        /// The maximum recommended frequency in Hertz for practical waveform generation.
        /// Based on MINIMUM_USABLE_PERIOD for better waveform quality.
        /// </summary>
        public const double MAXIMUM_USABLE_FREQUENCY = ONE_SECOND / MINIMUM_USABLE_PERIOD;

        /// <summary>
        /// Special constant value (-1) indicating that a waveform should loop continuously.
        /// Used as a flag value in waveform generation parameters.
        /// </summary>
        public const int LOOP_WAVEFORM = -1;

        /// <summary>
        /// Clamps a waveform offset value to ensure it stays within valid bounds.
        /// The offset is constrained to [0, 1] range and adjusted to prevent amplitude overflow.
        /// </summary>
        /// <param name="offset">The desired offset value to clamp (0-1 range).</param>
        /// <param name="amplitude">The amplitude of the waveform (0-1 range). Default is 0.</param>
        /// <returns>The clamped offset value guaranteed to be within valid bounds.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ClampOffset(double offset, double amplitude = WAVEFORM_MINIMUM_VALUE) =>
            Math.Min(Math.Clamp(offset, WAVEFORM_MINIMUM_VALUE, WAVEFORM_MAXIMUM_VALUE),
                WAVEFORM_MAXIMUM_VALUE - Math.Clamp(amplitude, WAVEFORM_MINIMUM_VALUE, WAVEFORM_MAXIMUM_VALUE));

        /// <summary>
        /// Clamps a waveform offset considering both minimum and maximum amplitude bounds.
        /// Ensures the offset stays within [0, 1] range and doesn't cause amplitude overflow.
        /// </summary>
        /// <param name="offset">The desired offset value to clamp (0-1 range).</param>
        /// <param name="minAmplitude">The minimum amplitude of the waveform (0-1 range).</param>
        /// <param name="maxAmplitude">The maximum amplitude of the waveform (0-1 range).</param>
        /// <returns>The clamped offset value guaranteed to work with both amplitude extremes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ClampOffset(double offset, double minAmplitude, double maxAmplitude)
        {
            offset = ClampOffset(offset, maxAmplitude);
            offset = ClampOffset(offset, minAmplitude);
            return offset;
        }

        /// <summary>
        /// Clamps a waveform amplitude value to ensure it stays within [0, 1] range.
        /// </summary>
        /// <param name="amplitude">The amplitude value to clamp.</param>
        /// <returns>The amplitude value guaranteed to be between 0 and 1 inclusive.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static double ClampAmplitude(double amplitude) =>
            Math.Clamp(amplitude, WAVEFORM_MINIMUM_VALUE, WAVEFORM_MAXIMUM_VALUE);

        /// <summary>
        /// Clamps and validates a minimum/maximum amplitude range for waveform generation.
        /// Handles cases where min > max according to the specified clamping strategy.
        /// </summary>
        /// <param name="minAmplitude">The minimum amplitude value (0-1 range).</param>
        /// <param name="maxAmplitude">The maximum amplitude value (0-1 range).</param>
        /// <param name="useSafeClamping">
        /// When true, minAmplitude is clamped to maxAmplitude if greater.
        /// When false, the values are swapped if minAmplitude > maxAmplitude.
        /// </param>
        /// <returns>A tuple containing the validated (min, max) amplitude values.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static (double, double) ClampAmplitudeRange(
            double minAmplitude,
            double maxAmplitude,
            bool useSafeClamping = true)
        {
            minAmplitude = ClampAmplitude(minAmplitude);
            maxAmplitude = ClampAmplitude(maxAmplitude);

            if (useSafeClamping)
                minAmplitude = Math.Min(minAmplitude, maxAmplitude);
            else if (minAmplitude > maxAmplitude) (minAmplitude, maxAmplitude) = (maxAmplitude, minAmplitude);

            return (minAmplitude, maxAmplitude);
        }

        /// <summary>
        /// Limits a frequency value to the maximum supported by the waveform system.
        /// </summary>
        /// <param name="frequency">The desired frequency in Hertz.</param>
        /// <returns>The input frequency capped at MAXIMUM_FREQUENCY if necessary.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static double ClampFrequency(double frequency) =>
            Math.Min(frequency, MAXIMUM_FREQUENCY);

        /// <summary>
        /// Converts a frequency value (Hz) to its corresponding period in milliseconds.
        /// Automatically clamps the input frequency to valid range.
        /// </summary>
        /// <param name="frequency">The frequency to convert, in Hertz.</param>
        /// <returns>The period in milliseconds corresponding to the frequency.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static double FrequencyToPeriod(double frequency)
        {
            frequency = ClampFrequency(frequency);
            return ONE_SECOND / frequency;
        }

        /// <summary>
        /// Converts a period value (ms) to its corresponding frequency in Hertz.
        /// Ensures the period doesn't fall below the minimum supported value.
        /// </summary>
        /// <param name="period">The period to convert, in milliseconds.</param>
        /// <returns>The frequency in Hertz corresponding to the period.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public static double PeriodToFrequency(double period)
        {
            period = Math.Max(period, MINIMUM_PERIOD);
            return ONE_SECOND / period;
        }

        /// <summary>
        /// Calculates the current position within a waveform cycle given absolute time.
        /// Handles edge cases and ensures the result is always within [0, period) range.
        /// </summary>
        /// <param name="currentTime">The absolute time in milliseconds.</param>
        /// <param name="period">The waveform period in milliseconds.</param>
        /// <returns>
        /// The time position within the current cycle, in milliseconds.
        /// Returns 0 for invalid currentTime and returns period unchanged if it's invalid.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double TimeInCycle(double currentTime, double period)
        {
            if (period <= 0) return period;
            if (currentTime <= 0) return 0;

            return currentTime % period;
        }
    }
}