using UnityEngine;
using SolarEngine;

namespace SolarEngineMeditationSample.Wrappers.Max
{
    /// <summary>
    /// Max -> SolarEngine tracker bridge for Unity
    /// Only exposes trackAdImpression(ImpressionAttributes attributes)
    /// </summary>
    public static class MaxSolarEngineTracker
    {
        public static void trackAdImpression(MaxAdType adType, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("MaxSolarEngineTracker.trackAdImpression() called");

            if (adInfo == null)
            {
                var emptyAttributes = new ImpressionAttributes
                {
                    ad_platform = "",
                    mediation_platform = "Max",
                    ad_appid = "",
                    ad_id = "",
                    ad_type = 0,
                    ad_ecpm = 0,
                    currency_type = "USD",
                    is_rendered = true,
                    customProperties = null
                };

                Analytics.trackAdImpression(emptyAttributes);                
                return;
            }

            var attributes = new ImpressionAttributes
            {
                ad_platform = adInfo.NetworkName,
                mediation_platform = "Max",
                ad_appid = "",
                ad_id = adInfo.NetworkPlacement,
                ad_type = (int) adType,
                ad_ecpm = adInfo.Revenue * 0.01,
                currency_type = "USD",
                is_rendered = true,
                customProperties = null
            };

            Analytics.trackAdImpression(attributes);
        }
    }
}


