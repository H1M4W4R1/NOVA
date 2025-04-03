using NOVA.Abstract;
using NOVA.Abstract.Interfaces;

namespace NOVA.Implementations.Modulated
{
    /// <summary>
    /// Waveform that ramps up from one value to another over a specified duration
    /// </summary>
    public sealed class RampWaveform : Waveform, IStaticDurationWaveform
    {
        /// <summary>
        /// Waveform that ramps up from one value to another over a specified duration
        /// </summary>
        /// <param name="from">Start value of the waveform in [0, 1] range</param>
        /// <param name="to">End value of the waveform in [0, 1] range</param>
        /// <param name="duration">Duration of the waveform in ms</param>
        public RampWaveform(double from, double to, double duration)
        {
            StartValue = Math.Clamp(from, 0, 1);
            EndValue = Math.Clamp(to, 0, 1);
            Duration = duration;
        }

        /// <summary>
        /// Start value of the waveform
        /// </summary>
        private double StartValue { get; set; }
        
        /// <summary>
        /// End value of the waveform
        /// </summary>
        private double EndValue { get; set; }
        
        /// <summary>
        /// Sets the start value of the waveform
        /// </summary>
        /// <param name="value">Start value of the waveform in [0, 1] range</param>
        public void SetStartValue(double value) => StartValue = Math.Clamp(value, 0, 1);
        
        /// <summary>
        /// Sets the end value of the waveform
        /// </summary>
        /// <param name="value">End value of the waveform in [0, 1] range</param>
        public void SetEndValue(double value) => EndValue = Math.Clamp(value, 0, 1);
        
        public override double CalculateValueAt(double time)
            => StartValue + (EndValue - StartValue) * Math.Clamp(time / Duration, 0, 1); // We clamp duration to [0, 1] range to prevent the value from going below 0 or above 1
    }
}