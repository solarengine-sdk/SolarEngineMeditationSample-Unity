using UnityEngine;

namespace SolarEngineMeditationSample.Taku
{
    /// <summary>
    /// Configuration for Taku SDK integration
    /// </summary>
    [CreateAssetMenu(fileName = "TakuConfig", menuName = "SolarEngine/Taku Config")]
    public class TakuConfig : ScriptableObject
    {
        [Header("Taku SDK Configuration")]
        [SerializeField] private string appId = "";
        [SerializeField] private string appKey = "";
        [SerializeField] private bool enableDebugLogging = true;
        [SerializeField] private bool enableTestMode = true;

        [Header("Ad Unit IDs")]
        [SerializeField] private string rewardedAdUnitId = "";
        [SerializeField] private string interstitialAdUnitId = "";
        [SerializeField] private string bannerAdUnitId = "";
        [SerializeField] private string nativeAdUnitId = "";

        [Header("Advanced Settings")]
        [SerializeField] private bool enableVerboseLogging = false;
        [SerializeField] private bool enableTestAd = false;

        // Public properties
        public string AppId => appId;
        public string AppKey => appKey;
        public bool EnableDebugLogging => enableDebugLogging;
        public bool EnableTestMode => enableTestMode;
        public bool EnableVerboseLogging => enableVerboseLogging;
        public bool EnableTestAd => enableTestAd;

        // Ad Unit ID getters
        public string GetRewardedAdUnitId() => rewardedAdUnitId;
        public string GetInterstitialAdUnitId() => interstitialAdUnitId;
        public string GetBannerAdUnitId() => bannerAdUnitId;
        public string GetNativeAdUnitId() => nativeAdUnitId;

        /// <summary>
        /// Check if the configuration is valid
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(appId) && !string.IsNullOrEmpty(appKey);
        }

        /// <summary>
        /// Get configuration status for debugging
        /// </summary>
        public string GetConfigStatus()
        {
            return $"TakuConfig - App ID: {(string.IsNullOrEmpty(appId) ? "NOT SET" : "SET")}, " +
                   $"App Key: {(string.IsNullOrEmpty(appKey) ? "NOT SET" : "SET")}, " +
                   $"Rewarded: {(string.IsNullOrEmpty(rewardedAdUnitId) ? "NOT SET" : "SET")}, " +
                   $"Interstitial: {(string.IsNullOrEmpty(interstitialAdUnitId) ? "NOT SET" : "SET")}, " +
                   $"Banner: {(string.IsNullOrEmpty(bannerAdUnitId) ? "NOT SET" : "SET")}, " +
                   $"Native: {(string.IsNullOrEmpty(nativeAdUnitId) ? "NOT SET" : "SET")}";
        }

        /// <summary>
        /// Set default values for the config
        /// </summary>
        public void SetDefaultValues()
        {
            // Set sensible defaults for development
            enableDebugLogging = true;
            enableTestMode = true;
            enableVerboseLogging = false;
            enableTestAd = false;
            
            // Note: App ID, App Key, and ad unit IDs must be set manually for production use
            Debug.Log("TakuConfig: Default values set. Remember to configure App ID, App Key, and ad unit IDs!");
        }

        /// <summary>
        /// Get setup instructions for the user
        /// </summary>
        public string GetSetupInstructions()
        {
            return "To enable Taku ads:\n" +
                   "1. Set your Taku App ID (required)\n" +
                   "2. Set your Taku App Key (required)\n" +
                   "3. Configure ad unit IDs for each ad type\n" +
                   "4. Adjust debug settings as needed\n" +
                   "5. Save the asset";
        }
    }
}
