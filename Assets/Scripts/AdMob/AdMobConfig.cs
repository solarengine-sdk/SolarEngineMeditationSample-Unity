using UnityEngine;

namespace SolarEngineMeditationSample.AdMob
{
    /// <summary>
    /// AdMob configuration class that stores app ID and ad unit IDs
    /// </summary>
    [CreateAssetMenu(fileName = "AdMobConfig", menuName = "AdMob/Configuration")]
    public class AdMobConfig : ScriptableObject
    {
        [Header("AdMob App ID")]
        [SerializeField] private string androidAppId = "ca-app-pub-3940256099942544~3347511713"; // Test App ID
        [SerializeField] private string iosAppId = "ca-app-pub-3940256099942544~1458002511"; // Test App ID
        
        [Header("Banner Ad Unit IDs")]
        [SerializeField] private string androidBannerAdUnitId = "ca-app-pub-3940256099942544/6300978111"; // Test Banner
        [SerializeField] private string iosBannerAdUnitId = "ca-app-pub-3940256099942544/2934735716"; // Test Banner
        
        [Header("Interstitial Ad Unit IDs")]
        [SerializeField] private string androidInterstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712"; // Test Interstitial
        [SerializeField] private string iosInterstitialAdUnitId = "ca-app-pub-3940256099942544/4411468910"; // Test Interstitial
        
        [Header("Rewarded Ad Unit IDs")]
        [SerializeField] private string androidRewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917"; // Test Rewarded
        [SerializeField] private string iosRewardedAdUnitId = "ca-app-pub-3940256099942544/1712485313"; // Test Rewarded
        
        [Header("Native Ad Unit IDs")]
        [SerializeField] private string androidNativeAdUnitId = "ca-app-pub-3940256099942544/2247696110"; // Test Native
        [SerializeField] private string iosNativeAdUnitId = "ca-app-pub-3940256099942544/3985214053"; // Test Native
        
        [Header("Splash Ad Unit IDs")]
        [SerializeField] private string androidSplashAdUnitId = "ca-app-pub-3940256099942544/3419835294"; // Test App Open
        [SerializeField] private string iosSplashAdUnitId = "ca-app-pub-3940256099942544/5662855259"; // Test App Open

        /// <summary>
        /// Get the AdMob App ID for the current platform
        /// </summary>
        public string GetAppId()
        {
            #if UNITY_ANDROID
                return androidAppId;
            #elif UNITY_IOS
                return iosAppId;
            #else
                return androidAppId; // Default to Android
            #endif
        }

        /// <summary>
        /// Get Banner Ad Unit ID for the current platform
        /// </summary>
        public string GetBannerAdUnitId()
        {
            #if UNITY_ANDROID
                return androidBannerAdUnitId;
            #elif UNITY_IOS
                return iosBannerAdUnitId;
            #else
                return androidBannerAdUnitId; // Default to Android
            #endif
        }

        /// <summary>
        /// Get Interstitial Ad Unit ID for the current platform
        /// </summary>
        public string GetInterstitialAdUnitId()
        {
            #if UNITY_ANDROID
                return androidInterstitialAdUnitId;
            #elif UNITY_IOS
                return iosInterstitialAdUnitId;
            #else
                return androidInterstitialAdUnitId; // Default to Android
            #endif
        }

        /// <summary>
        /// Get Rewarded Ad Unit ID for the current platform
        /// </summary>
        public string GetRewardedAdUnitId()
        {
            #if UNITY_ANDROID
                return androidRewardedAdUnitId;
            #elif UNITY_IOS
                return iosRewardedAdUnitId;
            #else
                return androidRewardedAdUnitId; // Default to Android
            #endif
        }

        /// <summary>
        /// Get Native Ad Unit ID for the current platform
        /// </summary>
        public string GetNativeAdUnitId()
        {
            #if UNITY_ANDROID
                return androidNativeAdUnitId;
            #elif UNITY_IOS
                return iosNativeAdUnitId;
            #else
                return androidNativeAdUnitId; // Default to Android
            #endif
        }

        /// <summary>
        /// Get Splash/App Open Ad Unit ID for the current platform
        /// </summary>
        public string GetSplashAdUnitId()
        {
            #if UNITY_ANDROID
                return androidSplashAdUnitId;
            #elif UNITY_IOS
                return iosSplashAdUnitId;
            #else
                return androidSplashAdUnitId; // Default to Android
            #endif
        }

        /// <summary>
        /// Get Ad Unit ID for a specific ad type
        /// </summary>
        public string GetAdUnitId(AdType adType)
        {
            switch (adType)
            {
                case AdType.Banner:
                    return GetBannerAdUnitId();
                case AdType.Interstitial:
                    return GetInterstitialAdUnitId();
                case AdType.Rewarded:
                    return GetRewardedAdUnitId();
                case AdType.Native:
                    return GetNativeAdUnitId();
                case AdType.Splash:
                    return GetSplashAdUnitId();
                default:
                    return GetBannerAdUnitId();
            }
        }
    }

    /// <summary>
    /// Ad types supported by AdMob
    /// </summary>
    public enum AdType
    {
        Banner,
        Interstitial,
        Rewarded,
        Native,
        Splash
    }
}
