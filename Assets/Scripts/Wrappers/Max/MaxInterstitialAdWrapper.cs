using System;
using UnityEngine;

namespace SolarEngineMeditationSample.Wrappers.Max
{
    /// <summary>
    /// Max Interstitial Ad wrapper for ad revenue event interception and SolarEngine tracking
    /// Follows the same pattern as iOS MaxWrapperInterstitialAdListener
    /// </summary>
    public static class MaxInterstitialAdWrapper
    {
        /// <summary>
        /// Build ad revenue event handler for interstitial ads
        /// Intercepts OnAdRevenuePaidEvent and forwards to SolarEngine tracking
        /// </summary>
        /// <param name="userRevenueHandler">User's ad revenue event handler</param>
        /// <returns>A wrapped ad revenue event handler that intercepts and tracks before forwarding</returns>
        public static Action<string, MaxSdkBase.AdInfo> BuildAdRevenueHandler(
            Action<string, MaxSdkBase.AdInfo> userRevenueHandler = null)
        {
            Debug.Log("MaxInterstitialAdWrapper.BuildAdRevenueHandler() called");

            return (string adUnitId, MaxSdkBase.AdInfo adInfo) => {
                Debug.Log($"MaxInterstitialAdWrapper.AdRevenueEvent() called with adUnitId: {adUnitId}, data: {adInfo}");
                
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
            Debug.Log($"MaxInterstitialAdWrapper: Tracking interstitial ad revenue for SolarEngine");
            // Forward to tracker with enum and adInfo
            MaxSolarEngineTracker.trackAdImpression(MaxAdType.Interstitial, adInfo);
        }
    }
}
