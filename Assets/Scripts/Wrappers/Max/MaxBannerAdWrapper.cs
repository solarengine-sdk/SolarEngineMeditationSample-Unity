using System;
using UnityEngine;

namespace SolarEngineMeditationSample.Wrappers.Max
{
    /// <summary>
    /// Max Banner Ad wrapper for ad revenue event interception and SolarEngine tracking
    /// Follows the same pattern as iOS MaxWrapperBannerAdViewListener
    /// </summary>
    public static class MaxBannerAdWrapper
    {
        /// <summary>
        /// Build ad revenue event handler for banner ads
        /// Intercepts OnAdRevenuePaidEvent and forwards to SolarEngine tracking
        /// </summary>
        /// <param name="userRevenueHandler">User's ad revenue event handler</param>
        /// <returns>A wrapped ad revenue event handler that intercepts and tracks before forwarding</returns>
        public static Action<string, MaxSdkBase.AdInfo> BuildAdRevenueHandler(
            Action<string, MaxSdkBase.AdInfo> userRevenueHandler = null)
        {
            Debug.Log("MaxBannerAdWrapper.BuildAdRevenueHandler() called");

            return (string adUnitId, MaxSdkBase.AdInfo adInfo) => {
                Debug.Log($"MaxBannerAdWrapper.AdRevenueEvent() called with adUnitId: {adUnitId}, data: {adInfo}");
                
                // Track ad revenue for SolarEngine
                TrackAdRevenue(adInfo);
                
                // Forward to user's handler if provided
                if (userRevenueHandler != null)
                {
                    userRevenueHandler(adUnitId, adInfo);
                }
            };
        }

        /// <summary>
        /// Track ad revenue for SolarEngine
        /// </summary>
        /// <param name="adInfo">Ad info from Max SDK</param>
        private static void TrackAdRevenue(MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"MaxBannerAdWrapper: Tracking banner ad revenue for SolarEngine");        
            // Forward to tracker with enum and adInfo
            MaxSolarEngineTracker.trackAdImpression(MaxAdType.Banner, adInfo);
        }
    }
}
