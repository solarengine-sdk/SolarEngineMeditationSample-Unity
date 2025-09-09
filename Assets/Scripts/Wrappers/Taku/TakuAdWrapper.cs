using System;
using UnityEngine;
using AnyThinkAds.Api;

namespace SolarEngineMeditationSample.Wrappers.Taku
{
    /// <summary>
    /// Consolidated Taku wrapper exposing only TrackAdRevenue per ad type.
    /// </summary>
    public static class TakuAdWrapper
    {
        public static void TrackRewardedAdRevenue(ATCallbackInfo callbackInfo)
        {
            Debug.Log("TakuAdWrapper.TrackRewardedAdRevenue()");
            TakuSolarEngineTracker.trackAdImpression(TakuAdType.RewardVideo, callbackInfo);
        }

        public static void TrackInterstitialAdRevenue(ATCallbackInfo callbackInfo)
        {
            Debug.Log("TakuAdWrapper.TrackInterstitialAdRevenue()");
            TakuSolarEngineTracker.trackAdImpression(TakuAdType.Interstitial, callbackInfo);
        }

        public static void TrackBannerAdRevenue(ATCallbackInfo callbackInfo)
        {
            Debug.Log("TakuAdWrapper.TrackBannerAdRevenue()");
            TakuSolarEngineTracker.trackAdImpression(TakuAdType.Banner, callbackInfo);
        }

        public static void TrackNativeAdRevenue(ATCallbackInfo callbackInfo)
        {
            Debug.Log("TakuAdWrapper.TrackNativeAdRevenue()");
            TakuSolarEngineTracker.trackAdImpression(TakuAdType.Native, callbackInfo);
        }

        public static void TrackSplashAdRevenue(ATCallbackInfo callbackInfo)
        {
            Debug.Log("TakuAdWrapper.TrackSplashAdRevenue()");
            TakuSolarEngineTracker.trackAdImpression(TakuAdType.Splash, callbackInfo);
        }
    }
}


