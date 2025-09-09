using System;
using UnityEngine;
using SolarEngineMeditationSample.Wrappers.Max;

namespace SolarEngineMeditationSample.Max
{
    /// <summary>
    /// Singleton manager for Max SDK integration, initialization, and ad lifecycle management
    /// </summary>
    public class MaxManager : MonoBehaviour
    {
        private static MaxManager _instance;
        public static MaxManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<MaxManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("MaxManager");
                        _instance = go.AddComponent<MaxManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        [Header("Max Configuration")]
        private MaxConfig config;
        private bool isInitialized = false;

        [Header("Ad State")]
        private bool isRewardedAdReady = false;
        private bool isInterstitialAdReady = false;
        private bool isBannerAdReady = false;
        private bool isAppOpenAdReady = false;

        // Events
        public event Action<string, MaxSdkBase.AdInfo> OnAdRevenuePaid;
        public event Action<string> OnAdLoaded;
        public event Action<string, MaxSdkBase.ErrorInfo> OnAdLoadFailed;
        public event Action<string> OnAdDisplayed;
        public event Action<string, MaxSdkBase.ErrorInfo> OnAdDisplayFailed;
        public event Action<string> OnAdClicked;
        public event Action<string> OnAdHidden;
        public event Action<string> OnAdExpanded;
        public event Action<string> OnAdCollapsed;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            SetupConfigAndInitialize();
        }

        private void OnDestroy()
        {
            // Clean up event subscriptions
            if (isInitialized)
            {
                UnsubscribeFromEvents();
            }
        }

        /// <summary>
        /// Setup configuration and initialize Max SDK
        /// </summary>
        private void SetupConfigAndInitialize()
        {
            // Find or create MaxConfig (consistent with AdMob/IronSource pattern)
            config = Resources.Load<MaxConfig>("MaxConfig");
            if (config == null)
            {
                Debug.LogWarning("MaxConfig not found in Resources folder. Creating default config.");
                config = ScriptableObject.CreateInstance<MaxConfig>();
                config.name = "MaxConfig";
                
                // Set some sensible defaults
                config.SetDefaultValues();
                
                Debug.Log("MaxConfig created automatically. Please configure SDK key and ad unit IDs in the inspector.");
            }

            SetConfig(config);
        }

        /// <summary>
        /// Set configuration and initialize Max SDK
        /// </summary>
        public void SetConfig(MaxConfig maxConfig)
        {
            if (maxConfig == null)
            {
                Debug.LogError("MaxConfig is null! Cannot initialize Max SDK.");
                return;
            }
            
            config = maxConfig;
            
            if (config.IsValid())
            {
                InitializeMax();
            }
            else
            {
                Debug.LogWarning("MaxConfig is invalid - SDK key not set. Max SDK will not initialize.");
                Debug.LogWarning("Please set your Max SDK key in the MaxConfig asset to enable ads.");
                Debug.LogWarning("You can find the config asset in the Project window under Resources/MaxConfig.asset");
            }
        }

        /// <summary>
        /// Initialize Max SDK
        /// </summary>
        private void InitializeMax()
        {
            if (isInitialized)
            {
                Debug.Log("Max SDK already initialized");
                return;
            }

            if (config == null || !config.IsValid())
            {
                Debug.LogError("MaxConfig is invalid or not assigned!");
                return;
            }

            Debug.Log("Initializing Max SDK...");

            try
            {
                // Set SDK key
                MaxSdk.SetSdkKey(config.SdkKey);

                // Set user ID if provided
                if (!string.IsNullOrEmpty(config.UserId))
                {
                    MaxSdk.SetUserId(config.UserId);
                }

                // Set debug logging
                MaxSdk.SetVerboseLogging(config.EnableVerboseLogging);

                // Set test mode
                if (config.EnableTestMode)
                {
                    // Test mode is set via MaxSdk.SetVerboseLogging for this version
                }

                // Set creative debugger
                if (config.EnableCreativeDebugger)
                {
                    MaxSdk.SetCreativeDebuggerEnabled(true);
                }

                // Initialize SDK
                MaxSdk.InitializeSdk();

                // Subscribe to events
                SubscribeToEvents();

                isInitialized = true;
                Debug.Log("Max SDK initialized successfully");

                // Load initial ads
                LoadInitialAds();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to initialize Max SDK: {e.Message}");
            }
        }

        /// <summary>
        /// Subscribe to Max SDK events
        /// </summary>
        private void SubscribeToEvents()
        {
            // Ad revenue events - these are nested under specific ad types
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += MaxRewardedAdWrapper.BuildAdRevenueHandler(OnAdRevenuePaid);
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += MaxInterstitialAdWrapper.BuildAdRevenueHandler(OnAdRevenuePaid);
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += MaxBannerAdWrapper.BuildAdRevenueHandler(OnAdRevenuePaid);
            MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += MaxAppOpenAdWrapper.BuildAdRevenueHandler(OnAdRevenuePaid);

            // Ad lifecycle events - these are nested under specific ad types
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoaded;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailed;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayed;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdDisplayFailed;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClicked;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHidden;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedReward;

            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialAdLoaded;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialAdLoadFailed;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialAdDisplayed;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdDisplayFailed;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialAdClicked;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialAdHidden;

            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoaded;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailed;
            MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClicked;
            MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpanded;
            MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsed;

            MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += OnAppOpenAdLoaded;
            MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent += OnAppOpenAdLoadFailed;
            MaxSdkCallbacks.AppOpen.OnAdDisplayedEvent += OnAppOpenAdDisplayed;
            MaxSdkCallbacks.AppOpen.OnAdDisplayFailedEvent += OnAppOpenAdDisplayFailed;
            MaxSdkCallbacks.AppOpen.OnAdClickedEvent += OnAppOpenAdClicked;
            MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAppOpenAdHidden;
        }

        /// <summary>
        /// Unsubscribe from Max SDK events
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            // Ad revenue events
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent -= MaxRewardedAdWrapper.BuildAdRevenueHandler(OnAdRevenuePaid);
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent -= MaxInterstitialAdWrapper.BuildAdRevenueHandler(OnAdRevenuePaid);
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent -= MaxBannerAdWrapper.BuildAdRevenueHandler(OnAdRevenuePaid);
            MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent -= MaxAppOpenAdWrapper.BuildAdRevenueHandler(OnAdRevenuePaid);

            // Ad lifecycle events - these are nested under specific ad types
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent -= OnRewardedAdLoaded;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent -= OnRewardedAdLoadFailed;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent -= OnRewardedAdDisplayed;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent -= OnRewardedAdDisplayFailed;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent -= OnRewardedAdClicked;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent -= OnRewardedAdHidden;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= OnRewardedAdReceivedReward;

            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent -= OnInterstitialAdLoaded;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent -= OnInterstitialAdLoadFailed;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent -= OnInterstitialAdDisplayed;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent -= OnInterstitialAdDisplayFailed;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent -= OnInterstitialAdClicked;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent -= OnInterstitialAdHidden;

            MaxSdkCallbacks.Banner.OnAdLoadedEvent -= OnBannerAdLoaded;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent -= OnBannerAdLoadFailed;
            MaxSdkCallbacks.Banner.OnAdClickedEvent -= OnBannerAdClicked;
            MaxSdkCallbacks.Banner.OnAdExpandedEvent -= OnBannerAdExpanded;
            MaxSdkCallbacks.Banner.OnAdCollapsedEvent -= OnBannerAdCollapsed;

            MaxSdkCallbacks.AppOpen.OnAdLoadedEvent -= OnAppOpenAdLoaded;
            MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent -= OnAppOpenAdLoadFailed;
            MaxSdkCallbacks.AppOpen.OnAdDisplayedEvent -= OnAppOpenAdDisplayed;
            MaxSdkCallbacks.AppOpen.OnAdDisplayFailedEvent -= OnAppOpenAdDisplayFailed;
            MaxSdkCallbacks.AppOpen.OnAdClickedEvent -= OnAppOpenAdClicked;
            MaxSdkCallbacks.AppOpen.OnAdHiddenEvent -= OnAppOpenAdHidden;
        }

        /// <summary>
        /// Load initial ads
        /// </summary>
        private void LoadInitialAds()
        {
            if (!isInitialized) return;

            LoadRewardedAd();
            LoadInterstitialAd();
            LoadBannerAd();
            LoadAppOpenAd();
        }

        #region Ad Loading Methods

        /// <summary>
        /// Load rewarded ad
        /// </summary>
        public void LoadRewardedAd()
        {
            if (!isInitialized || string.IsNullOrEmpty(config.GetRewardedAdUnitId()))
            {
                Debug.LogWarning("Max SDK not initialized or rewarded ad unit ID not set");
                return;
            }

            Debug.Log("Loading Max rewarded ad...");
            MaxSdk.LoadRewardedAd(config.GetRewardedAdUnitId());
        }

        /// <summary>
        /// Load interstitial ad
        /// </summary>
        public void LoadInterstitialAd()
        {
            if (!isInitialized || string.IsNullOrEmpty(config.GetInterstitialAdUnitId()))
            {
                Debug.LogWarning("Max SDK not initialized or interstitial ad unit ID not set");
                return;
            }

            Debug.Log("Loading Max interstitial ad...");
            MaxSdk.LoadInterstitial(config.GetInterstitialAdUnitId());
        }

        /// <summary>
        /// Load banner ad
        /// </summary>
        public void LoadBannerAd()
        {
            if (!isInitialized || string.IsNullOrEmpty(config.GetBannerAdUnitId()))
            {
                Debug.LogWarning("Max SDK not initialized or banner ad unit ID not set");
                return;
            }

            Debug.Log("Loading Max banner ad...");
            MaxSdk.CreateBanner(config.GetBannerAdUnitId(), MaxSdkBase.BannerPosition.BottomCenter);
        }

        /// <summary>
        /// Load app open ad
        /// </summary>
        public void LoadAppOpenAd()
        {
            if (!isInitialized || string.IsNullOrEmpty(config.GetAppOpenAdUnitId()))
            {
                Debug.LogWarning("Max SDK not initialized or app open ad unit ID not set");
                return;
            }

            Debug.Log("Loading Max app open ad...");
            MaxSdk.LoadAppOpenAd(config.GetAppOpenAdUnitId());
        }

        #endregion

        #region Ad Display Methods

        /// <summary>
        /// Show rewarded ad
        /// </summary>
        public void ShowRewardedAd()
        {
            if (!isInitialized || string.IsNullOrEmpty(config.GetRewardedAdUnitId()))
            {
                Debug.LogWarning("Max SDK not initialized or rewarded ad unit ID not set");
                return;
            }

            if (MaxSdk.IsRewardedAdReady(config.GetRewardedAdUnitId()))
            {
                Debug.Log("Showing Max rewarded ad...");
                MaxSdk.ShowRewardedAd(config.GetRewardedAdUnitId());
            }
            else
            {
                Debug.Log("Max rewarded ad not ready, loading...");
                LoadRewardedAd();
            }
        }

        /// <summary>
        /// Show interstitial ad
        /// </summary>
        public void ShowInterstitialAd()
        {
            if (!isInitialized || string.IsNullOrEmpty(config.GetInterstitialAdUnitId()))
            {
                Debug.LogWarning("Max SDK not initialized or interstitial ad unit ID not set");
                return;
            }

            if (MaxSdk.IsInterstitialReady(config.GetInterstitialAdUnitId()))
            {
                Debug.Log("Showing Max interstitial ad...");
                MaxSdk.ShowInterstitial(config.GetInterstitialAdUnitId());
            }
            else
            {
                Debug.Log("Max interstitial ad not ready, loading...");
                LoadInterstitialAd();
            }
        }

        /// <summary>
        /// Show banner ad
        /// </summary>
        public void ShowBannerAd()
        {
            if (!isInitialized || string.IsNullOrEmpty(config.GetBannerAdUnitId()))
            {
                Debug.LogWarning("Max SDK not initialized or banner ad unit ID not set");
                return;
            }

            Debug.Log("Showing Max banner ad...");
            MaxSdk.ShowBanner(config.GetBannerAdUnitId());
        }

        /// <summary>
        /// Show app open ad
        /// </summary>
        public void ShowAppOpenAd()
        {
            if (!isInitialized || string.IsNullOrEmpty(config.GetAppOpenAdUnitId()))
            {
                Debug.LogWarning("Max SDK not initialized or app open ad unit ID not set");
                return;
            }

            if (MaxSdk.IsAppOpenAdReady(config.GetAppOpenAdUnitId()))
            {
                Debug.Log("Showing Max app open ad...");
                MaxSdk.ShowAppOpenAd(config.GetAppOpenAdUnitId());
            }
            else
            {
                Debug.Log("Max app open ad not ready, loading...");
                LoadAppOpenAd();
            }
        }

        #endregion

        #region Event Handlers

        // Rewarded Ad Events
        private void OnRewardedAdLoaded(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"Max Rewarded Ad loaded: {adUnitId}");
            isRewardedAdReady = true;
            OnAdLoaded?.Invoke(adUnitId);
        }

        private void OnRewardedAdLoadFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            Debug.LogWarning($"Max Rewarded Ad load failed: {adUnitId}, Error: {errorInfo.Message}");
            isRewardedAdReady = false;
            OnAdLoadFailed?.Invoke(adUnitId, errorInfo);
        }

        private void OnRewardedAdDisplayed(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"Max Rewarded Ad displayed: {adUnitId}");
            OnAdDisplayed?.Invoke(adUnitId);
        }

        private void OnRewardedAdDisplayFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            Debug.LogWarning($"Max Rewarded Ad display failed: {adUnitId}, Error: {errorInfo.Message}");
            OnAdDisplayFailed?.Invoke(adUnitId, errorInfo);
        }

        private void OnRewardedAdClicked(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"Max Rewarded Ad clicked: {adUnitId}");
            OnAdClicked?.Invoke(adUnitId);
        }

        private void OnRewardedAdHidden(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"Max Rewarded Ad hidden: {adUnitId}");
            isRewardedAdReady = false;
            OnAdHidden?.Invoke(adUnitId);
            LoadRewardedAd(); // Reload for next use
        }

        private void OnRewardedAdReceivedReward(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"Max Rewarded Ad reward received: {adUnitId}, Amount: {reward.Amount}, Label: {reward.Label}");
        }

        // Interstitial Ad Events
        private void OnInterstitialAdLoaded(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"Max Interstitial Ad loaded: {adUnitId}");
            isInterstitialAdReady = true;
            OnAdLoaded?.Invoke(adUnitId);
        }

        private void OnInterstitialAdLoadFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            Debug.LogWarning($"Max Interstitial Ad load failed: {adUnitId}, Error: {errorInfo.Message}");
            isInterstitialAdReady = false;
            OnAdLoadFailed?.Invoke(adUnitId, errorInfo);
        }

        private void OnInterstitialAdDisplayed(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"Max Interstitial Ad displayed: {adUnitId}");
            OnAdDisplayed?.Invoke(adUnitId);
        }

        private void OnInterstitialAdDisplayFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            Debug.LogWarning($"Max Interstitial Ad display failed: {adUnitId}, Error: {errorInfo.Message}");
            OnAdDisplayFailed?.Invoke(adUnitId, errorInfo);
        }

        private void OnInterstitialAdClicked(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"Max Interstitial Ad clicked: {adUnitId}");
            OnAdClicked?.Invoke(adUnitId);
        }

        private void OnInterstitialAdHidden(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"Max Interstitial Ad hidden: {adUnitId}");
            isInterstitialAdReady = false;
            OnAdHidden?.Invoke(adUnitId);
            LoadInterstitialAd(); // Reload for next use
        }

        // Banner Ad Events
        private void OnBannerAdLoaded(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"Max Banner Ad loaded: {adUnitId}");
            isBannerAdReady = true;
            OnAdLoaded?.Invoke(adUnitId);
        }

        private void OnBannerAdLoadFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            Debug.LogWarning($"Max Banner Ad load failed: {adUnitId}, Error: {errorInfo.Message}");
            isBannerAdReady = false;
            OnAdLoadFailed?.Invoke(adUnitId, errorInfo);
        }

        private void OnBannerAdClicked(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"Max Banner Ad clicked: {adUnitId}");
            OnAdClicked?.Invoke(adUnitId);
        }

        private void OnBannerAdExpanded(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"Max Banner Ad expanded: {adUnitId}");
            OnAdExpanded?.Invoke(adUnitId);
        }

        private void OnBannerAdCollapsed(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"Max Banner Ad collapsed: {adUnitId}");
            OnAdCollapsed?.Invoke(adUnitId);
        }

        // App Open Ad Events
        private void OnAppOpenAdLoaded(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"Max App Open Ad loaded: {adUnitId}");
            isAppOpenAdReady = true;
            OnAdLoaded?.Invoke(adUnitId);
        }

        private void OnAppOpenAdLoadFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            Debug.LogWarning($"Max App Open Ad load failed: {adUnitId}, Error: {errorInfo.Message}");
            isAppOpenAdReady = false;
            OnAdLoadFailed?.Invoke(adUnitId, errorInfo);
        }

        private void OnAppOpenAdDisplayed(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"Max App Open Ad displayed: {adUnitId}");
            OnAdDisplayed?.Invoke(adUnitId);
        }

        private void OnAppOpenAdDisplayFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            Debug.LogWarning($"Max App Open Ad display failed: {adUnitId}, Error: {errorInfo.Message}");
            OnAdDisplayFailed?.Invoke(adUnitId, errorInfo);
        }

        private void OnAppOpenAdClicked(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"Max App Open Ad clicked: {adUnitId}");
            OnAdClicked?.Invoke(adUnitId);
        }

        private void OnAppOpenAdHidden(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log($"Max App Open Ad hidden: {adUnitId}");
            isAppOpenAdReady = false;
            OnAdHidden?.Invoke(adUnitId);
            LoadAppOpenAd(); // Reload for next use
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Check if Max SDK is initialized
        /// </summary>
        public bool IsInitialized => isInitialized;

        /// <summary>
        /// Check if configuration is valid
        /// </summary>
        public bool IsConfigValid => config != null && config.IsValid();

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
        /// Check if app open ad is ready
        /// </summary>
        public bool IsAppOpenAdReady => isAppOpenAdReady;

        /// <summary>
        /// Get initialization status for debugging
        /// </summary>
        public string GetInitializationStatus()
        {
            return $"Max SDK - Initialized: {isInitialized}, Config Valid: {IsConfigValid}";
        }

        /// <summary>
        /// Log configuration status for debugging
        /// </summary>
        public void LogConfigurationStatus()
        {
            if (config != null)
            {
                Debug.Log(config.GetConfigStatus());
            }
            else
            {
                Debug.LogWarning("MaxConfig is not assigned!");
            }
        }

        #endregion
    }
}
