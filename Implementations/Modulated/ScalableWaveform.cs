using System.Runtime.CompilerServices;
using NOVA.Abstract;
using NOVA.Utility;

namespace NOVA.Implementations.Modulated
{
    /// <summary>
    ///     Represents a constant amplitude waveform where the amplitude can be dynamically adjusted.
    ///     This waveform maintains a constant output value equal to its current amplitude setting,
    ///     making it useful for scenarios requiring a simple, adjustable DC level.
    /// </summary>
    /// <param name="startAmplitude">
    ///     The initial amplitude value for the waveform, clamped to the range [0, 1].
    ///     This determines the constant output value of the waveform.
    /// </param>
    /// <remarks>
    ///     <para>
    ///         The ScalableWaveform provides a simple way to generate a constant signal whose level
    ///         can be modified at runtime. The amplitude is automatically clamped to ensure it stays
    ///         within the valid [0, 1] range.
    ///     </para>
    ///     <para>
    ///         Unlike other waveforms that vary over time, this waveform always returns its current
    ///         amplitude value regardless of the input time parameter.
    ///     </para>
    /// </remarks>
    public sealed class ScalableWaveform(double startAmplitude) : Waveform
    {
        /// <summary>
        ///     Gets or sets the current amplitude of the waveform.
        ///     The value is automatically clamped to the range [0, 1] when set.
        /// </summary>
        /// <value>
        ///     The current amplitude level, always between 0 and 1 inclusive.
        /// </value>
        private double Amplitude { get; set; } = WaveformMath.ClampAmplitude(startAmplitude);

        /// <summary>
        ///     Updates the amplitude of the waveform to the specified value.
        ///     The input value is automatically clamped to the valid [0, 1] range.
        /// </summary>
        /// <param name="amplitude">
        ///     The new amplitude value to set. Values outside [0, 1] will be clamped.
        /// </param>
        /// <remarks>
        ///     This method is aggressively inlined for performance optimization.
        ///     Changes to the amplitude take effect immediately for subsequent waveform calculations.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetAmplitude(double amplitude) => Amplitude = WaveformMath.ClampAmplitude(amplitude);

        /// <summary>
        ///     Calculates the waveform's value at a specific point in time.
        ///     For ScalableWaveform, this always returns the current amplitude value regardless of time.
        /// </summary>
        /// <param name="time">The time point at which to evaluate the waveform (ignored for this waveform type).</param>
        /// <returns>
        ///     The current amplitude value of the waveform, always in the range [0, 1].
        /// </returns>
        public override double[] CalculateValuesAt(double time)
        {
            CurrentValues[0] = Amplitude;
            return CurrentValues;
        }
    }
}