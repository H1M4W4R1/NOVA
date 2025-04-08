using NOVA.Abstract;
using NOVA.Utility;

namespace NOVA.Implementations.Modulated.Periodic.Mathematical
{
    /// <summary>
    /// Simple wave waveform
    /// </summary>
    /// <param name="frequency">Frequency of the wave in Hz</param>
    /// <param name="amplitude">Amplitude of the wave</param>
    /// <param name="offset">Offset of the wave, less than 1 - amplitude, automatically ensured</param>
    /// <param name="smoothingFactor">Smoothing factor of the wave</param>
    /// <remarks>
    /// Equation: <br/>
    /// a\left(1-\left|\sin\left(2\pi x\cdot\frac{f_{r}}{1000}-\frac{\pi}{2}\right)\right|^{n}\right)+o
    /// </remarks>
    public sealed class WaveWaveform(double frequency, double amplitude = 1, double offset = 0,
        double smoothingFactor = 5)
        : FAOWaveform (frequency, amplitude, offset)
    {
        /// <summary>
        /// Smoothing factor of the wave
        /// </summary>
        /// <remarks>
        /// If one then peak will be a simple triangle wave. If value is greater than one it will be smoothed to
        /// increase peak width (convex), otherwise it will be sharpened to decrease peak width (concave).
        /// </remarks>
        public double SmoothingFactor { get; set; } = smoothingFactor;
        
        public override double CalculateValueAt(double time)
        {
            // Internal sine calculation
            // Function is periodic with default period of 1 second (when frequency = 1)
            // Shifted by half cycle to ensure the first value is minimum
            double sinInternal = 2 * Math.PI * time * (Frequency / WaveformMath.ONE_SECOND) - Math.PI / 2;
            
            // Calculate the sine value
            double sineValue = Math.Sin(sinInternal);
            
            // Calculate waveform
            return Amplitude * (1 - Math.Pow(Math.Abs(sineValue), SmoothingFactor)) + Offset;
        }
    }
}