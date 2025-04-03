namespace NOVA.Abstract.Interfaces
{
    /// <summary>
    /// Indicates that a class is a periodic waveform - it repeats itself over time.
    /// </summary>
    public interface IPeriodicWaveform
    {
        /// <summary>
        /// Period of the waveform in milliseconds.
        /// </summary>
        public double Period { get; }
        
        /// <summary>
        /// Frequency of the waveform in Hertz.
        /// </summary>
        public double Frequency { get; }
    }
}