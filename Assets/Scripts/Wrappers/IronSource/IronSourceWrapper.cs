using System;
using UnityEngine;
using Unity.Services.LevelPlay;

namespace SolarEngineMeditationSample.Wrappers.IronSource
{
    /// <summary>
    /// Unity IronSource wrapper for impression data event interception and SolarEngine tracking
    /// Follows the same pattern as iOS IronSourceWrapper
    /// Uses the new non-deprecated LevelPlay classes
    /// </summary>
    public static class IronSourceWrapper
    {
        /// <summary>
        /// Build impression data event handler that intercepts callbacks and forwards them to the user
        /// Uses the new LevelPlay.OnImpressionDataReady event
        /// </summary>
        /// <param name="userImpressionDataHandler">User's impression data event handler</param>
        /// <returns>A wrapped impression data event handler that intercepts and tracks before forwarding</returns>
        public static Action<LevelPlayImpressionData> BuildImpressionDataHandler(
            Action<LevelPlayImpressionData> userImpressionDataHandler = null)
        {
            Debug.Log("IronSourceWrapper.BuildImpressionDataHandler() called");

            return (LevelPlayImpressionData impressionData) => {
                Debug.Log($"IronSourceWrapper.ImpressionDataReadyEvent() called with data: {impressionData}");
                
                // Track ad impression for SolarEngine (forward raw data to tracker)
                IronSourceSolarEngineTracker.trackAdImpression(impressionData);
                
                // Forward to user's handler if provided
                if (userImpressionDataHandler != null)
                {
                    userImpressionDataHandler(impressionData);
                }
            };
        }


    }
}
