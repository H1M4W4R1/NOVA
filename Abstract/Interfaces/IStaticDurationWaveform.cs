namespace NOVA.Abstract.Interfaces
{
    /// <summary>
    /// Interface representing a waveform with a fixed, unchangeable duration.
    /// 
    /// <para>
    /// This interface serves as a marker for waveforms whose duration is determined
    /// at creation and remains constant throughout its lifetime. The duration cannot
    /// be modified after instantiation.
    /// </para>
    /// 
    /// <para>
    /// Implementation Note:
    /// The actual duration handling is managed by the <see cref="Waveform"/> base class.
    /// Classes implementing this interface should not attempt to modify the duration.
    /// </para>
    /// 
    /// <remarks>
    /// This is typically used for waveforms where the duration is an intrinsic property
    /// of the waveform type itself, such as pre-recorded audio clips or fixed-length
    /// signal patterns.
    /// </remarks>
    /// </summary>
    public interface IStaticDurationWaveform
    {
        
    }
}