using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;

namespace SolarEngineMeditationSample.Wrappers.AdMob
{
    /// <summary>
    /// AdMob Ad Wrapper - Consolidated wrapper for all AdMob ad types
    /// Provides callback interception methods for paid events across all ad formats
    /// </summary>
    public static class AdMobAdWrapper
    {
        /// <summary>
        /// Build paid event handler for Rewarded Ads
        /// </summary>
        /// <param name="userCallback">The user's paid event callback that will receive the forwarded callbacks</param>
        /// <param name="responseInfo">Response info from the ad loading process</param>
        /// <returns>A wrapped paid event handler that intercepts and logs callbacks before forwarding them</returns>
        public static Action<AdValue> BuildRewardedAdPaidEventHandler(Action<AdValue> userCallback, ResponseInfo responseInfo = null)
        {
            Debug.Log("AdMobAdWrapper.BuildRewardedAdPaidEventHandler() called");

            return (AdValue adValue) => {
                Debug.Log($"AdMob Rewarded Ad onPaidEvent callback received - {adValue.Value} {adValue.CurrencyCode}");
                
                // Track ad impression for SolarEngine
                TrackAdImpression(AdMobAdType.RewardVideo, adValue, responseInfo);

                if (userCallback != null)
                {
                    userCallback(adValue);
                }
            };
        }

        /// <summary>
        /// Build paid event handler for Interstitial Ads
        /// </summary>
        /// <param name="userCallback">The user's paid event callback that will receive the forwarded callbacks</param>
        /// <param name="responseInfo">Response info from the ad loading process</param>
        /// <returns>A wrapped paid event handler that intercepts and logs callbacks before forwarding them</returns>
        public static Action<AdValue> BuildInterstitialAdPaidEventHandler(Action<AdValue> userCallback, ResponseInfo responseInfo = null)
        {
            Debug.Log("AdMobAdWrapper.BuildInterstitialAdPaidEventHandler() called");

            return (AdValue adValue) => {
                Debug.Log($"AdMob Interstitial Ad onPaidEvent callback received - {adValue.Value} {adValue.CurrencyCode}");
                
                // Track ad impression for SolarEngine
                TrackAdImpression(AdMobAdType.Interstitial, adValue, responseInfo);

                if (userCallback != null)
                {
                    userCallback(adValue);
                }
            };
        }

        /// <summary>
        /// Build paid event handler for Banner Ads
        /// </summary>
        /// <param name="userCallback">The user's paid event callback that will receive the forwarded callbacks</param>
        /// <param name="responseInfo">Response info from the ad loading process</param>
        /// <returns>A wrapped paid event handler that intercepts and logs callbacks before forwarding them</returns>
        public static Action<AdValue> BuildBannerAdPaidEventHandler(Action<AdValue> userCallback, ResponseInfo responseInfo = null)
        {
            Debug.Log("AdMobAdWrapper.BuildBannerAdPaidEventHandler() called");

            return (AdValue adValue) => {
                Debug.Log($"AdMob Banner Ad onPaidEvent callback received - {adValue.Value} {adValue.CurrencyCode}");
                
                // Track ad impression for SolarEngine
                TrackAdImpression(AdMobAdType.Banner, adValue, responseInfo);

                if (userCallback != null)
                {
                    userCallback(adValue);
                }
            };
        }

        /// <summary>
        /// Build paid event handler for Native Ads
        /// </summary>
        /// <param name="userCallback">The user's paid event callback that will receive the forwarded callbacks</param>
        /// <param name="responseInfo">Response info from the ad loading process</param>
        /// <returns>A wrapped paid event handler that intercepts and logs callbacks before forwarding them</returns>
        public static Action<AdValue> BuildNativeAdPaidEventHandler(Action<AdValue> userCallback, ResponseInfo responseInfo = null)
        {
            Debug.Log("AdMobAdWrapper.BuildNativeAdPaidEventHandler() called");

            return (AdValue adValue) => {
                Debug.Log($"AdMob Native Ad onPaidEvent callback received - {adValue.Value} {adValue.CurrencyCode}");
                
                // Track ad impression for SolarEngine
                TrackAdImpression(AdMobAdType.Native, adValue, responseInfo);

                if (userCallback != null)
                {
                    userCallback(adValue);
                }
            };
        }

        /// <summary>
        /// Build paid event handler for App Open Ads
        /// </summary>
        /// <param name="userCallback">The user's paid event callback that will receive the forwarded callbacks</param>
        /// <param name="responseInfo">Response info from the ad loading process</param>
        /// <returns>A wrapped paid event handler that intercepts and logs callbacks before forwarding them</returns>
        public static Action<AdValue> BuildAppOpenAdPaidEventHandler(Action<AdValue> userCallback, ResponseInfo responseInfo = null)
        {
            Debug.Log("AdMobAdWrapper.BuildAppOpenAdPaidEventHandler() called");

            return (AdValue adValue) => {
                Debug.Log($"AdMob App Open Ad onPaidEvent callback received - {adValue.Value} {adValue.CurrencyCode}");
                
                // Track ad impression for SolarEngine (map AppOpen to Splash)
                TrackAdImpression(AdMobAdType.Splash, adValue, responseInfo);

                if (userCallback != null)
                {
                    userCallback(adValue);
                }
            };
        }

        /// <summary>
        /// Track ad impression for SolarEngine with ad type and revenue data
        /// </summary>
        /// <param name="adType">Type of ad (rewarded, interstitial, banner, native, app_open)</param>
        /// <param name="adValue">Ad value containing revenue information</param>
        /// <param name="responseInfo">Response info from the ad loading process</param>
        private static void TrackAdImpression(AdMobAdType adType, AdValue adValue, ResponseInfo responseInfo)
        {
            AdMobSolarEngineTracker.trackAdImpression(adType, adValue, responseInfo);
        }
    }
}
