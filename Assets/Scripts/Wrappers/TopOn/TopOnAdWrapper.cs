using System;
using UnityEngine;
using AnyThinkAds.Api;

namespace SolarEngineMeditationSample.Wrappers.TopOn
{
    /// <summary>
    /// Consolidated TopOn wrapper exposing only TrackAdRevenue per ad type.
    /// </summary>
    public static class TopOnAdWrapper
    {
        public static void TrackRewardedAdRevenue(ATCallbackInfo callbackInfo)
        {
            Debug.Log("TopOnAdWrapper.TrackRewardedAdRevenue()");
            TopOnSolarEngineTracker.trackAdImpression(TopOnAdType.RewardVideo, callbackInfo);
        }

        public static void TrackInterstitialAdRevenue(ATCallbackInfo callbackInfo)
        {
            Debug.Log("TopOnAdWrapper.TrackInterstitialAdRevenue()");
            TopOnSolarEngineTracker.trackAdImpression(TopOnAdType.Interstitial, callbackInfo);
        }

        public static void TrackBannerAdRevenue(ATCallbackInfo callbackInfo)
        {
            Debug.Log("TopOnAdWrapper.TrackBannerAdRevenue()");
            TopOnSolarEngineTracker.trackAdImpression(TopOnAdType.Banner, callbackInfo);
        }

        public static void TrackNativeAdRevenue(ATCallbackInfo callbackInfo)
        {
            Debug.Log("TopOnAdWrapper.TrackNativeAdRevenue()");
            TopOnSolarEngineTracker.trackAdImpression(TopOnAdType.Native, callbackInfo);
        }

        public static void TrackSplashAdRevenue(ATCallbackInfo callbackInfo)
        {
            Debug.Log("TopOnAdWrapper.TrackSplashAdRevenue()");
            TopOnSolarEngineTracker.trackAdImpression(TopOnAdType.Splash, callbackInfo);
        }
    }
}


