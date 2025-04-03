using NOVA.Abstract;
using NOVA.Abstract.Interfaces;

namespace NOVA.Implementations.Modulated.Periodic
{
    /// <summary>
    /// Rectangular waveform with a trapezoidal shape <br/>
    /// <ul>
    /// <li>Minimum value is offset</li>
    /// <li>Maximum value is offset + amplitude</li>
    /// </ul>
    /// </summary>
    /// <param name="rampUpTime">Time in ms to ramp up from minimum to maximum value</param>
    /// <param name="keepMaxTime">>Time in ms to keep maximum value</param>
    /// <param name="rampDownTime">>Time in ms to ramp down from maximum to minimum value</param>
    /// <param name="keepMinTime">>Time in ms to keep minimum value</param>
    /// <param name="amplitude">>Amplitude of the waveform in [0, 1] range</param>
    /// <param name="offset">>Offset of the waveform in [0, 1] range</param>
    /// <remarks>
    /// The sum of amplitude and offset must be less than or equal to 1. 
    /// </remarks>
    public sealed class TrapezoidalWaveform(double rampUpTime, double keepMaxTime, double rampDownTime, double keepMinTime, 
        double amplitude = 1, double offset = 0) : Waveform, IPeriodicWaveform
    {
        /// <summary>
        /// Period of the waveform in ms
        /// </summary>
        public double Period => rampUpTime + keepMaxTime + rampDownTime + keepMinTime;
        
        /// <summary>
        /// Frequency of the waveform in Hz
        /// </summary>
        public double Frequency => 1000 / Period;
        
        /// <summary>
        /// Amplitude of the waveform in [0, 1] range
        /// </summary>
        public double Amplitude { get; private set; } = Math.Clamp(amplitude, 0, 1);
        
        /// <summary>
        /// Offset of the waveform in [0, 1] range
        /// </summary>
        public double Offset { get; private set; } = Math.Min(Math.Clamp(offset, 0, 1), 1 - Math.Clamp(amplitude, 0, 1));
        
        /// <summary>
        /// Sets the frequency of the waveform
        /// </summary>
        /// <param name="amplitude">Amplitude in [0, 1] range</param>
        public void SetAmplitude(double amplitude)
        {
            // Ensure amplitude is in [0, 1] range
            amplitude = Math.Clamp(amplitude, 0, 1);

            // Ensure amplitude + offset is in [0, 1] range
            if (amplitude + Offset > 1) Offset = 1 - amplitude;

            // Set amplitude
            Amplitude = amplitude;
        }
        
        /// <summary>
        /// Sets the offset of the waveform
        /// </summary>
        /// <param name="offset">>Offset of the waveform in [0, 1] range</param>
        public void SetOffset(double offset)
        {
            // Ensure offset is in [0, 1] range
            offset = Math.Min(Math.Clamp(offset, 0, 1), 1 - Math.Clamp(Amplitude, 0, 1));

            // Set offset
            Offset = offset;
        }
        
        public override double CalculateValueAt(double time)
        {
            // Calculate time in period
            double timeInPeriod = time % Period;

            // Calculate value based on time in period
            if (timeInPeriod < rampUpTime) // Ramp-up, triangle
                return Offset + Amplitude * (timeInPeriod / rampUpTime);
            if (timeInPeriod < rampUpTime + keepMaxTime) // Keep max, constant value
                return Offset + Amplitude;
            if (timeInPeriod < rampUpTime + keepMaxTime + rampDownTime) // Ramp-down, inverted triangle
                return Offset + Amplitude * (1 - (timeInPeriod - rampUpTime - keepMaxTime) / rampDownTime);
            return Offset; // Keep min, constant value
        }
    }
}