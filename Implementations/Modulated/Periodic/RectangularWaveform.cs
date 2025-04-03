using NOVA.Abstract;
using NOVA.Utility;

namespace NOVA.Implementations.Modulated.Periodic
{
    public sealed class RectangularWaveform(double frequency,
        double amplitude = WaveformMath.WAVEFORM_MAXIMUM_VALUE,
        double offset = WaveformMath.WAVEFORM_MINIMUM_VALUE,
        double fillFactor = 0.5
    )
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
            double period = WaveformMath.FrequencyToPeriod(Frequency);

            // Calculate time in period
            double timeInPeriod = WaveformMath.TimeInCycle(time, period);

            // If the waveform is in the second half of the period, invert the value
            if (timeInPeriod <= period * FillFactor) return Amplitude + Offset;

            return Offset;
        }
    }
}