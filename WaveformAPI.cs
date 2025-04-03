using System.Diagnostics;
using NOVA.Abstract;
using NOVA.Utility;

namespace NOVA
{
    /// <summary>
    /// This class is used as an entry point for all waveform-related operations
    /// </summary>
    public static class WaveformAPI
    {
        /// <summary>
        /// Resolution of the waveform in milliseconds
        /// </summary>
        public const int WAVEFORM_RESOLUTION = WaveformMath.WAVEFORM_RESOLUTION;
        
        /// <summary>
        /// List of all created waveforms, automatically managed by the API
        /// </summary>
        private static readonly List<Waveform> _waveforms = [];
        
        /// <summary>
        /// Registers the waveform in the API
        /// </summary>
        /// <param name="waveform">Waveform to register</param>
        internal static void RegisterWaveform(Waveform waveform)
        {
            _waveforms.Add(waveform);
        }
        
        /// <summary>
        /// Unregisters the waveform from the API
        /// </summary>
        /// <param name="waveform">Waveform to unregister</param>
        internal static void UnregisterWaveform(Waveform waveform)
        {
            _waveforms.Remove(waveform);
        }
        
        /// <summary>
        /// Starts the task that will update all waveforms
        /// Automatically called when the API is initialized (static constructor)
        /// </summary>
        private static void StartTask()
        {
            // Start the task that will update all waveforms
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        RunUpdate();
                        Thread.Sleep(WAVEFORM_RESOLUTION);
                    }
                    catch (Exception anyException)
                    {
                        // Log the exception to debug output and try to continue
                        Debug.Write(anyException);
                    }
                }
                
                // This is infinite loop, so we don't need to return anything
                // ReSharper disable once FunctionNeverReturns
            });
        }
        
        internal static void RunUpdate()
        {
            // Update all waveforms, waveform should check if they need to update
            // As we're using .NET8 we don't need to care for Linq performance issues that much
            foreach (Waveform waveform in _waveforms)
            {
                waveform.Update();
            }
        }
        
        static WaveformAPI()
        {
            // Start the task that will update all waveforms
            StartTask();
        }
    }
}