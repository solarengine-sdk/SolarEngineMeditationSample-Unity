using UnityEngine;

namespace SolarEngineMeditationSample.Max
{
    /// <summary>
    /// Configuration for Max SDK integration
    /// </summary>
    [CreateAssetMenu(fileName = "MaxConfig", menuName = "SolarEngine/Max Config")]
    public class MaxConfig : ScriptableObject
    {
        [Header("Max SDK Configuration")]
        [SerializeField] private string sdkKey = "";
        [SerializeField] private string userId = "";
        [SerializeField] private bool enableDebugLogging = true;
        [SerializeField] private bool enableTestMode = true;

        [Header("Ad Unit IDs")]
        [SerializeField] private string rewardedAdUnitId = "";
        [SerializeField] private string interstitialAdUnitId = "";
        [SerializeField] private string bannerAdUnitId = "";
        [SerializeField] private string appOpenAdUnitId = "";

        [Header("Advanced Settings")]
        [SerializeField] private bool enableVerboseLogging = false;
        [SerializeField] private bool enableCreativeDebugger = false;

        // Public properties
        public string SdkKey => sdkKey;
        public string UserId => userId;
        public bool EnableDebugLogging => enableDebugLogging;
        public bool EnableTestMode => enableTestMode;
        public bool EnableVerboseLogging => enableVerboseLogging;
        public bool EnableCreativeDebugger => enableCreativeDebugger;

        // Ad Unit ID getters
        public string GetRewardedAdUnitId() => rewardedAdUnitId;
        public string GetInterstitialAdUnitId() => interstitialAdUnitId;
        public string GetBannerAdUnitId() => bannerAdUnitId;
        public string GetAppOpenAdUnitId() => appOpenAdUnitId;

        /// <summary>
        /// Check if the configuration is valid
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(sdkKey);
        }

        /// <summary>
        /// Get configuration status for debugging
        /// </summary>
        public string GetConfigStatus()
        {
            return $"MaxConfig - SDK Key: {(string.IsNullOrEmpty(sdkKey) ? "NOT SET" : "SET")}, " +
                   $"Rewarded: {(string.IsNullOrEmpty(rewardedAdUnitId) ? "NOT SET" : "SET")}, " +
                   $"Interstitial: {(string.IsNullOrEmpty(interstitialAdUnitId) ? "NOT SET" : "SET")}, " +
                   $"Banner: {(string.IsNullOrEmpty(bannerAdUnitId) ? "NOT SET" : "SET")}, " +
                   $"AppOpen: {(string.IsNullOrEmpty(appOpenAdUnitId) ? "NOT SET" : "SET")}";
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
            enableCreativeDebugger = false;
            
            // Note: SDK key and ad unit IDs must be set manually for production use
            Debug.Log("MaxConfig: Default values set. Remember to configure SDK key and ad unit IDs!");
        }

        /// <summary>
        /// Get setup instructions for the user
        /// </summary>
        public string GetSetupInstructions()
        {
            return "To enable Max ads:\n" +
                   "1. Set your Max SDK key (required)\n" +
                   "2. Configure ad unit IDs for each ad type\n" +
                   "3. Adjust debug settings as needed\n" +
                   "4. Save the asset";
        }
    }
}
