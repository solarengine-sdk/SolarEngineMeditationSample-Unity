using UnityEngine;

namespace SolarEngineMeditationSample.IronSource
{
    /// <summary>
    /// Configuration for IronSource LevelPlay SDK
    /// </summary>
    [CreateAssetMenu(fileName = "IronSourceConfig", menuName = "SolarEngine/IronSource Config")]
    public class IronSourceConfig : ScriptableObject
    {
        [Header("IronSource App Configuration")]
        [SerializeField] private string appKey = "YOUR_IRONSOURCE_APP_KEY";
        
        [Header("User Configuration")]
        [SerializeField] private string userId = "";
        
        [Header("Platform-Specific App Keys")]
        [SerializeField] private string androidAppKey = "";
        [SerializeField] private string iosAppKey = "";
        
        [Header("Debug Settings")]
        [SerializeField] private bool enableDebugLogging = true;
        [SerializeField] private bool enableTestSuite = true;
        
        [Header("Ad Unit Configuration")]
        [SerializeField] private string rewardedAdUnitId = "DefaultRewardedVideo";
        [SerializeField] private string interstitialAdUnitId = "DefaultInterstitial";
        [SerializeField] private string bannerAdUnitId = "DefaultBanner";
        
        #region Public Properties
        
        /// <summary>
        /// Get the app key for the current platform
        /// </summary>
        public string GetAppKey()
        {
            #if UNITY_ANDROID
                return !string.IsNullOrEmpty(androidAppKey) ? androidAppKey : appKey;
            #elif UNITY_IOS
                return !string.IsNullOrEmpty(iosAppKey) ? iosAppKey : appKey;
            #else
                return appKey;
            #endif
        }
        
        /// <summary>
        /// Get the user ID
        /// </summary>
        public string GetUserId() => userId;
        
        /// <summary>
        /// Check if debug logging is enabled
        /// </summary>
        public bool IsDebugLoggingEnabled => enableDebugLogging;
        
        /// <summary>
        /// Check if test suite is enabled
        /// </summary>
        public bool IsTestSuiteEnabled => enableTestSuite;
        
        /// <summary>
        /// Get rewarded ad unit ID
        /// </summary>
        public string GetRewardedAdUnitId() => rewardedAdUnitId;
        
        /// <summary>
        /// Get interstitial ad unit ID
        /// </summary>
        public string GetInterstitialAdUnitId() => interstitialAdUnitId;
        
        /// <summary>
        /// Get banner ad unit ID
        /// </summary>
        public string GetBannerAdUnitId() => bannerAdUnitId;
        
        #endregion
        
        #region Editor Methods
        
        #if UNITY_EDITOR
        /// <summary>
        /// Set the app key (editor only)
        /// </summary>
        public void SetAppKey(string newAppKey)
        {
            appKey = newAppKey;
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        /// <summary>
        /// Set the Android app key (editor only)
        /// </summary>
        public void SetAndroidAppKey(string newAndroidAppKey)
        {
            androidAppKey = newAndroidAppKey;
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        /// <summary>
        /// Set the iOS app key (editor only)
        /// </summary>
        public void SetIosAppKey(string newIosAppKey)
        {
            iosAppKey = newIosAppKey;
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        /// <summary>
        /// Set the user ID (editor only)
        /// </summary>
        public void SetUserId(string newUserId)
        {
            userId = newUserId;
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        /// <summary>
        /// Set debug logging (editor only)
        /// </summary>
        public void SetDebugLogging(bool enabled)
        {
            enableDebugLogging = enabled;
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        /// <summary>
        /// Set test suite (editor only)
        /// </summary>
        public void SetTestSuite(bool enabled)
        {
            enableTestSuite = enabled;
            UnityEditor.EditorUtility.SetDirty(this);
        }
        #endif
        
        #endregion
        
        #region Validation
        
        /// <summary>
        /// Validate the configuration
        /// </summary>
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(GetAppKey()))
            {
                Debug.LogError("IronSource: App key is not set!");
                return false;
            }
            
            if (GetAppKey() == "YOUR_IRONSOURCE_APP_KEY")
            {
                Debug.LogWarning("IronSource: Using default app key. Please set your actual IronSource app key!");
                return false;
            }
            
            return true;
        }
        
        #endregion
    }
}
