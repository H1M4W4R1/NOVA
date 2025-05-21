using System.Runtime.CompilerServices;
using NOVA.Abstract;
using NOVA.Abstract.Interfaces;
using NOVA.Utility;

namespace NOVA.Implementations.Modulated
{
    /// <summary>
    ///     Represents a linear ramp waveform that transitions between two values over a specified duration.
    ///     The waveform produces a continuous linear interpolation from start value to end value.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The waveform maintains the invariant that both start and end values are clamped to [0, 1] range.
    ///         The duration must be a positive value representing the time in milliseconds for the full transition.
    ///     </para>
    ///     <para>
    ///         The waveform implements IStaticDurationWaveform, indicating it has a fixed duration after which
    ///         the value remains constant at the end value.
    ///     </para>
    /// </remarks>
    public sealed class RampWaveform : Waveform, IStaticDurationWaveform
    {
        /// <summary>
        ///     Initializes a new instance of the RampWaveform class with specified start, end values and duration.
        /// </summary>
        /// <param name="from">The initial value of the waveform, clamped to [0, 1] range.</param>
        /// <param name="to">The target value of the waveform, clamped to [0, 1] range.</param>
        /// <param name="duration">The total transition time in milliseconds. Must be positive.</param>
        /// <remarks>
        ///     Both start and end values are automatically clamped to ensure they remain within valid range.
        ///     The duration determines how quickly the transition occurs - larger values result in slower ramps.
        /// </remarks>
        public RampWaveform(double from, double to, double duration)
        {
            StartValue = WaveformMath.ClampAmplitude(from);
            EndValue = WaveformMath.ClampAmplitude(to);
            Duration = duration;
        }

        /// <summary>
        ///     Gets or sets the starting value of the ramp waveform.
        /// </summary>
        /// <value>
        ///     A double between 0 and 1 inclusive, representing the initial output value.
        /// </value>
        private double StartValue { get; set; }

        /// <summary>
        ///     Gets or sets the ending value of the ramp waveform.
        /// </summary>
        /// <value>
        ///     A double between 0 and 1 inclusive, representing the target output value.
        /// </value>
        private double EndValue { get; set; }

        /// <summary>
        ///     Updates the starting value of the ramp waveform.
        /// </summary>
        /// <param name="value">The new starting value, clamped to [0, 1] range.</param>
        /// <remarks>
        ///     This method is aggressively inlined for performance optimization.
        ///     Changing the start value immediately affects subsequent waveform calculations.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetStartValue(double value) => StartValue = WaveformMath.ClampAmplitude(value);

        /// <summary>
        ///     Updates the ending value of the ramp waveform.
        /// </summary>
        /// <param name="value">The new ending value, clamped to [0, 1] range.</param>
        /// <remarks>
        ///     This method is aggressively inlined for performance optimization.
        ///     Changing the end value immediately affects subsequent waveform calculations.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetEndValue(double value) => EndValue = WaveformMath.ClampAmplitude(value);

        /// <summary>
        ///     Calculates the waveform's value at a specific point in time.
        /// </summary>
        /// <param name="time">The elapsed time in milliseconds since waveform start.</param>
        /// <returns>
        ///     The interpolated value between start and end values based on elapsed time.
        ///     Returns start value when time ≤ 0, end value when time ≥ duration, and
        ///     a linearly interpolated value between them otherwise.
        /// </returns>
        /// <remarks>
        ///     The calculation clamps the time ratio to [0, 1] range to ensure the output
        ///     never exceeds the specified start/end values regardless of input time.
        /// </remarks>
        public override double CalculateValueAt(double time)
            => StartValue + (EndValue - StartValue) * Math.Clamp(time / Duration, 0, 1); // We clamp duration to [0, 1] range to prevent the value from going below 0 or above 1
    }
}