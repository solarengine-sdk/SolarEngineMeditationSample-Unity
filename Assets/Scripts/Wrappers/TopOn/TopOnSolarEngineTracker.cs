using UnityEngine;
using SolarEngine;
using AnyThinkAds.Api;

namespace SolarEngineMeditationSample.Wrappers.TopOn
{
    /// <summary>
    /// TopOn -> SolarEngine tracker bridge for Unity
    /// Only exposes trackAdImpression(ImpressionAttributes attributes)
    /// </summary>
    public static class TopOnSolarEngineTracker
    {
        public static void trackAdImpression(TopOnAdType adType, ATCallbackInfo callbackInfo)
        {
            Debug.Log("TopOnSolarEngineTracker.trackAdImpression() called");

            var attributes = new ImpressionAttributes
            {
                ad_platform = callbackInfo != null ? callbackInfo.network_name : "TopOn",
                mediation_platform = "TopOn",
                ad_appid = "",
                ad_id = callbackInfo != null ? callbackInfo.network_placement_id : "",
                ad_type = (int) adType,
                ad_ecpm = callbackInfo != null ? callbackInfo.publisher_revenue : 0,
                currency_type = callbackInfo != null ? callbackInfo.currency : "USD",
                is_rendered = true,
                customProperties = null
            };

            Analytics.trackAdImpression(attributes);
        }
    }
}


