using System;
using UnityEngine;

namespace SolarEngineMeditationSample.Wrappers.Max
{
    /// <summary>
    /// Max Native Ad wrapper for ad revenue event interception and SolarEngine tracking
    /// Follows the same pattern as other Max wrappers
    /// </summary>
    public static class MaxNativeAdWrapper
    {
        /// <summary>
        /// Build ad revenue event handler for native ads
        /// Intercepts OnAdRevenuePaidEvent and forwards to SolarEngine tracking
        /// </summary>
        /// <param name="userRevenueHandler">User's ad revenue event handler</param>
        /// <returns>A wrapped ad revenue event handler that intercepts and tracks before forwarding</returns>
        public static Action<string, MaxSdkBase.AdInfo> BuildAdRevenueHandler(
            Action<string, MaxSdkBase.AdInfo> userRevenueHandler = null)
        {
            Debug.Log("MaxNativeAdWrapper.BuildAdRevenueHandler() called");

            return (string adUnitId, MaxSdkBase.AdInfo adInfo) => {
                Debug.Log($"MaxNativeAdWrapper.AdRevenueEvent() called with adUnitId: {adUnitId}, data: {adInfo}");

                // Forward to tracker with enum and adInfo
                MaxSolarEngineTracker.trackAdImpression(MaxAdType.Native, adInfo);

                // Forward to user's handler if provided
                if (userRevenueHandler != null)
                {
                    userRevenueHandler(adUnitId, adInfo);
                }
            };
        }
    }
}


