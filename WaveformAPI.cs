using System.Diagnostics;
using NOVA.Abstract;
using NOVA.Utility;

namespace NOVA
{
    /// <summary>
    ///     Provides the central management system for all waveform operations in the NOVA framework.
    ///     This static API handles waveform registration, lifecycle management, and periodic updates.
    /// </summary>
    /// <remarks>
    ///     The WaveformAPI automatically initializes upon first access and maintains an internal
    ///     collection of all active waveforms. It runs a background task that updates waveforms
    ///     at a fixed interval defined by WAVEFORM_RESOLUTION.
    /// </remarks>
    public static class WaveformAPI
    {
        /// <summary>
        ///     Gets the update resolution for all waveforms in milliseconds.
        ///     This determines how frequently waveform values are recalculated.
        /// </summary>
        /// <value>
        ///     The minimum time interval between waveform updates, in milliseconds.
        ///     This value is synchronized with WaveformMath.WAVEFORM_RESOLUTION.
        /// </value>
        public const int WAVEFORM_RESOLUTION = WaveformMath.WAVEFORM_RESOLUTION;

        /// <summary>
        ///     Contains all active waveforms registered with the API.
        ///     This collection is thread-safe for enumeration during updates.
        /// </summary>
        private static readonly List<Waveform> _waveforms = [];

        /// <summary>
        ///     Registers a waveform instance with the API to receive periodic updates.
        /// </summary>
        /// <param name="waveform">The waveform instance to register. Must not be null.</param>
        /// <remarks>
        ///     Registered waveforms will begin receiving Update() calls at the resolution
        ///     specified by WAVEFORM_RESOLUTION. This method is typically called automatically
        ///     when a waveform is instantiated.
        /// </remarks>
        internal static void RegisterWaveform(Waveform waveform)
        {
            _waveforms.Add(waveform);
        }

        /// <summary>
        ///     Removes a waveform from the API's update cycle.
        /// </summary>
        /// <param name="waveform">The waveform instance to unregister.</param>
        /// <remarks>
        ///     After unregistration, the waveform will no longer receive Update() calls.
        ///     This method is typically called automatically when a waveform is disposed.
        /// </remarks>
        internal static void UnregisterWaveform(Waveform waveform)
        {
            _waveforms.Remove(waveform);
        }

        /// <summary>
        ///     Initializes and starts the background task responsible for updating all waveforms.
        /// </summary>
        /// <remarks>
        ///     This method creates a long-running background task that:
        ///     1. Executes at intervals defined by WAVEFORM_RESOLUTION
        ///     2. Safely handles exceptions to prevent task termination
        ///     3. Runs indefinitely while the application is active
        /// </remarks>
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

        /// <summary>
        ///     Executes an update cycle for all registered waveforms.
        /// </summary>
        /// <remarks>
        ///     This method iterates through all registered waveforms and invokes their Update() method.
        ///     The update frequency is controlled by the calling task's sleep interval.
        ///     Individual waveform implementations determine whether they need to recalculate values
        ///     during each update cycle.
        /// </remarks>
        internal static void RunUpdate()
        {
            // Update all waveforms, waveform should check if they need to update
            // As we're using .NET8 we don't need to care for Linq performance issues that much
            foreach (Waveform waveform in _waveforms)
            {
                waveform.Update();
            }
        }

        /// <summary>
        ///     Static constructor that initializes the waveform update system.
        /// </summary>
        /// <remarks>
        ///     This constructor is automatically called when the WaveformAPI class is first accessed.
        ///     It ensures the background update task is running before any waveforms can be registered.
        /// </remarks>
        static WaveformAPI()
        {
            // Start the task that will update all waveforms
            StartTask();
        }
    }
}