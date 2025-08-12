namespace NOVA
{
    /// <summary>
    ///     Represents a method that will handle waveform start events.
    ///     This delegate is invoked when a waveform begins its execution.
    /// </summary>
    public delegate void WaveformStartHandler();

    /// <summary>
    ///     Represents a method that will handle waveform value change events.
    /// </summary>
    /// <param name="value">The current value of the waveform, in the range [0, 1].</param>
    /// <remarks>
    ///     This delegate is invoked whenever a waveform's output value changes significantly.
    ///     The exact frequency of invocation depends on the waveform implementation.
    /// </remarks>
    public delegate void WaveformValuesChanged(Span<double> values);
    
    /// <summary>
    ///     Represents a method that will handle waveform value change events.
    /// </summary>
    /// <param name="value">The current value of the waveform, in the range [0, 1].</param>
    /// <remarks>
    ///     This delegate is invoked whenever a waveform's output value changes significantly.
    ///     The exact frequency of invocation depends on the waveform implementation.
    /// </remarks>
    public delegate void WaveformValueChangedHandler(double value);

    /// <summary>
    ///     Represents a method that will handle waveform end events.
    ///     This delegate is invoked when a waveform completes its execution cycle.
    /// </summary>
    /// <remarks>
    ///     For periodic waveforms, this is called at the end of each period.
    ///     For one-shot waveforms, this is called when the waveform reaches its final state.
    /// </remarks>
    public delegate void WaveformEndHandler();
}