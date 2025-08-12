using NOVA.Abstract;

namespace NOVA.Implementations
{
    /// <summary>
    ///     Represents a constant (DC) waveform that maintains a fixed amplitude value indefinitely.
    ///     <para>
    ///         This waveform implementation produces a continuous, unchanging output value regardless
    ///         of elapsed time. It inherits from <see cref="Waveform" /> but overrides the standard
    ///         time-dependent behavior with a constant value.
    ///     </para>
    ///     <remarks>
    ///         Key characteristics:
    ///         <list type="bullet">
    ///             <item>Infinite duration (automatically inherits looping behavior)</item>
    ///             <item>Time-independent output value</item>
    ///             <item>Sealed to prevent further inheritance</item>
    ///         </list>
    ///         The waveform maintains thread safety through the base <see cref="Waveform" /> class's
    ///         implementation while providing maximum performance for constant value generation.
    ///     </remarks>
    /// </summary>
    /// <param name="amplitude">
    ///     The fixed output value of the waveform, normalized to range [0, 1].
    ///     Values outside this range will be automatically clamped by the base class.
    /// </param>
    public sealed class ConstantWaveform(double amplitude) : Waveform
    {
        /// <summary>
        ///     Calculates the waveform's value at the specified time.
        ///     <para>
        ///         For <see cref="ConstantWaveform" />, this always returns the constant amplitude value
        ///         regardless of the input time parameter. The time parameter is accepted but ignored
        ///         to maintain interface compatibility with time-varying waveforms.
        ///     </para>
        /// </summary>
        /// <param name="time">
        ///     The time in milliseconds since waveform start (unused in this implementation).
        /// </param>
        /// <returns>The constant amplitude value set during construction.</returns>
        public override double[] CalculateValuesAt(double time)
        {
            CurrentValues[0] = amplitude;
            return CurrentValues;
        }
    }
}