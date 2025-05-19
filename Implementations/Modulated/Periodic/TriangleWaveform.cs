using NOVA.Abstract;
using NOVA.Utility;

namespace NOVA.Implementations.Modulated.Periodic
{
    /// <summary>
    /// Represents a triangle waveform generator that inherits from FAOWaveform.
    /// The waveform produces a periodic triangular signal with specified frequency, amplitude and offset.
    /// </summary>
    /// <param name="frequency">The frequency of the waveform in Hertz (Hz). Must be a positive value.</param>
    /// <param name="amplitude">The peak amplitude of the waveform, normalized to the range [0, 1]. Defaults to maximum waveform value.</param>
    /// <param name="offset">The DC offset of the waveform, normalized to the range [0, 1]. Defaults to minimum waveform value.</param>
    /// <remarks>
    /// <para>
    /// The waveform maintains the invariant that amplitude + offset ≤ 1. If the sum exceeds 1,
    /// the offset will be automatically adjusted downward to maintain this constraint.
    /// </para>
    /// <para>
    /// The triangle waveform rises linearly from offset to (offset + amplitude) during the first half period,
    /// then falls linearly back to offset during the second half period.
    /// </para>
    /// </remarks>
    public sealed class TriangleWaveform(double frequency, double amplitude = WaveformMath.WAVEFORM_MAXIMUM_VALUE,
        double offset = WaveformMath.WAVEFORM_MINIMUM_VALUE)
        : FAOWaveform(frequency, amplitude, offset)
    {
        /// <summary>
        /// Calculates the instantaneous value of the triangle waveform at the specified time.
        /// </summary>
        /// <param name="time">The time in seconds at which to evaluate the waveform.</param>
        /// <returns>
        /// The waveform value at the given time, in the range [offset, offset + amplitude].
        /// The value follows a triangular pattern over each period.
        /// </returns>
        /// <remarks>
        /// The calculation first determines the current position within the waveform's period.
        /// For the rising half of the period, the value increases linearly from offset to (offset + amplitude).
        /// For the falling half, it decreases linearly back to offset.
        /// </remarks>
        public override double CalculateValueAt(double time)
        {
            // Calculate period
            double period = WaveformMath.FrequencyToPeriod(Frequency);

            // Calculate if the waveform is in the first or second half of the period
            double timeInPeriod = WaveformMath.TimeInCycle(time, period);
            double halfPeriod = period / 2;

            // If the waveform is in the second half of the period, invert the value
            return timeInPeriod <= halfPeriod
                ? Amplitude * timeInPeriod / halfPeriod + Offset
                : Amplitude * (period - timeInPeriod) / halfPeriod + Offset;
        }
    }
}