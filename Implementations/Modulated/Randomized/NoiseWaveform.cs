using NOVA.Abstract;
using NOVA.Utility;

namespace NOVA.Implementations.Modulated.Randomized
{
    /// <summary>
    /// Simple noise waveform, changes value each millisecond (or tick of the WaveformAPI, whichever
    /// takes longer; usually the tick of the WaveformAPI)
    /// </summary>
    /// <param name="amplitude">Amplitude of the waveform in [0, 1] range</param>
    /// <param name="offset">Offset of the waveform in [0, 1] range</param>
    public sealed class NoiseWaveform(double amplitude, double offset = 0) : Waveform
    {
        private double _lastCalculatedTime;
        private double _lastCalculatedValue;
        
        /// <summary>
        /// Amplitude of the waveform in [0, 1] range
        /// </summary>
        public double Amplitude { get; private set; } = WaveformMath.ClampAmplitude(amplitude);
        
        /// <summary>
        /// Offset of the waveform in [0, 1] range
        /// </summary>
        public double Offset { get; private set; } = WaveformMath.ClampOffset(offset, amplitude);
        
        /// <summary>
        /// Sets the amplitude of the waveform
        /// </summary>
        /// <param name="amplitude">Amplitude in [0, 1] range</param>
        public void SetAmplitude(double amplitude)
        {
            // Ensure amplitude and offset is in [0, 1] range
            // and their sum is less or equal to 1
            Amplitude = WaveformMath.ClampAmplitude(amplitude);
            Offset = WaveformMath.ClampOffset(Offset, Amplitude);
        }
        
        /// <summary>
        /// Sets the offset of the waveform
        /// </summary>
        /// <param name="offset">Offset in [0, 1] range</param>
        public void SetOffset(double offset) => Offset = WaveformMath.ClampOffset(offset, Amplitude);
        
        public override double CalculateValueAt(double time)
        {
            // Check if time has changed by one millisecond, regenerate value if it has
            if (Math.Abs(time - _lastCalculatedTime) > WaveformMath.MINIMUM_FREQUENCY)
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