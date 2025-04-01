using NOVA.Abstract;

namespace NOVA.Implementations.Modulated
{
    public sealed class RectangularWaveform(double frequency, double amplitude = 1, double offset = 0, double fillFactor = 0.5)
        : FAOWaveform(frequency, amplitude, offset)
    {
        /// <summary>
        /// Fill factor of the waveform in [0, 1] range
        /// </summary>
        private double FillFactor { get; set; } = Math.Clamp(fillFactor, 0, 1);
        
        /// <summary>
        /// Sets the fill factor of the waveform
        /// </summary>
        /// <param name="fillFactor">Fill factor of the waveform in [0, 1] range</param>
        public void SetFillFactor(double fillFactor) => FillFactor = Math.Clamp(fillFactor, 0, 1);
        
        public override double CalculateValueAt(double time)
        {
            // Calculate period
            double period = 1000 / Frequency;
            
            // Calculate time in period
            double timeInPeriod = time % period;
            
            // If the waveform is in the second half of the period, invert the value
            if (timeInPeriod <= period * FillFactor)
                return Amplitude + Offset;

            return Offset;
        }
    }
}