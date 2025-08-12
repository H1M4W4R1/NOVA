using NOVA.Abstract;
using NOVA.Utility;

namespace NOVA.Implementations.Modulated.Periodic.Mathematical
{
    /// <summary>
    ///     Represents a wave waveform with configurable smoothing characteristics.
    ///     This waveform produces a periodic signal that can range from sharp peaks to smooth curves
    ///     based on the smoothing factor parameter.
    /// </summary>
    /// <param name="frequency">
    ///     The frequency of the waveform in Hertz (Hz). Determines how many complete cycles occur per
    ///     second.
    /// </param>
    /// <param name="amplitude">
    ///     The peak amplitude of the waveform, normalized to range [0, 1]. Represents the maximum
    ///     deviation from the offset.
    /// </param>
    /// <param name="offset">
    ///     The vertical offset of the waveform, normalized to range [0, 1]. Automatically clamped to prevent
    ///     amplitude overflow.
    /// </param>
    /// <param name="smoothingFactor">The exponent applied to the absolute sine value to control waveform shape. Default is 5.</param>
    /// <remarks>
    ///     <para>
    ///         The waveform follows the mathematical equation:
    ///         <br />
    ///         a(1 - |sin(2πx·(f/1000) - π/2)|ⁿ) + o
    ///         <br />
    ///         Where:
    ///         <list type="bullet">
    ///             <item>a = amplitude</item>
    ///             <item>f = frequency</item>
    ///             <item>n = smoothingFactor</item>
    ///             <item>o = offset</item>
    ///             <item>x = time in milliseconds</item>
    ///         </list>
    ///     </para>
    ///     <para>
    ///         The waveform is phase-shifted by π/2 to ensure the initial value starts at the minimum point.
    ///     </para>
    /// </remarks>
    public sealed class WaveWaveform(double frequency, double amplitude = 1, double offset = 0,
        double smoothingFactor = 5)
        : FAOWaveform (frequency, amplitude, offset)
    {
        /// <summary>
        ///     Gets or sets the smoothing factor that controls the waveform's peak characteristics.
        /// </summary>
        /// <value>
        ///     A positive number that determines the shape of the waveform peaks:
        ///     <list type="bullet">
        ///         <item>1.0: Produces triangular peaks (linear interpolation between minimum and maximum)</item>
        ///         <item>Greater than 1: Creates rounded, convex peaks with wider tops</item>
        ///         <item>Less than 1: Creates sharp, concave peaks with narrow tops</item>
        ///     </list>
        /// </value>
        public double SmoothingFactor { get; set; } = smoothingFactor;

        /// <summary>
        ///     Calculates the waveform's value at a specific point in time.
        /// </summary>
        /// <param name="time">The time in milliseconds since waveform start.</param>
        /// <returns>
        ///     The computed waveform value at the specified time, combining:
        ///     <list type="bullet">
        ///         <item>A base sine wave shifted to start at minimum value</item>
        ///         <item>Absolute value transformation</item>
        ///         <item>Smoothing factor exponentiation</item>
        ///         <item>Amplitude scaling and offset application</item>
        ///     </list>
        /// </returns>
        public override double[] CalculateValuesAt(double time)
        {
            // Internal sine calculation
            // Function is periodic with default period of 1 second (when frequency = 1)
            // Shifted by half cycle to ensure the first value is minimum
            double sinInternal = 2 * Math.PI * time * (Frequency / WaveformMath.ONE_SECOND) - Math.PI / 2;
            
            // Calculate the sine value
            double sineValue = Math.Sin(sinInternal);
            
            // Calculate waveform
            CurrentValues[0] = Amplitude * (1 - Math.Pow(Math.Abs(sineValue), SmoothingFactor)) + Offset;
            return CurrentValues;
        }
    }
}