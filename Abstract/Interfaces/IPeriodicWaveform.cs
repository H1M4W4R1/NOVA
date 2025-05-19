namespace NOVA.Abstract.Interfaces
{
    /// <summary>
    /// Defines the fundamental characteristics of a periodic waveform - a signal that repeats its pattern
    /// at regular intervals over time. Implement this interface to indicate that a class represents
    /// a periodic waveform with consistent temporal properties.
    /// </summary>
    /// <remarks>
    /// Implementations should ensure that Period and Frequency maintain mathematical consistency:
    /// Frequency = 1000 / Period (since Period is in milliseconds and Frequency is in Hertz)
    /// </remarks>
    public interface IPeriodicWaveform
    {
        /// <summary>
        /// Gets the duration of one complete cycle of the waveform in milliseconds.
        /// </summary>
        /// <value>
        /// A positive double value representing the time taken for one full repetition of the waveform.
        /// For example, a waveform repeating every 2 seconds would have a Period of 2000.0.
        /// </value>
        public double Period { get; }
        
        /// <summary>
        /// Gets the number of waveform cycles that occur per second, measured in Hertz (Hz).
        /// </summary>
        /// <value>
        /// A positive double value representing the repetition rate of the waveform.
        /// For example, a waveform repeating 50 times per second would have a Frequency of 50.0.
        /// </value>
        /// <remarks>
        /// This property should be mathematically related to Period such that:
        /// Frequency = 1000 / Period
        /// Implementations may choose to calculate this dynamically or store it separately.
        /// </remarks>
        public double Frequency { get; }
    }
}