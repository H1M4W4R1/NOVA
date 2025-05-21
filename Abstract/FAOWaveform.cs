using System.Runtime.CompilerServices;
using NOVA.Abstract.Interfaces;
using NOVA.Utility;

namespace NOVA.Abstract
{
    /// <summary>
    ///     Abstract base class representing a waveform with modifiable frequency, amplitude and offset (FAO).
    ///     Primarily used for mathematical waveform generation and manipulation.
    /// </summary>
    /// <param name="frequency">Initial frequency of the waveform in Hertz (Hz). Minimum allowed value is 0.001Hz.</param>
    /// <param name="amplitude">Initial amplitude of the waveform, normalized to range [0, 1].</param>
    /// <param name="offset">Initial vertical offset of the waveform, normalized to range [0, 1].</param>
    /// <remarks>
    ///     <para>
    ///         The waveform maintains the invariant that amplitude + offset ≤ 1. If the sum would exceed 1,
    ///         the offset is automatically adjusted downward to maintain this constraint.
    ///     </para>
    ///     <para>
    ///         This class implements IPeriodicWaveform interface and inherits from Waveform base class.
    ///     </para>
    /// </remarks>
    public abstract class FAOWaveform(double frequency, double amplitude, double offset) : Waveform,
        IPeriodicWaveform
    {
        /// <summary>
        ///     Gets the current frequency of the waveform in Hertz (Hz).
        ///     The value is clamped to ensure it meets minimum frequency requirements.
        /// </summary>
        /// <value>Frequency in Hz, always ≥ 0.001Hz.</value>
        public double Frequency { get; private set; } = WaveformMath.ClampFrequency(frequency);

        /// <summary>
        ///     Gets the period of the waveform in milliseconds.
        ///     This is a derived property calculated from the current frequency.
        /// </summary>
        /// <value>Waveform period in milliseconds.</value>
        public double Period => WaveformMath.FrequencyToPeriod(Frequency);

        /// <summary>
        ///     Gets the current amplitude of the waveform.
        ///     Protected access allows derived classes to use this value in waveform calculations.
        /// </summary>
        /// <value>Normalized amplitude in range [0, 1].</value>
        protected double Amplitude { get; private set; } = WaveformMath.ClampAmplitude(amplitude);

        /// <summary>
        ///     Gets the current vertical offset of the waveform.
        ///     Protected access allows derived classes to use this value in waveform calculations.
        /// </summary>
        /// <value>Normalized offset in range [0, 1], constrained such that amplitude + offset ≤ 1.</value>
        protected double Offset { get; private set; } = WaveformMath.ClampOffset(offset, amplitude);

        /// <summary>
        ///     Updates the waveform's frequency to the specified value.
        /// </summary>
        /// <param name="frequency">New frequency value in Hertz (Hz). Will be clamped to valid range.</param>
        /// <remarks>
        ///     Uses aggressive inlining for performance optimization in high-frequency waveform applications.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFrequency(double frequency) => Frequency = WaveformMath.ClampFrequency(frequency);

        /// <summary>
        ///     Updates the waveform's amplitude to the specified value.
        ///     Automatically adjusts offset if necessary to maintain amplitude + offset ≤ 1 constraint.
        /// </summary>
        /// <param name="amplitude">New amplitude value in range [0, 1]. Will be clamped to valid range.</param>
        /// <remarks>
        ///     <para>
        ///         This method ensures both amplitude and offset remain within their valid ranges and maintains
        ///         their sum ≤ 1 constraint.
        ///     </para>
        ///     <para>
        ///         Uses aggressive inlining for performance optimization in high-frequency waveform applications.
        ///     </para>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetAmplitude(double amplitude)
        {
            // Clamp amplitude to valid range [0, 1]
            Amplitude = WaveformMath.ClampAmplitude(amplitude);
            // Adjust offset if needed to maintain amplitude + offset ≤ 1
            Offset = WaveformMath.ClampOffset(Offset, Amplitude);
        }

        /// <summary>
        ///     Updates the waveform's vertical offset to the specified value.
        /// </summary>
        /// <param name="offset">New offset value in range [0, 1]. Will be clamped to valid range considering current amplitude.</param>
        /// <remarks>
        ///     Uses aggressive inlining for performance optimization in high-frequency waveform applications.
        ///     The offset is constrained such that amplitude + offset ≤ 1.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetOffset(double offset) => Offset = WaveformMath.ClampOffset(offset, Amplitude);
    }
}