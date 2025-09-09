using UnityEngine;
using SolarEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace SolarEngineMeditationSample.Wrappers.AdMob
{
    /// <summary>
    /// AdMob -> SolarEngine tracker bridge for Unity
    /// Only exposes trackAdImpression(ImpressionAttributes attributes)
    /// </summary>
    public static class AdMobSolarEngineTracker
    {
        /// <summary>
        /// Forward an ad impression to SolarEngine Analytics
        /// </summary>
        /// <param name="adType">AdMob ad type</param>
        public static void trackAdImpression(AdMobAdType adType, AdValue adValue, ResponseInfo responseInfo)
        {
            Debug.Log("AdMobSolarEngineTracker.trackAdImpression() called");

            long valueMicros = adValue.Value;
            string currencyCode = adValue.CurrencyCode;

        
            AdapterResponseInfo loadedAdapterResponseInfo = responseInfo.GetLoadedAdapterResponseInfo();
            string adSourceInstanceId = loadedAdapterResponseInfo.AdSourceInstanceId;
            string adSourceName = loadedAdapterResponseInfo.AdSourceName;
   

            ImpressionAttributes impressionAttributes = new ImpressionAttributes();
            impressionAttributes.ad_platform = adSourceName;
            impressionAttributes.ad_appid = "";
            impressionAttributes.ad_id = adSourceInstanceId;
            impressionAttributes.ad_type = (int)adType;
            impressionAttributes.ad_ecpm = valueMicros / 1000.0;
            impressionAttributes.currency_type = currencyCode;
            impressionAttributes.mediation_platform = "AdMob";
            impressionAttributes.is_rendered = true;
 
            // Optional: add custom properties if needed
            impressionAttributes.customProperties = null;
            // customProperties 为自定义属性，可以不设置
            // 注：开发者传入属性 key 不能为"_"下划线开头，"_"下划线开头为SDK保留字段，开发者设置则直接报错丢弃
            SolarEngine.Analytics.trackAdImpression(impressionAttributes);


        }
        
    }
}


