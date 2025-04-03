using NOVA.Abstract;

namespace NOVA.Implementations.Modulated
{
    /// <summary>
    /// Simple noise waveform, changes value each millisecond
    /// </summary>
    /// <param name="amplitude">Amplitude of the waveform in [0, 1] range</param>
    /// <param name="offset">>Offset of the waveform in [0, 1] range</param>
    public sealed class NoiseWaveform(double amplitude, double offset = 0) : Waveform
    {
        private double _lastCalculatedTime;
        private double _lastCalculatedValue;
        
        /// <summary>
        /// Amplitude of the waveform in [0, 1] range
        /// </summary>
        public double Amplitude { get; private set; } = Math.Clamp(amplitude, 0, 1);
        
        /// <summary>
        /// Offset of the waveform in [0, 1] range
        /// </summary>
        public double Offset { get; private set; } = Math.Min(Math.Clamp(offset, 0, 1), 1 - Math.Clamp(amplitude, 0, 1));
        
        /// <summary>
        /// Sets the amplitude of the waveform
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
        /// <param name="offset">>Offset in [0, 1] range</param>
        public void SetOffset(double offset)
        {
            // Ensure offset is in [0, 1] range
            offset = Math.Min(Math.Clamp(offset, 0, 1), 1 - Math.Clamp(Amplitude, 0, 1));
            
            // Set offset
            Offset = offset;
        }
        
        public override double CalculateValueAt(double time)
        {
            // Check if time has changed by one millisecond, regenerate value if it has
            if (Math.Abs(time - _lastCalculatedTime) > 1e-6)
            {
                // Generate a new random value
                _lastCalculatedValue = Random.Shared.NextDouble() * Amplitude + Offset;
                _lastCalculatedTime = time;
            }
            
            // Return the last calculated value
            return _lastCalculatedValue;
        }
    }
}