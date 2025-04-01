namespace NOVA.Abstract
{
    /// <summary>
    /// Base class for all waveforms
    /// </summary>
    public abstract class WaveformBase() : Waveform
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
        public virtual double Duration { get; set; } = -1;

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
        public double DefaultValue { get; set; } = 0;
        
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
        public sealed override void Start()
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
        public sealed override void Stop()
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