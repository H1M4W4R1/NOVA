using System.Runtime.CompilerServices;
using NOVA.Abstract.Interfaces;
using NOVA.Utility;

namespace NOVA.Abstract
{
    /// <summary>
    /// Base class for all waveforms
    /// </summary>
    public abstract class Waveform
    {
        /// <summary>
        /// Time when the waveform started
        /// </summary>
        protected DateTime StartTime { get; private set; }

        /// <summary>
        /// Tick before the last tick
        /// </summary>
        protected DateTime PreviousTickTime { get; private set; }

        /// <summary>
        /// Time when last update was received
        /// </summary>
        protected DateTime LastTickTime { get; private set; }

        /// <summary>
        /// Indicates whether the waveform is running, if false
        /// then the waveform is stopped
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Time since the waveform started
        /// </summary>
        public double TimeSinceStart => (LastTickTime - StartTime).TotalMilliseconds;

        /// <summary>
        /// Total time of the last update in milliseconds
        /// </summary>
        public double DeltaTime => (LastTickTime - PreviousTickTime).TotalMilliseconds;
        
        /// <summary>
        /// Calculates the value of the waveform at the given time.
        /// <i>Must support looping.</i>
        /// </summary>
        /// <param name="time">Time in milliseconds</param>
        /// <returns>Value of the waveform at the given time</returns>
        public abstract double CalculateValueAt(double time);

        /// <summary>
        /// Duration of the waveform in milliseconds, -1 if infinite
        /// </summary>
        public double Duration { get; protected set; } = WaveformMath.LOOP_WAVEFORM;

        /// <summary>
        /// Checks whether the waveform is infinite (automatically loops)
        /// </summary>
        /// <remarks>
        /// Waveform is always infinite if duration is less than zero.
        /// </remarks>
        public bool IsInfinite => Duration < 0;

        /// <summary>
        /// Default value of the waveform. Set when waveform starts or stops.
        /// </summary>
        /// <remarks>
        /// Changeable to allow for dynamic default value changes during runtime to fit specific requirements.
        /// </remarks>
        public double DefaultValue { get; protected set; }

        /// <summary>
        /// Sets the duration of the waveform.
        /// </summary>
        /// <param name="milliseconds">Duration in milliseconds</param>
        /// <param name="silentException">Exception will not be thrown if set to true, function will just return</param>
        /// <exception cref="InvalidOperationException">Thrown if the waveform does not support dynamic duration</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDuration(double milliseconds, bool silentException = false)
        {
            // Check if the waveform supports dynamic duration
            if (this is IStaticDurationWaveform)
            {
                if(!silentException)
                    throw new InvalidOperationException("This waveform does not support dynamic duration.");
                
                return;
            }

            // Set duration to the given milliseconds or loop waveform if negative
            Duration = milliseconds < 0 ? WaveformMath.LOOP_WAVEFORM : milliseconds;
        }
        
        /// <summary>
        /// Sets the default value of the waveform.
        /// </summary>
        /// <param name="value">Default value in [0, 1] range</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetDefaultValue(double value)
        {
            // Set default value
            DefaultValue = WaveformMath.ClampAmplitude(value);
        }
        
        /// <summary>
        /// Shifts the start time of the waveform by the given milliseconds.
        /// </summary>
        /// <param name="milliseconds">Milliseconds to shift the start time by</param>
        public void ShiftTime(double milliseconds)
        {
            // Check if waveform is running
            if (!IsRunning) return;
            
            // Shift the start time by the given milliseconds
            StartTime = StartTime.AddMilliseconds(milliseconds);

            // Update the last tick time, previous tick time and current value of the waveform
            Update();
        }
        
        /// <summary>
        /// Event raised when the waveform starts
        /// </summary>
        public WaveformStartHandler? OnWaveformStart = delegate { };

        /// <summary>
        /// Event raised when the value of the waveform changes
        /// </summary>
        public WaveformValueChangedHandler? OnWaveformValueChanged = delegate { };

        /// <summary>
        /// Event raised when the waveform ends
        /// </summary>
        public WaveformEndHandler? OnWaveformEnd = delegate { };

        /// <summary>
        /// Starts the waveform
        /// </summary>
        public void Start()
        {
            // Ensure the waveform is not already running
            if (IsRunning) return;

            // Update times and set the running flag
            StartTime = DateTime.UtcNow;
            LastTickTime = StartTime;
            IsRunning = true;

            // Raise the start event and set the default value
            OnWaveformStart?.Invoke();
            OnWaveformValueChanged?.Invoke(DefaultValue);

            // Register the waveform in the API
            WaveformAPI.RegisterWaveform(this);
        }

        /// <summary>
        /// Stops the waveform
        /// </summary>
        public void Stop()
        {
            // Ensure the waveform is running
            if (!IsRunning) return;
                
            // Reset times and the running flag
            StartTime = PreviousTickTime = LastTickTime = DateTime.MinValue;
            IsRunning = false;

            // Reset value to default and raise the end event
            OnWaveformValueChanged?.Invoke(DefaultValue);
            OnWaveformEnd?.Invoke();

            // Unregister the waveform from the API
            WaveformAPI.UnregisterWaveform(this);
        }

        /// <summary>
        /// Updates the waveform
        /// </summary>
        internal void Update() => _Update(DateTime.UtcNow);

        /// <summary>
        /// Updates the waveform using UTC time, used mostly for synchronization
        /// </summary>
        private void _Update(DateTime currentDateUtc)
        {
            if (!IsRunning) return;

            // Update the last tick time, must be UTC to prevent issues with time zones
            PreviousTickTime = LastTickTime;
            LastTickTime = currentDateUtc;

            // Stop the waveform if it is not infinite and the duration has passed
            if (!IsInfinite && TimeSinceStart > Duration)
            {
                Stop();
                return;
            }

            // Update the waveform value
            OnWaveformValueChanged?.Invoke(CalculateValueAt(TimeSinceStart));
        }
    }
}