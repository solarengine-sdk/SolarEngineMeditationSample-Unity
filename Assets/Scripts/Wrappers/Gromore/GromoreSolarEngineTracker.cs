using UnityEngine;
using SolarEngine;
using ByteDance.Union.Mediation;

namespace SolarEngineMeditationSample.Wrappers.Gromore
{
    /// <summary>
    /// Gromore -> SolarEngine tracker bridge for Unity
    /// Only exposes trackAdImpression(ImpressionAttributes attributes)
    /// </summary>
    public static class GromoreSolarEngineTracker
    {
        public static void trackAdImpression(GromoreAdType adType, string placementId, MediationAdEcpmInfo ecpm)
        {
            Debug.Log("GromoreSolarEngineTracker.trackAdImpression() called");

            double ecpmValue = 0;
            if (ecpm != null)
            {
                // ecpm may be a string in some SDK versions; try to parse
                if (!double.TryParse(ecpm.ecpm, out ecpmValue))
                {
                    ecpmValue = 0;
                }
            }

            var attributes = new ImpressionAttributes
            {
                ad_platform = ecpm != null ? ecpm.sdkName : "Gromore",
                mediation_platform = "Gromore",
                ad_appid = "",
                ad_id = placementId,
                ad_type = (int) adType,
                ad_ecpm = ecpmValue,
                currency_type = "USD",
                is_rendered = true,
                customProperties = null
            };

            Analytics.trackAdImpression(attributes);
        }
    }
}


