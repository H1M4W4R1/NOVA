using System.Runtime.CompilerServices;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using NOVA.Abstract;
using NOVA.Utility;

namespace NOVA.Implementations.Modulated.Randomized
{
    /// <summary>
    ///     Implements a pseudo-randomized waveform inspired by octopus tentacle movement
    /// </summary>
    public sealed class TentacleWaveform : Waveform
    {
        private const int TIME_SEGMENTS = 15; // Also known as duration in seconds
        private const int TOTAL_SEGMENTS = 50 * (TIME_SEGMENTS / 15);
        private const int WINDOW_SAMPLES = 20;
        private const int TIME_SAMPLES = TIME_SEGMENTS * WINDOW_SAMPLES;

        /// <summary>
        ///     Gets the maximum amplitude of the waveform.
        /// </summary>
        /// <value>Normalized maximum amplitude in range [0, 1]</value>
        public double Amplitude { get; private set; }

        /// <summary>
        ///     Gets or sets the current DC offset of the waveform.
        ///     Value is automatically clamped to ensure offset + amplitude ≤ 1.
        /// </summary>
        /// <value>Normalized offset value in range [0, 1]</value>
        private double Offset { get; set; }

        public float Frequency { get; set; } = 1;
        public float SpatialWaveNumber { get; set; } = 3;
        public float TaperExponent { get; set; } = 2;
        public float BaseAmplitude { get; set; } = 2f;
        public float TipAmplitude { get; set; } = 2f;
        public float PulseAmplitude { get; set; } = 0.9f;
        public float PulseSpeed { get; set; } = 1.5f;
        public float InitialPulsePhase { get; set; } = 0f;
        public float PulseWidth { get; set; } = 0.05f;
        public float PulseTemporalDecay { get; set; } = 1.0f;
        public float PulseTipGrowthExponent { get; set; } = 2f;

        private float MValue => TipAmplitude + PulseAmplitude;

        private double[] Waveform { get; init; }

        public TentacleWaveform(
            double amplitude = WaveformMath.WAVEFORM_MAXIMUM_VALUE,
            double offset = WaveformMath.WAVEFORM_MINIMUM_VALUE)
        {
            Amplitude = WaveformMath.ClampAmplitude(amplitude);
            Offset = WaveformMath.ClampOffset(offset, amplitude);

            double[] s = Generate.LinearSpaced(TOTAL_SEGMENTS, 0, 1);
            double[] t = Generate.LinearSpaced(TIME_SAMPLES, 0, TIME_SEGMENTS);

            // --- simulate tentacle motion matrix N(s,t) ---
            double[,] nArray = new double[TOTAL_SEGMENTS, TIME_SAMPLES];
            for (int ti = 0; ti < TIME_SAMPLES; ti++)
            {
                double tt = t[ti];
                for (int si = 0; si < TOTAL_SEGMENTS; si++)
                {
                    double ss = s[si];
                    double a = BaseAmplitude + (TipAmplitude - BaseAmplitude) * Math.Pow(ss, TaperExponent);
                    double wave0 = a * Math.Sin(2 * Math.PI * (Frequency * tt - SpatialWaveNumber * ss));
                    double pulse =
                        PulseAmplitude * Math.Exp(-Math.Pow(ss - (InitialPulsePhase + PulseSpeed * tt), 2) /
                                                  (2 * PulseWidth * PulseWidth))
                                       * Math.Exp(-PulseTemporalDecay * tt)
                                       * Math.Pow(ss, PulseTipGrowthExponent);
                    double raw = wave0 + pulse;
                    nArray[si, ti] = Math.Min(1.0, Math.Max(0.0, (raw / MValue + 1.0) / 2.0));
                }
            }

            // --- compress tentacle matrix to one average signal ---
            double[] signal = new double[TIME_SAMPLES];
            for (int ti = 0; ti < TIME_SAMPLES; ti++)
            {
                double sum = 0.0;
                for (int si = 0; si < TOTAL_SEGMENTS; si++) sum += nArray[si, ti];
                signal[ti] = sum / TOTAL_SEGMENTS;
            }

            double mean = signal.Average();
            for (int i = 0; i < TIME_SAMPLES; i++) signal[i] -= mean;

            // --- FFT and frequency mapping ---
            Complex32[] y = signal.Select(v => new Complex32((float) v, 0)).ToArray();
            Fourier.Forward(y, FourierOptions.Matlab);
            const int L2 = TIME_SAMPLES / 2;
            float[] mag = new float[L2];
            for (int i = 0; i < L2; i++) mag[i] = y[i].Magnitude;

            int step = Math.Max(1, L2 / TOTAL_SEGMENTS);
            int[] bins = Enumerable.Range(1, TOTAL_SEGMENTS).Select(i => Math.Min(i * step, L2 - 1)).ToArray();

            // --- build complex frequency spectrum for IFFT ---
            Complex32[] spec = new Complex32[TIME_SAMPLES];
            Random rnd = new Random();
            for (int i = 0; i < TOTAL_SEGMENTS; i++)
            {
                int b = bins[i];
                float amp = mag[b];
                float phase = (float) (rnd.NextDouble() * 2 * Math.PI);
                Complex32 val = Complex32.FromPolarCoordinates(amp, phase);
                spec[b] = val;
                if (TIME_SAMPLES - b + 1 < TIME_SAMPLES) spec[TIME_SAMPLES - b + 1] = val.Conjugate();
            }

            // --- inverse FFT to reconstruct waveform ---
            Fourier.Inverse(spec, FourierOptions.Matlab);
            double[] wave = spec.Select(c => (double) c.Real).ToArray();

            double min = wave.Min();
            double max = wave.Max();
            for (int i = 0; i < wave.Length; i++) wave[i] = (wave[i] - min) / (max - min);

            Waveform = wave;
        }

        public override double[] CalculateValuesAt(double time)
        {
            // Get waveform index based on time
            double cTime = time % TIME_SEGMENTS;
            int index = (int) (cTime / TIME_SEGMENTS * TIME_SAMPLES);

            // Convert amplitude from normalized to local values and return
            CurrentValues[0] = Offset + Amplitude * Waveform[index];
            return CurrentValues;
        }
        
        /// <summary>
        ///     Sets a new maximum amplitude for the waveform.
        /// </summary>
        /// <param name="maxAmplitude">New maximum amplitude in [0, 1] range</param>
        /// <remarks>
        ///     Automatically clamps the input value and adjusts offset if necessary.
        ///     Uses aggressive inlining for performance optimization.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetAmplitude(double maxAmplitude)
        {
            Amplitude = WaveformMath.ClampAmplitude(maxAmplitude);
            Offset = WaveformMath.ClampOffset(Offset, Amplitude);
        }

        /// <summary>
        ///     Sets a new DC offset for the waveform.
        /// </summary>
        /// <param name="offset">New offset value in [0, 1] range</param>
        /// <remarks>
        ///     Automatically clamps the input value to ensure offset + amplitude ≤ 1.
        ///     Uses aggressive inlining for performance optimization.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetOffset(double offset) => Offset = WaveformMath.ClampOffset(offset, Amplitude);

    }
}