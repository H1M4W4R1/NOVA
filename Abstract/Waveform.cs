using System.Runtime.CompilerServices;
using NOVA.Abstract.Interfaces;
using NOVA.Utility;

namespace NOVA.Abstract
{
    /// <summary>
    ///     Abstract base class that defines the core functionality for all waveform implementations.
    ///     Provides timing control, state management, and event handling for waveform generation.
    /// </summary>
    public abstract class Waveform(int nParameters = 1)
    {
        protected double[] CurrentValues { get; } = new double[nParameters];

        /// <summary>
        ///     Gets the UTC timestamp when the waveform was started. Protected for derived class access.
        /// </summary>
        protected DateTime StartTime { get; private set; }

        /// <summary>
        ///     Gets the UTC timestamp of the previous update tick. Protected for derived class access.
        /// </summary>
        protected DateTime PreviousTickTime { get; private set; }

        /// <summary>
        ///     Gets the UTC timestamp of the most recent update tick. Protected for derived class access.
        /// </summary>
        protected DateTime LastTickTime { get; private set; }

        /// <summary>
        ///     Indicates whether the waveform is currently active and generating values.
        ///     False indicates the waveform is stopped and not updating.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        ///     Gets the elapsed time in milliseconds since the waveform started.
        ///     Calculated as the difference between current time and start time.
        /// </summary>
        public double TimeSinceStart => (LastTickTime - StartTime).TotalMilliseconds;

        /// <summary>
        ///     Gets the time delta in milliseconds between the last two updates.
        ///     Useful for frame-rate independent calculations.
        /// </summary>
        public double DeltaTime => (LastTickTime - PreviousTickTime).TotalMilliseconds;

        /// <summary>
        ///     Abstract method that derived classes must implement to calculate the waveform's value at a specific time.
        /// </summary>
        /// <param name="time">The time in milliseconds since waveform start</param>
        /// <returns>The calculated waveform value at the specified time</returns>
        /// <remarks>
        ///     Implementations must support looping behavior when the waveform duration is exceeded.
        /// </remarks>
        public abstract double[] CalculateValuesAt(double time);

        /// <summary>
        ///     Gets or sets the total duration of the waveform in milliseconds.
        ///     A value less than 0 indicates an infinite/looping waveform.
        ///     Protected setter allows modification by derived classes.
        /// </summary>
        public double Duration { get; protected set; } = WaveformMath.LOOP_WAVEFORM;

        /// <summary>
        ///     Determines whether the waveform is infinite (continuously loops).
        /// </summary>
        /// <value>
        ///     True if Duration is negative (infinite), false for finite durations.
        /// </value>
        public bool IsInfinite => Duration < 0;

        /// <summary>
        ///     Gets or sets the default output value used when the waveform starts or stops.
        ///     Protected setter allows modification by derived classes.
        /// </summary>
        /// <remarks>
        ///     The value is automatically clamped to the range [0, 1]. Can be changed
        ///     dynamically during runtime to meet specific application requirements.
        /// </remarks>
        public double[] DefaultValues = new double[nParameters];

        /// <summary>
        ///     Updates the waveform's duration if supported by the implementation.
        /// </summary>
        /// <param name="milliseconds">New duration in milliseconds. Negative values indicate infinite duration.</param>
        /// <param name="silentException">When true, suppresses exceptions for unsupported operations</param>
        /// <exception cref="InvalidOperationException">
        ///     Thrown if the waveform implements IStaticDurationWaveform and silentException is false
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDuration(double milliseconds, bool silentException = false)
        {
            if (this is IStaticDurationWaveform)
            {
                if (!silentException)
                    throw new InvalidOperationException("This waveform does not support dynamic duration.");
                return;
            }

            Duration = milliseconds < 0 ? WaveformMath.LOOP_WAVEFORM : milliseconds;
        }

        /// <summary>
        ///     Sets the default output value for the waveform, clamped to valid range.
        /// </summary>
        /// <param name="values">Array of new default value in range [0, 1]</param>
        /// <exception cref="ArgumentException">Invalid amount of values provided.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void SetDefaultValues(Span<double> values)
        {
            if (values.Length != nParameters)
                throw new ArgumentException("The waveform default values must have the same length as the number of parameters.");

            for (int n = 0; n < DefaultValues.Length; n++) 
                DefaultValues[n] = WaveformMath.ClampAmplitude(values[n]);
        }

        /// <summary>
        ///     Adjusts the waveform's timeline by shifting its start time.
        /// </summary>
        /// <param name="milliseconds">Time shift in milliseconds (positive or negative)</param>
        /// <remarks>
        ///     Has no effect if the waveform is not running. Triggers an immediate update after shifting.
        /// </remarks>
        public void ShiftTime(double milliseconds)
        {
            if (!IsRunning) return;
            StartTime = StartTime.AddMilliseconds(milliseconds);
            Update();
        }

        /// <summary>
        ///     Event triggered when the waveform begins generating values.
        /// </summary>
        public WaveformStartHandler? OnWaveformStart = delegate { };

        /// <summary>
        ///     Event triggered whenever the waveform's output value changes.
        /// </summary>
        public WaveformValuesChanged? OnWaveformValuesChanged = delegate { };

        /// <summary>
        ///     Event triggered whenever the waveform's output value changes.
        /// </summary>
        public WaveformValueChangedHandler? OnWaveformValueChanged = delegate { };

        /// <summary>
        ///     Event triggered when the waveform stops generating values.
        /// </summary>
        public WaveformEndHandler? OnWaveformEnd = delegate { };

        /// <summary>
        ///     Starts waveform generation using the current UTC time as reference.
        /// </summary>
        public void Start() => Start(null);

        /// <summary>
        ///     Starts waveform generation synchronized with another waveform's timeline.
        /// </summary>
        /// <param name="waveform">Reference waveform to synchronize with</param>
        /// <param name="shiftMilliseconds">Optional time offset in milliseconds</param>
        /// <remarks>
        ///     No effect if reference waveform isn't running. Applies time offset before starting.
        /// </remarks>
        public void StartSynchronizedWith(Waveform waveform, double shiftMilliseconds = 0)
        {
            if (!waveform.IsRunning) return;
            DateTime startTime = waveform.StartTime;
            startTime = startTime.AddMilliseconds(shiftMilliseconds);
            Start(startTime);
        }

        /// <summary>
        ///     Internal implementation of waveform start with optional specific start time.
        /// </summary>
        /// <param name="startTime">Optional specific UTC start time</param>
        private void Start(DateTime? startTime)
        {
            if (IsRunning) return;
            StartTime = startTime ?? DateTime.UtcNow;
            LastTickTime = StartTime;
            IsRunning = true;
            OnWaveformStart?.Invoke();
            OnWaveformValuesChanged?.Invoke(DefaultValues);
            if(DefaultValues.Length > 0)
                OnWaveformValueChanged?.Invoke(DefaultValues[0]);
            WaveformAPI.RegisterWaveform(this);
        }

        /// <summary>
        ///     Stops waveform generation and resets all timing state.
        /// </summary>
        /// <remarks>
        ///     Triggers value reset to default and unregisters from the waveform API.
        /// </remarks>
        public void Stop()
        {
            if (!IsRunning) return;
            StartTime = PreviousTickTime = LastTickTime = DateTime.MinValue;
            IsRunning = false;
            OnWaveformValuesChanged?.Invoke(DefaultValues);
            if(DefaultValues.Length > 0)
                OnWaveformValueChanged?.Invoke(DefaultValues[0]);
            OnWaveformEnd?.Invoke();
            WaveformAPI.UnregisterWaveform(this);
        }

        /// <summary>
        ///     Internal method to update waveform state using current UTC time.
        /// </summary>
        internal void Update() => _Update(DateTime.UtcNow);

        /// <summary>
        ///     Core update logic that advances the waveform's timeline and calculates new values.
        /// </summary>
        /// <param name="currentDateUtc">Current UTC timestamp for synchronization</param>
        private void _Update(DateTime currentDateUtc)
        {
            if (!IsRunning) return;
            PreviousTickTime = LastTickTime;
            LastTickTime = currentDateUtc;
            if (!IsInfinite && TimeSinceStart > Duration)
            {
                Stop();
                return;
            }

            // Invoke value changed event
            Span<double> values = CalculateValuesAt(TimeSinceStart);
            OnWaveformValuesChanged?.Invoke(values);
            if (values.Length > 0) OnWaveformValueChanged?.Invoke(values[0]);
        }
    }
}