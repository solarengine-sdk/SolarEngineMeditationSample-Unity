using UnityEngine;
using Unity.Services.LevelPlay;
using SolarEngineMeditationSample.Wrappers.IronSource;
using System;
using System.Collections.Generic;

namespace SolarEngineMeditationSample.IronSource
{
    /// <summary>
    /// Manager for IronSource LevelPlay SDK integration
    /// </summary>
    public class IronSourceManager : MonoBehaviour
    {
        [Header("IronSource Configuration")]
        [SerializeField] private string appKey = "YOUR_IRONSOURCE_APP_KEY";
        [SerializeField] private string userId = "";
        
        [Header("Debug Settings")]
        [SerializeField] private bool enableDebugLogging = true;
        [SerializeField] private bool enableTestSuite = true;
        
        // Events
        public static event Action OnInitializationSuccess;
        public static event Action<string> OnInitializationFailed;
        
        // Singleton instance
        public static IronSourceManager Instance { get; private set; }
        
        // Ad state tracking
        private bool isInitialized = false;
        private bool isRewardedAdReady = false;
        private bool isInterstitialAdReady = false;
        private bool isBannerAdReady = false;
        
        // LevelPlay ad objects
        private LevelPlayRewardedAd rewardedAd;
        private LevelPlayInterstitialAd interstitialAd;
        private LevelPlayBannerAd bannerAd;
        
        // Configuration
        private IronSourceConfig config;
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            // Try to find or create config, then initialize
            SetupConfigAndInitialize();
        }
        
        private void OnDestroy()
        {
            // Clean up event listeners
            LevelPlay.OnInitSuccess -= SdkInitializationCompletedEvent;
            LevelPlay.OnInitFailed -= SdkInitializationFailedEvent;
            LevelPlay.OnImpressionDataReady -= IronSourceWrapper.BuildImpressionDataHandler();
        }
        
        #endregion
        
        #region Initialization
        
        /// <summary>
        /// Setup configuration and initialize IronSource
        /// </summary>
        private void SetupConfigAndInitialize()
        {
            if (enableDebugLogging)
                Debug.Log("IronSource: Setting up configuration...");
            
            // Try to find existing IronSourceConfig in Resources or create one
            IronSourceConfig existingConfig = Resources.Load<IronSourceConfig>("IronSourceConfig");
            if (existingConfig == null)
            {
                if (enableDebugLogging)
                    Debug.Log("IronSource: No config found in Resources, creating default config...");
                
                // Create a new config with default values
                config = ScriptableObject.CreateInstance<IronSourceConfig>();
                config.name = "IronSourceConfig";
                
                // Set some default values for testing
                #if UNITY_EDITOR
                // In editor, we can set some test values
                if (enableDebugLogging)
                    Debug.Log("IronSource: Using default config for testing");
                #endif
            }
            else
            {
                if (enableDebugLogging)
                    Debug.Log("IronSource: Found existing config in Resources");
                config = existingConfig;
            }
            
            // Now initialize with the config
            SetConfig(config);
        }
        
        /// <summary>
        /// Initialize IronSource LevelPlay SDK
        /// </summary>
        private void InitializeIronSource()
        {
            if (enableDebugLogging)
                Debug.Log("IronSource: Starting initialization...");
            
            // Check if app key is valid
            if (string.IsNullOrEmpty(appKey) || appKey == "YOUR_IRONSOURCE_APP_KEY")
            {
                Debug.LogError("IronSource: Invalid app key! Please set a valid IronSource app key in the config.");
                isInitialized = false;
                return;
            }
            
            // Enable test suite if requested
            if (enableTestSuite)
            {
                LevelPlay.SetMetaData("is_test_suite", "enable");
                if (enableDebugLogging)
                    Debug.Log("IronSource: Test suite enabled");
            }
            
            // Register event listeners BEFORE initializing
            LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
            LevelPlay.OnInitFailed += SdkInitializationFailedEvent;
            
            // Register impression data event using the wrapper
            LevelPlay.OnImpressionDataReady += IronSourceWrapper.BuildImpressionDataHandler();
            if (enableDebugLogging)
                Debug.Log("IronSource: Impression data event registered with IronSourceWrapper");
            
            // Initialize the SDK
            if (!string.IsNullOrEmpty(userId))
            {
                LevelPlay.Init(appKey, userId);
                if (enableDebugLogging)
                    Debug.Log($"IronSource: Initializing with App Key: {appKey}, User ID: {userId}");
            }
            else
            {
                LevelPlay.Init(appKey);
                if (enableDebugLogging)
                    Debug.Log($"IronSource: Initializing with App Key: {appKey}");
            }
        }
        
        /// <summary>
        /// SDK initialization success callback
        /// </summary>
        private void SdkInitializationCompletedEvent(LevelPlayConfiguration config)
        {
            isInitialized = true;
            
            if (enableDebugLogging)
                Debug.Log($"IronSource: SDK initialized successfully with config: {config}!");
            
            // Create ad objects after successful initialization
            CreateAdObjects();
            
            // Launch test suite if enabled
            if (enableTestSuite)
            {
                LevelPlay.LaunchTestSuite();
                if (enableDebugLogging)
                    Debug.Log("IronSource: Test suite launched");
            }
            
            // Notify other systems
            OnInitializationSuccess?.Invoke();
        }
        
        /// <summary>
        /// SDK initialization failed callback
        /// </summary>
        private void SdkInitializationFailedEvent(LevelPlayInitError error)
        {
            isInitialized = false;
            
            Debug.LogError($"IronSource: SDK initialization failed: {error}");
            
            // Notify other systems
            OnInitializationFailed?.Invoke(error.ErrorMessage);
        }
        
        #endregion
        
        #region Ad Object Creation
        
        /// <summary>
        /// Create LevelPlay ad objects after SDK initialization
        /// </summary>
        private void CreateAdObjects()
        {
            if (enableDebugLogging)
                Debug.Log("IronSource: Creating ad objects...");
            
            try
            {
                // Get ad unit IDs from config or use defaults
                string rewardedAdUnitId = config?.GetRewardedAdUnitId() ?? "DefaultRewardedVideo";
                string interstitialAdUnitId = config?.GetInterstitialAdUnitId() ?? "DefaultInterstitial";
                string bannerAdUnitId = config?.GetBannerAdUnitId() ?? "DefaultBanner";
                
                // Create Rewarded Video ad object
                rewardedAd = new LevelPlayRewardedAd(rewardedAdUnitId);
                RegisterRewardedAdEvents();
                
                // Create Interstitial ad object
                interstitialAd = new LevelPlayInterstitialAd(interstitialAdUnitId);
                RegisterInterstitialAdEvents();
                
                // Create Banner ad object
                bannerAd = new LevelPlayBannerAd(bannerAdUnitId);
                RegisterBannerAdEvents();
                
                if (enableDebugLogging)
                    Debug.Log($"IronSource: Ad objects created successfully - Rewarded: {rewardedAdUnitId}, Interstitial: {interstitialAdUnitId}, Banner: {bannerAdUnitId}");
            }
            catch (Exception e)
            {
                Debug.LogError($"IronSource: Error creating ad objects: {e.Message}");
            }
        }
        
        /// <summary>
        /// Register events for Rewarded Video ads
        /// </summary>
        private void RegisterRewardedAdEvents()
        {
            if (rewardedAd == null) return;
            
            rewardedAd.OnAdLoaded += RewardedAdOnLoadedEvent;
            rewardedAd.OnAdLoadFailed += RewardedAdOnLoadFailedEvent;
            rewardedAd.OnAdDisplayed += RewardedAdOnDisplayedEvent;
            rewardedAd.OnAdDisplayFailed += RewardedAdOnDisplayFailedEvent;
            rewardedAd.OnAdRewarded += RewardedAdOnRewardedEvent;
            rewardedAd.OnAdClicked += RewardedAdOnClickedEvent;
            rewardedAd.OnAdClosed += RewardedAdOnClosedEvent;
            rewardedAd.OnAdInfoChanged += RewardedAdOnInfoChangedEvent;
        }
        
        /// <summary>
        /// Register events for Interstitial ads
        /// </summary>
        private void RegisterInterstitialAdEvents()
        {
            if (interstitialAd == null) return;
            
            interstitialAd.OnAdLoaded += InterstitialAdOnLoadedEvent;
            interstitialAd.OnAdLoadFailed += InterstitialAdOnLoadFailedEvent;
            interstitialAd.OnAdDisplayed += InterstitialAdOnDisplayedEvent;
            interstitialAd.OnAdDisplayFailed += InterstitialAdOnDisplayFailedEvent;
            interstitialAd.OnAdClicked += InterstitialAdOnClickedEvent;
            interstitialAd.OnAdClosed += InterstitialAdOnClosedEvent;
            interstitialAd.OnAdInfoChanged += InterstitialAdOnInfoChangedEvent;
        }
        
        /// <summary>
        /// Register events for Banner ads
        /// </summary>
        private void RegisterBannerAdEvents()
        {
            if (bannerAd == null) return;
            
            bannerAd.OnAdLoaded += BannerAdOnLoadedEvent;
            bannerAd.OnAdLoadFailed += BannerAdOnLoadFailedEvent;
            bannerAd.OnAdDisplayed += BannerAdOnDisplayedEvent;
            bannerAd.OnAdDisplayFailed += BannerAdOnDisplayFailedEvent;
            bannerAd.OnAdClicked += BannerAdOnClickedEvent;
            bannerAd.OnAdCollapsed += BannerAdOnCollapsedEvent;
            bannerAd.OnAdLeftApplication += BannerAdOnLeftApplicationEvent;
            bannerAd.OnAdExpanded += BannerAdOnExpandedEvent;
        }
        
        #endregion
        
        #region Public API
        
        /// <summary>
        /// Check if IronSource is initialized
        /// </summary>
        public bool IsInitialized => isInitialized;
        
        /// <summary>
        /// Check if IronSource configuration is valid
        /// </summary>
        public bool IsConfigValid => config != null && config.IsValid();
        
        /// <summary>
        /// Get initialization status details
        /// </summary>
        public string GetInitializationStatus()
        {
            if (config == null)
                return "No configuration set";
            
            if (!config.IsValid())
                return "Invalid configuration - app key not set";
            
            if (!isInitialized)
                return "SDK not initialized - check console for errors";
            
            return "SDK initialized successfully";
        }
        
        /// <summary>
        /// Log current configuration status for debugging
        /// </summary>
        public void LogConfigurationStatus()
        {
            Debug.Log("=== IronSource Configuration Status ===");
            Debug.Log($"Config exists: {config != null}");
            if (config != null)
            {
                Debug.Log($"App Key: {config.GetAppKey()}");
                Debug.Log($"User ID: {config.GetUserId()}");
                Debug.Log($"Debug Enabled: {config.IsDebugLoggingEnabled}");
                Debug.Log($"Test Suite Enabled: {config.IsTestSuiteEnabled}");
                Debug.Log($"Config Valid: {config.IsValid()}");
            }
            Debug.Log($"Is Initialized: {isInitialized}");
            Debug.Log($"Rewarded Ad Ready: {isRewardedAdReady}");
            Debug.Log($"Interstitial Ad Ready: {isInterstitialAdReady}");
            Debug.Log($"Banner Ad Ready: {isBannerAdReady}");
            Debug.Log("=====================================");
        }
        
        /// <summary>
        /// Test the IronSource wrapper integration
        /// </summary>
        public void TestWrapperIntegration()
        {
            if (enableDebugLogging)
                Debug.Log("IronSource: Testing wrapper integration...");
            
            // Test the wrapper by creating a test impression data handler
            var testHandler = IronSourceWrapper.BuildImpressionDataHandler((impressionData) => {
                Debug.Log($"IronSource: Test wrapper callback received: {impressionData}");
            });
            
            if (enableDebugLogging)
                Debug.Log("IronSource: Wrapper integration test completed");
        }
        
        /// <summary>
        /// Check if rewarded ad is ready
        /// </summary>
        public bool IsRewardedAdReady => isRewardedAdReady;
        
        /// <summary>
        /// Check if interstitial ad is ready
        /// </summary>
        public bool IsInterstitialAdReady => isInterstitialAdReady;
        
        /// <summary>
        /// Check if banner ad is ready
        /// </summary>
        public bool IsBannerAdReady => isBannerAdReady;
        
        /// <summary>
        /// Set the app key (useful for runtime configuration)
        /// </summary>
        public void SetAppKey(string newAppKey)
        {
            appKey = newAppKey;
            if (enableDebugLogging)
                Debug.Log($"IronSource: App key updated to: {appKey}");
        }
        
        /// <summary>
        /// Set the configuration for IronSource
        /// </summary>
        public void SetConfig(IronSourceConfig config)
        {
            if (config == null)
            {
                Debug.LogError("IronSource: Config is null!");
                return;
            }
            
            if (enableDebugLogging)
                Debug.Log("IronSource: Setting configuration...");
            
            // Store the config reference
            this.config = config;
            
            // Set app key and user ID from config
            appKey = config.GetAppKey();
            userId = config.GetUserId();
            
            // Set debug settings
            enableDebugLogging = config.IsDebugLoggingEnabled;
            enableTestSuite = config.IsTestSuiteEnabled;
            
            if (enableDebugLogging)
                Debug.Log($"IronSource: Config set - App Key: {appKey}, User ID: {userId}, Debug: {enableDebugLogging}, Test Suite: {enableTestSuite}");
            
            // If already initialized, reinitialize with new config
            if (isInitialized)
            {
                if (enableDebugLogging)
                    Debug.Log("IronSource: Reinitializing with new config...");
                Reinitialize();
            }
            else
            {
                // Initialize for the first time
                if (enableDebugLogging)
                    Debug.Log("IronSource: Initializing for the first time with config...");
                InitializeIronSource();
            }
        }
        
        /// <summary>
        /// Set the user ID (useful for runtime configuration)
        /// </summary>
        public void SetUserId(string newUserId)
        {
            userId = newUserId;
            if (enableDebugLogging)
                Debug.Log($"IronSource: User ID updated to: {userId}");
        }
        
        /// <summary>
        /// Reinitialize the SDK (useful after configuration changes)
        /// </summary>
        public void Reinitialize()
        {
            if (enableDebugLogging)
                Debug.Log("IronSource: Reinitializing SDK...");
            
            // Clean up existing listeners
            LevelPlay.OnInitSuccess -= SdkInitializationCompletedEvent;
            LevelPlay.OnInitFailed -= SdkInitializationFailedEvent;
            LevelPlay.OnImpressionDataReady -= IronSourceWrapper.BuildImpressionDataHandler();
            
            // Reset state
            isInitialized = false;
            isRewardedAdReady = false;
            isInterstitialAdReady = false;
            isBannerAdReady = false;
            
            // Reinitialize
            InitializeIronSource();
        }
        
        #endregion
        
        #region Ad Event Handlers
        
        // Rewarded Video Event Handlers
        private void RewardedAdOnLoadedEvent(LevelPlayAdInfo adInfo)
        {
            isRewardedAdReady = true;
            if (enableDebugLogging)
                Debug.Log($"IronSource: Rewarded ad loaded - {adInfo}");
        }
        
        private void RewardedAdOnLoadFailedEvent(LevelPlayAdError error)
        {
            isRewardedAdReady = false;
            if (enableDebugLogging)
                Debug.LogError($"IronSource: Rewarded ad load failed - {error}");
        }
        
        private void RewardedAdOnDisplayedEvent(LevelPlayAdInfo adInfo)
        {
            if (enableDebugLogging)
                Debug.Log($"IronSource: Rewarded ad displayed - {adInfo}");
        }
        
        private void RewardedAdOnDisplayFailedEvent(LevelPlayAdDisplayInfoError error)
        {
            if (enableDebugLogging)
                Debug.LogError($"IronSource: Rewarded ad display failed - {error}");
        }
        
        private void RewardedAdOnRewardedEvent(LevelPlayAdInfo adInfo, LevelPlayReward reward)
        {
            if (enableDebugLogging)
                Debug.Log($"IronSource: Rewarded ad rewarded - {adInfo}, Reward: {reward}");
        }
        
        private void RewardedAdOnClickedEvent(LevelPlayAdInfo adInfo)
        {
            if (enableDebugLogging)
                Debug.Log($"IronSource: Rewarded ad clicked - {adInfo}");
        }
        
        private void RewardedAdOnClosedEvent(LevelPlayAdInfo adInfo)
        {
            isRewardedAdReady = false;
            if (enableDebugLogging)
                Debug.Log($"IronSource: Rewarded ad closed - {adInfo}");
            
            // Load new rewarded ad
            LoadRewardedAd();
        }
        
        private void RewardedAdOnInfoChangedEvent(LevelPlayAdInfo adInfo)
        {
            if (enableDebugLogging)
                Debug.Log($"IronSource: Rewarded ad info changed - {adInfo}");
        }
        
        // Interstitial Event Handlers
        private void InterstitialAdOnLoadedEvent(LevelPlayAdInfo adInfo)
        {
            isInterstitialAdReady = true;
            if (enableDebugLogging)
                Debug.Log($"IronSource: Interstitial ad loaded - {adInfo}");
        }
        
        private void InterstitialAdOnLoadFailedEvent(LevelPlayAdError error)
        {
            isInterstitialAdReady = false;
            if (enableDebugLogging)
                Debug.LogError($"IronSource: Interstitial ad load failed - {error}");
        }
        
        private void InterstitialAdOnDisplayedEvent(LevelPlayAdInfo adInfo)
        {
            if (enableDebugLogging)
                Debug.Log($"IronSource: Interstitial ad displayed - {adInfo}");
        }
        
        private void InterstitialAdOnDisplayFailedEvent(LevelPlayAdDisplayInfoError error)
        {
            if (enableDebugLogging)
                Debug.LogError($"IronSource: Interstitial ad display failed - {error}");
        }
        
        private void InterstitialAdOnClickedEvent(LevelPlayAdInfo adInfo)
        {
            if (enableDebugLogging)
                Debug.Log($"IronSource: Interstitial ad clicked - {adInfo}");
        }
        
        private void InterstitialAdOnClosedEvent(LevelPlayAdInfo adInfo)
        {
            isInterstitialAdReady = false;
            if (enableDebugLogging)
                Debug.Log($"IronSource: Interstitial ad closed - {adInfo}");
            
            // Load new interstitial ad
            LoadInterstitialAd();
        }
        
        private void InterstitialAdOnInfoChangedEvent(LevelPlayAdInfo adInfo)
        {
            if (enableDebugLogging)
                Debug.Log($"IronSource: Interstitial ad info changed - {adInfo}");
        }
        
        // Banner Event Handlers
        private void BannerAdOnLoadedEvent(LevelPlayAdInfo adInfo)
        {
            isBannerAdReady = true;
            if (enableDebugLogging)
                Debug.Log($"IronSource: Banner ad loaded - {adInfo}");
        }
        
        private void BannerAdOnLoadFailedEvent(LevelPlayAdError error)
        {
            isBannerAdReady = false;
            if (enableDebugLogging)
                Debug.LogError($"IronSource: Banner ad load failed - {error}");
        }
        
        private void BannerAdOnDisplayedEvent(LevelPlayAdInfo adInfo)
        {
            if (enableDebugLogging)
                Debug.Log($"IronSource: Banner ad displayed - {adInfo}");
        }
        
        private void BannerAdOnDisplayFailedEvent(LevelPlayAdDisplayInfoError error)
        {
            if (enableDebugLogging)
                Debug.LogError($"IronSource: Banner ad display failed - {error}");
        }
        
        private void BannerAdOnClickedEvent(LevelPlayAdInfo adInfo)
        {
            if (enableDebugLogging)
                Debug.Log($"IronSource: Banner ad clicked - {adInfo}");
        }
        
        private void BannerAdOnCollapsedEvent(LevelPlayAdInfo adInfo)
        {
            if (enableDebugLogging)
                Debug.Log($"IronSource: Banner ad collapsed - {adInfo}");
        }
        
        private void BannerAdOnLeftApplicationEvent(LevelPlayAdInfo adInfo)
        {
            if (enableDebugLogging)
                Debug.Log($"IronSource: Banner ad left application - {adInfo}");
        }
        
        private void BannerAdOnExpandedEvent(LevelPlayAdInfo adInfo)
        {
            if (enableDebugLogging)
                Debug.Log($"IronSource: Banner ad expanded - {adInfo}");
        }
        
        #endregion
        
        #region Ad Loading and Showing
        
        /// <summary>
        /// Load and show banner ad
        /// </summary>
        public void LoadAndShowBannerAd()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("IronSource not initialized yet");
                return;
            }
            
            if (bannerAd == null)
            {
                Debug.LogWarning("IronSource: Banner ad object not created yet");
                return;
            }
            
            if (enableDebugLogging)
                Debug.Log("IronSource: Loading and showing banner ad...");
            
            try
            {
                // Load the banner ad
                bannerAd.LoadAd();
            }
            catch (Exception e)
            {
                Debug.LogError($"IronSource: Error loading banner ad: {e.Message}");
            }
        }
        
        /// <summary>
        /// Load and show interstitial ad
        /// </summary>
        public void LoadAndShowInterstitialAd()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("IronSource not initialized yet");
                return;
            }
            
            if (interstitialAd == null)
            {
                Debug.LogWarning("IronSource: Interstitial ad object not created yet");
                return;
            }
            
            if (enableDebugLogging)
                Debug.Log("IronSource: Loading and showing interstitial ad...");
            
            try
            {
                if (isInterstitialAdReady)
                {
                    // Show the ad if it's ready
                    interstitialAd.ShowAd();
                }
                else
                {
                    // Load the ad if it's not ready
                    interstitialAd.LoadAd();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"IronSource: Error with interstitial ad: {e.Message}");
            }
        }
        
        /// <summary>
        /// Load and show rewarded ad
        /// </summary>
        public void LoadAndShowRewardedAd()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("IronSource not initialized yet");
                return;
            }
            
            if (rewardedAd == null)
            {
                Debug.LogWarning("IronSource: Rewarded ad object not created yet");
                return;
            }
            
            if (enableDebugLogging)
                Debug.Log("IronSource: Loading and showing rewarded ad...");
            
            try
            {
                if (isRewardedAdReady)
                {
                    // Show the ad if it's ready
                    rewardedAd.ShowAd();
                }
                else
                {
                    // Load the ad if it's not ready
                    rewardedAd.LoadAd();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"IronSource: Error with rewarded ad: {e.Message}");
            }
        }
        
        /// <summary>
        /// Load and show app open ad
        /// </summary>
        public void LoadAndShowAppOpenAd()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("IronSource not initialized yet");
                return;
            }
            
            if (enableDebugLogging)
                Debug.Log("IronSource: Loading and showing app open ad...");
            
            // TODO: App Open ads not yet implemented in LevelPlay
            // For now, just log that it's not implemented
            Debug.Log("IronSource: App open ads not yet implemented in LevelPlay");
        }
        
        #endregion
        
        #region Debug Methods
        
        /// <summary>
        /// Toggle debug logging
        /// </summary>
        public void ToggleDebugLogging()
        {
            enableDebugLogging = !enableDebugLogging;
            Debug.Log($"IronSource: Debug logging {(enableDebugLogging ? "enabled" : "disabled")}");
        }
        
        /// <summary>
        /// Toggle test suite
        /// </summary>
        public void ToggleTestSuite()
        {
            enableTestSuite = !enableTestSuite;
            if (enableTestSuite)
            {
                LevelPlay.SetMetaData("is_test_suite", "enable");
                Debug.Log("IronSource: Test suite enabled");
            }
            else
            {
                LevelPlay.SetMetaData("is_test_suite", "disable");
                Debug.Log("IronSource: Test suite disabled");
            }
        }
        
        #endregion
        
        #region Helper Methods
        
        /// <summary>
        /// Load a new rewarded ad
        /// </summary>
        private void LoadRewardedAd()
        {
            if (rewardedAd != null && IsInitialized)
            {
                try
                {
                    rewardedAd.LoadAd();
                    if (enableDebugLogging)
                        Debug.Log("IronSource: Loading new rewarded ad...");
                }
                catch (Exception e)
                {
                    Debug.LogError($"IronSource: Error loading new rewarded ad: {e.Message}");
                }
            }
        }
        
        /// <summary>
        /// Load a new interstitial ad
        /// </summary>
        private void LoadInterstitialAd()
        {
            if (interstitialAd != null && IsInitialized)
            {
                try
                {
                    interstitialAd.LoadAd();
                    if (enableDebugLogging)
                        Debug.Log("IronSource: Loading new interstitial ad...");
                }
                catch (Exception e)
                {
                    Debug.LogError($"IronSource: Error loading new interstitial ad: {e.Message}");
                }
            }
        }
        
        #endregion
    }
}
