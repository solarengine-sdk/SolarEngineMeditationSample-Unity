using UnityEngine;
using SolarEngine;
using Unity.Services.LevelPlay;

namespace SolarEngineMeditationSample.Wrappers.IronSource
{
    /// <summary>
    /// IronSource -> SolarEngine tracker bridge for Unity
    /// Only exposes trackAdImpression(ImpressionAttributes attributes)
    /// </summary>
    public static class IronSourceSolarEngineTracker
    {
        public static void trackAdImpression(LevelPlayImpressionData impressionData)
        {
            Debug.Log("IronSourceSolarEngineTracker.trackAdImpression() called");

            if (impressionData == null)
            {
                var emptyAttributes = new ImpressionAttributes
                {
                    ad_platform = "",
                    mediation_platform = "IronSource",
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

            IronSourceAdType adType = IronSourceAdType.Other;

            // Map format to enum (based on iOS logic)
            var format = impressionData.AdFormat != null ? impressionData.AdFormat.ToLowerInvariant() : string.Empty;
            if (!string.IsNullOrEmpty(format))
            {
                if (format.Contains("interstitial"))
                {
                    adType = IronSourceAdType.Interstitial;
                }
                else if (format.Contains("rewarded") || format.Contains("reward"))
                {
                    adType = IronSourceAdType.RewardVideo;
                }
                else if (format.Contains("banner"))
                {
                    adType = IronSourceAdType.Banner;
                }
                else if (format.Contains("native"))
                {
                    adType = IronSourceAdType.Native;
                }
                else if (format.Contains("splash") || format.Contains("appopen"))
                {
                    adType = IronSourceAdType.Splash;
                }
            }


            var attributes = new ImpressionAttributes
            {
                ad_platform = impressionData.AdNetwork,
                mediation_platform = "IronSource",
                ad_appid = "",
                ad_id = impressionData.InstanceId,
                ad_type = (int) adType,
                ad_ecpm = (impressionData.Revenue ?? 0) * 1000.0,
                currency_type = "USD",
                is_rendered = true,
                customProperties = null
            };

            Analytics.trackAdImpression(attributes);
        }
    }
}


