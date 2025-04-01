using NOVA.Abstract;

namespace NOVA.Implementations.Combined
{
    /// <summary>
    /// Represents a sequential waveform - a waveform that is composed of multiple waveforms
    /// </summary>
    public sealed class SequentialWaveform : WaveformBase
    {
        /// <summary>
        /// Waveforms that make up the sequential waveform
        /// </summary>
        private readonly WaveformBase[] _waveforms;
        
        /// <summary>
        /// Duration of the waveform in milliseconds
        /// </summary>
        private double Period { get; init; }
        
        public override double CalculateValueAt(double time)
        {
            // Calculate index of waveform with specified time
            int index = 0;
            double timeLeft = time;
            
            // While time is larger than current waveform duration, move to next waveform
            // and subtract the duration of the current waveform from the time
            // this automatically skips whole waveforms until the correct one is found
            while (timeLeft > _waveforms[index].Duration)
            {
                timeLeft -= _waveforms[index].Duration;
                index++;
                
                // If index is out of bounds, return to first waveform
                if (index >= _waveforms.Length) index = 0;
            }
            
            // Calculate value of the waveform
            return _waveforms[index].CalculateValueAt(timeLeft);
        }
        
        public SequentialWaveform(bool loopWaveform = false, params WaveformBase[] waveforms)
        {
            _waveforms = waveforms;
            
            // Check if any waveform is infinite and throw an exception if it is
            // This is to prevent infinite waveforms from being used in a sequential waveform
            // as it would be impossible to determine the duration of the sequential waveform properly
            if(waveforms.Any(waveform => waveform.IsInfinite))
                throw new ArgumentException("Waveform cannot be infinite");
            
            // Set duration to sum of all waveforms durations
            Duration = 0;
            foreach (WaveformBase waveform in waveforms)
                Period += waveform.Duration;

            // Set looping mode
            Duration = loopWaveform ? -1 : Period;
        }
    }
}