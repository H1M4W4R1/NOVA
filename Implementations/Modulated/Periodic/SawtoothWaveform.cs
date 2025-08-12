using System.Runtime.CompilerServices;
using NOVA.Abstract;
using NOVA.Utility;

namespace NOVA.Implementations.Modulated.Periodic
{
    /// <summary>
    ///     Represents a sawtooth waveform generator that produces a periodic, linear ramp waveform.
    ///     The waveform can be configured with frequency, amplitude, offset, and inversion parameters.
    /// </summary>
    /// <param name="frequency">The frequency of the waveform in Hertz (Hz). Determines how many cycles occur per second.</param>
    /// <param name="amplitude">The peak deviation of the waveform from its center value. Must be in the range [0, 1].</param>
    /// <param name="offset">The DC offset of the waveform. Must be in the range [0, 1].</param>
    /// <param name="inverted">
    ///     When true, the waveform starts at maximum amplitude and ramps down. When false, it starts at
    ///     minimum and ramps up.
    /// </param>
    /// <remarks>
    ///     <para>
    ///         The waveform maintains the invariant that amplitude + offset ≤ 1. If the sum exceeds 1,
    ///         the offset will be automatically adjusted downward to maintain this constraint.
    ///     </para>
    ///     <para>
    ///         The waveform is periodic with the given frequency, producing a continuous sawtooth pattern
    ///         that can be either rising (standard) or falling (inverted).
    ///     </para>
    /// </remarks>
    public sealed class SawtoothWaveform(double frequency,
        double amplitude = WaveformMath.WAVEFORM_MAXIMUM_VALUE,
        double offset = WaveformMath.WAVEFORM_MINIMUM_VALUE,
        bool inverted = false
    ) : FAOWaveform(frequency, amplitude, offset)
    {
        /// <summary>
        ///     Gets or sets whether the waveform is inverted.
        /// </summary>
        /// <value>
        ///     True if the waveform is inverted (starts high and ramps down), false if it's standard (starts low and ramps up).
        /// </value>
        private bool Inverted { get; set; } = inverted;

        /// <summary>
        ///     Sets the inversion state of the sawtooth waveform.
        /// </summary>
        /// <param name="inverted">
        ///     The desired inversion state:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>True: Waveform will be inverted (falling ramp)</description>
        ///         </item>
        ///         <item>
        ///             <description>False: Waveform will be standard (rising ramp)</description>
        ///         </item>
        ///     </list>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetInverted(bool inverted) => Inverted = inverted;

        /// <summary>
        ///     Calculates the waveform's value at a specific point in time.
        /// </summary>
        /// <param name="time">The time point (in seconds) at which to evaluate the waveform.</param>
        /// <returns>
        ///     The waveform's amplitude value at the specified time, in the range [offset, offset + amplitude].
        ///     The exact value depends on the waveform's phase within its current period and its inversion state.
        /// </returns>
        /// <remarks>
        ///     The calculation follows these steps:
        ///     1. Converts frequency to period
        ///     2. Determines the phase within the current period
        ///     3. Calculates a linear ramp value based on the phase
        ///     4. Applies inversion if specified
        ///     5. Adds the DC offset
        /// </remarks>
        public override double[] CalculateValuesAt(double time)
        {
            // Calculate period
            double period = WaveformMath.FrequencyToPeriod(Frequency);

            // Calculate time in period
            double timeInPeriod = WaveformMath.TimeInCycle(time, period);

            // Calculate ramp-up value
            double rampUpValue = Amplitude * timeInPeriod / period;

            // If the waveform is inverted, invert the ramp-up value and return otherwise return the ramp-up value
            CurrentValues[0] = (Inverted ? Amplitude - rampUpValue : rampUpValue) + Offset;
            return CurrentValues;
        }
    }
}