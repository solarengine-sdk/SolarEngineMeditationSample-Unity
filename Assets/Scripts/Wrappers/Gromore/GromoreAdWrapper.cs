using System;
using UnityEngine;
using ByteDance.Union.Mediation;

namespace SolarEngineMeditationSample.Wrappers.Gromore
{
    /// <summary>
    /// Consolidated Gromore wrapper exposing only TrackAdRevenue per ad type.
    /// </summary>
    public static class GromoreAdWrapper
    {
        public static void TrackRewardedAdRevenue(string placementId, MediationAdEcpmInfo ecpmInfo)
        {
            Debug.Log("GromoreAdWrapper.TrackRewardedAdRevenue()");
            GromoreSolarEngineTracker.trackAdImpression(GromoreAdType.RewardVideo, placementId, ecpmInfo);
        }

        public static void TrackInterstitialAdRevenue(string placementId, MediationAdEcpmInfo ecpmInfo)
        {
            Debug.Log("GromoreAdWrapper.TrackInterstitialAdRevenue()");
            GromoreSolarEngineTracker.trackAdImpression(GromoreAdType.Interstitial, placementId, ecpmInfo);
        }

        public static void TrackBannerAdRevenue(string placementId, MediationAdEcpmInfo ecpmInfo)
        {
            Debug.Log("GromoreAdWrapper.TrackBannerAdRevenue()");
            GromoreSolarEngineTracker.trackAdImpression(GromoreAdType.Banner, placementId, ecpmInfo);
        }

        public static void TrackNativeAdRevenue(string placementId, MediationAdEcpmInfo ecpmInfo)
        {
            Debug.Log("GromoreAdWrapper.TrackNativeAdRevenue()");
            GromoreSolarEngineTracker.trackAdImpression(GromoreAdType.Native, placementId, ecpmInfo);
        }

        public static void TrackSplashAdRevenue(string placementId, MediationAdEcpmInfo ecpmInfo)
        {
            Debug.Log("GromoreAdWrapper.TrackSplashAdRevenue()");
            GromoreSolarEngineTracker.trackAdImpression(GromoreAdType.Splash, placementId, ecpmInfo);
        }
    }
}


