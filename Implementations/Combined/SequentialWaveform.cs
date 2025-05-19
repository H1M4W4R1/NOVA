using NOVA.Abstract;
using NOVA.Abstract.Interfaces;
using NOVA.Utility;

namespace NOVA.Implementations.Combined
{
    /// <summary>
    /// Represents a sequential waveform composed of multiple child waveforms played in sequence.
    /// Implements IPeriodicWaveform to support periodic waveform properties and behaviors.
    /// </summary>
    public sealed class SequentialWaveform : Waveform, IPeriodicWaveform
    {
        /// <summary>
        /// The array of Waveform objects that make up this sequential waveform.
        /// These waveforms will be played in the order they appear in the array.
        /// </summary>
        private readonly Waveform[] _waveforms;
        
        /// <summary>
        /// Gets the total duration of one complete cycle of the sequential waveform in milliseconds.
        /// This is the sum of all contained waveform durations.
        /// </summary>
        public double Period { get; init; }

        /// <summary>
        /// Gets the frequency of the waveform in Hz, calculated from the Period property.
        /// </summary>
        public double Frequency => WaveformMath.PeriodToFrequency(Period);

        /// <summary>
        /// Calculates the waveform's value at a specific point in time.
        /// </summary>
        /// <param name="time">The time in milliseconds at which to calculate the value.</param>
        /// <returns>The calculated waveform value at the specified time.</returns>
        /// <remarks>
        /// This method handles time wrapping for periodic waveforms and automatically
        /// selects the appropriate child waveform based on the elapsed time.
        /// </remarks>
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
        
        /// <summary>
        /// Initializes a new instance of the SequentialWaveform class.
        /// </summary>
        /// <param name="loopWaveform">Whether the waveform should loop indefinitely.</param>
        /// <param name="waveforms">The waveforms to combine sequentially.</param>
        /// <exception cref="ArgumentException">Thrown if any provided waveform is infinite.</exception>
        /// <remarks>
        /// The constructor calculates the total period as the sum of all child waveform durations.
        /// Infinite waveforms are not allowed as they would make the sequential waveform's duration undefined.
        /// </remarks>
        public SequentialWaveform(bool loopWaveform = false, params Waveform[] waveforms)
        {
            _waveforms = waveforms;
            
            // Check if any waveform is infinite and throw an exception if it is
            // This is to prevent infinite waveforms from being used in a sequential waveform
            // as it would be impossible to determine the duration of the sequential waveform properly
            if(waveforms.Any(waveform => waveform.IsInfinite))
                throw new ArgumentException("Waveform cannot be infinite");
            
            // Set duration to sum of all waveforms durations
            Duration = 0;
            foreach (Waveform waveform in waveforms)
                Period += waveform.Duration;

            // Set looping mode
            Duration = loopWaveform ? WaveformMath.LOOP_WAVEFORM : Period;
        }
    }
}