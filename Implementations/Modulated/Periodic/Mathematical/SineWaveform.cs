using NOVA.Abstract;
using NOVA.Utility;

namespace NOVA.Implementations.Modulated.Periodic.Mathematical
{
    /// <summary>
    ///     Represents a sinusoidal waveform generator that produces values following a sine function.
    ///     This waveform is periodic and mathematically defined, inheriting from FAOWaveform base class.
    /// </summary>
    /// <remarks>
    ///     The waveform produces values in the range [0, 1] when amplitude is 1 and offset is 0.
    ///     The sine wave is phase-shifted by -π/2 to start at its minimum value at time 0.
    /// </remarks>
    /// <param name="frequency">
    ///     The frequency of the waveform in Hertz (Hz). Determines how many complete cycles occur per
    ///     second.
    /// </param>
    /// <param name="amplitude">
    ///     The peak deviation of the waveform from its center position. Must be in range [0, 1]. Defaults
    ///     to maximum waveform value.
    /// </param>
    /// <param name="offset">
    ///     The vertical offset applied to the waveform. Must be in range [0, 1]. Defaults to minimum waveform
    ///     value.
    /// </param>
    public sealed class SineWaveform(double frequency,
        double amplitude = WaveformMath.WAVEFORM_MAXIMUM_VALUE,
        double offset = WaveformMath.WAVEFORM_MINIMUM_VALUE
    )
        : FAOWaveform(frequency, amplitude, offset)
    {
        /// <summary>
        ///     Calculates the waveform's value at a specific point in time.
        /// </summary>
        /// <param name="time">The time point in milliseconds at which to evaluate the waveform.</param>
        /// <returns>A double value between 0 and 1 representing the waveform's amplitude at the given time.</returns>
        /// <remarks>
        ///     The calculation follows the formula:
        ///     ((sin(-π/2 + 2πft) * A + A) / 2) + O
        ///     where f is frequency, A is amplitude, O is offset, and t is time in milliseconds.
        ///     The -π/2 phase shift ensures the waveform starts at its minimum value.
        /// </remarks>
        public override double[] CalculateValuesAt(double time)
        {
            CurrentValues[0] = (Math.Sin(-Math.PI / 2 + 2 * Math.PI * Frequency * time / 1000) * Amplitude + Amplitude) / 2 + Offset;
            return CurrentValues;
        }
    }
}