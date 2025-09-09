using System;
using System.Collections.Generic;
using UnityEngine;
using SolarEngineMeditationSample.Wrappers.Taku;
using AnyThinkAds.Api;

namespace SolarEngineMeditationSample.Taku
{
    /// <summary>
    /// Singleton manager for Taku SDK integration
    /// Handles initialization, ad loading, and lifecycle management
    /// </summary>
    public class TakuManager : MonoBehaviour, ATRewardedVideoListener, ATInterstitialAdListener, ATBannerAdListener, ATNativeAdListener
    {
        private static TakuManager _instance;
        public static TakuManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<TakuManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("TakuManager");
                        _instance = go.AddComponent<TakuManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        [Header("Configuration")]
        [SerializeField] private TakuConfig config;

        // Ad instances
        private bool isInitialized = false;
        private bool isRewardedAdReady = false;
        private bool isInterstitialAdReady = false;
        private bool isBannerAdReady = false;
        private bool isNativeAdReady = false;

        // Public properties for external access
        public bool IsInitialized => isInitialized;

        // Events
        public event Action OnTakuInitialized;
        public event Action<string> OnTakuInitializationFailed;
        public event Action<string, object> OnAdRevenuePaid;

        // Ad ready events
        public event Action OnRewardedAdReady;
        public event Action OnInterstitialAdReady;
        public event Action OnBannerAdReady;
        public event Action OnNativeAdReady;

        // Ad load failed events
        public event Action<string> OnRewardedAdLoadFailed;
        public event Action<string> OnInterstitialAdLoadFailed;
        public event Action<string> OnBannerAdLoadFailed;
        public event Action<string> OnNativeAdLoadFailed;

        // Ad display events
        public event Action OnRewardedAdDisplayed;
        public event Action OnInterstitialAdDisplayed;
        public event Action OnBannerAdDisplayed;
        public event Action OnNativeAdDisplayed;

        // Ad close events
        public event Action OnRewardedAdClosed;
        public event Action OnInterstitialAdClosed;
        public event Action OnBannerAdClosed;
        public event Action OnNativeAdClosed;

        // Ad click events
        public event Action OnRewardedAdClicked;
        public event Action OnInterstitialAdClicked;
        public event Action OnBannerAdClicked;
        public event Action OnNativeAdClicked;

        // Reward events
        public event Action OnRewardedAdRewarded;

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
            UnsubscribeFromEvents();
        }

        /// <summary>
        /// Setup configuration and initialize Taku SDK
        /// </summary>
        private void SetupConfigAndInitialize()
        {
            // Find or create TakuConfig (consistent with other SDKs pattern)
            config = Resources.Load<TakuConfig>("TakuConfig");
            if (config == null)
            {
                Debug.LogWarning("TakuConfig not found in Resources folder. Creating default config.");
                config = ScriptableObject.CreateInstance<TakuConfig>();
                config.name = "TakuConfig";
                
                // Set some sensible defaults
                config.SetDefaultValues();
                
                Debug.Log("TakuConfig created automatically. Please configure App ID, App Key, and ad unit IDs in the inspector.");
            }

            SetConfig(config);
        }

        /// <summary>
        /// Set configuration and initialize if valid
        /// </summary>
        /// <param name="takuConfig">Taku configuration</param>
        public void SetConfig(TakuConfig takuConfig)
        {
            if (takuConfig == null)
            {
                Debug.LogError("TakuConfig is null! Cannot initialize Taku SDK.");
                return;
            }
            
            config = takuConfig;
            
            if (config.IsValid())
            {
                InitializeTaku();
            }
            else
            {
                Debug.LogWarning("TakuConfig is invalid - App ID or App Key not set. Taku SDK will not initialize.");
                Debug.LogWarning("Please set your Taku App ID and App Key in the TakuConfig asset to enable ads.");
                Debug.LogWarning("You can find the config asset in the Project window under Resources/TakuConfig.asset");
            }
        }

        /// <summary>
        /// Initialize Taku SDK
        /// </summary>
        private void InitializeTaku()
        {
            if (isInitialized)
            {
                Debug.Log("Taku SDK already initialized.");
                return;
            }

            Debug.Log($"TakuManager: Initializing Taku SDK with App ID: {config.AppId}");

            try
            {
                // Initialize AnyThink SDK (which includes Taku)
                ATSDKAPI.initSDK(config.AppId, config.AppKey);
                
                // Set debug mode
                if (config.EnableDebugLogging)
                {
                    ATSDKAPI.showDebuggerUI();
                    Debug.Log("TakuManager: Debug mode enabled");
                }

                // Subscribe to events
                SubscribeToEvents();

                // Mark as initialized
                isInitialized = true;
                Debug.Log("TakuManager: Taku SDK initialized successfully");

                // Trigger initialization event
                OnTakuInitialized?.Invoke();

                // Load ads
                LoadAds();
            }
            catch (Exception e)
            {
                Debug.LogError($"TakuManager: Failed to initialize Taku SDK: {e.Message}");
                OnTakuInitializationFailed?.Invoke(e.Message);
            }
        }

        /// <summary>
        /// Subscribe to Taku SDK events
        /// </summary>
        private void SubscribeToEvents()
        {
            Debug.Log("TakuManager: Subscribing to Taku SDK events");

            // Note: AnyThink SDK uses listener interfaces for events
            // We'll implement these listeners in the ad loading methods
            // The wrappers will be used when we implement the revenue tracking
            
            Debug.Log("TakuManager: Event subscription will be handled by listener interfaces");
        }

        /// <summary>
        /// Unsubscribe from Taku SDK events
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            Debug.Log("TakuManager: Unsubscribing from Taku SDK events");

            // Note: AnyThink SDK uses listener interfaces for events
            // No explicit unsubscription needed as listeners are managed per ad instance
            Debug.Log("TakuManager: No explicit unsubscription needed for listener interfaces");
        }

        /// <summary>
        /// Load all ad types
        /// </summary>
        private void LoadAds()
        {
            if (!isInitialized)
            {
                Debug.LogWarning("TakuManager: Cannot load ads - SDK not initialized");
                return;
            }

            Debug.Log("TakuManager: Loading ads");

            LoadRewardedAd();
            LoadInterstitialAd();
            LoadBannerAd();
            LoadNativeAd();
        }

        /// <summary>
        /// Load rewarded ad
        /// </summary>
        public void LoadRewardedAd()
        {
            if (!isInitialized)
            {
                Debug.LogWarning("TakuManager: Cannot load rewarded ad - SDK not initialized");
                return;
            }

            string adUnitId = config.GetRewardedAdUnitId();
            if (string.IsNullOrEmpty(adUnitId))
            {
                Debug.LogWarning("TakuManager: Rewarded ad unit ID not configured");
                return;
            }

            Debug.Log($"TakuManager: Loading rewarded ad with unit ID: {adUnitId}");

            try
            {
                // Load rewarded ad using AnyThink SDK
                ATRewardedVideo.Instance.loadVideoAd(adUnitId, new Dictionary<string, string>());
                Debug.Log($"TakuManager: Rewarded ad load requested for {adUnitId}");
            }
            catch (Exception e)
            {
                Debug.LogError($"TakuManager: Failed to load rewarded ad: {e.Message}");
            }
        }

        /// <summary>
        /// Load interstitial ad
        /// </summary>
        public void LoadInterstitialAd()
        {
            if (!isInitialized)
            {
                Debug.LogWarning("TakuManager: Cannot load interstitial ad - SDK not initialized");
                return;
            }

            string adUnitId = config.GetInterstitialAdUnitId();
            if (string.IsNullOrEmpty(adUnitId))
            {
                Debug.LogWarning("TakuManager: Interstitial ad unit ID not configured");
                return;
            }

            Debug.Log($"TakuManager: Loading interstitial ad with unit ID: {adUnitId}");

            try
            {
                // TODO: Load interstitial ad
                // TakuSDK.LoadInterstitialAd(adUnitId);
            }
            catch (Exception e)
            {
                Debug.LogError($"TakuManager: Failed to load interstitial ad: {e.Message}");
            }
        }

        /// <summary>
        /// Load banner ad
        /// </summary>
        public void LoadBannerAd()
        {
            if (!isInitialized)
            {
                Debug.LogWarning("TakuManager: Cannot load banner ad - SDK not initialized");
                return;
            }

            string adUnitId = config.GetBannerAdUnitId();
            if (string.IsNullOrEmpty(adUnitId))
            {
                Debug.LogWarning("TakuManager: Banner ad unit ID not configured");
                return;
            }

            Debug.Log($"TakuManager: Loading banner ad with unit ID: {adUnitId}");

            try
            {
                // TODO: Load banner ad
                // TakuSDK.LoadBannerAd(adUnitId);
            }
            catch (Exception e)
            {
                Debug.LogError($"TakuManager: Failed to load banner ad: {e.Message}");
            }
        }

        /// <summary>
        /// Load native ad
        /// </summary>
        public void LoadNativeAd()
        {
            if (!isInitialized)
            {
                Debug.LogWarning("TakuManager: Cannot load native ad - SDK not initialized");
                return;
            }

            string adUnitId = config.GetNativeAdUnitId();
            if (string.IsNullOrEmpty(adUnitId))
            {
                Debug.LogWarning("TakuManager: Native ad unit ID not configured");
                return;
            }

            Debug.Log($"TakuManager: Loading native ad with unit ID: {adUnitId}");

            try
            {
                // TODO: Load native ad
                // TakuSDK.LoadNativeAd(adUnitId);
            }
            catch (Exception e)
            {
                Debug.LogError($"TakuManager: Failed to load native ad: {e.Message}");
            }
        }

        /// <summary>
        /// Show rewarded ad
        /// </summary>
        public void ShowRewardedAd()
        {
            if (!isInitialized)
            {
                Debug.LogWarning("TakuManager: Cannot show rewarded ad - SDK not initialized");
                return;
            }

            string adUnitId = config.GetRewardedAdUnitId();
            if (string.IsNullOrEmpty(adUnitId))
            {
                Debug.LogWarning("TakuManager: Rewarded ad unit ID not configured");
                return;
            }

            if (!isRewardedAdReady)
            {
                Debug.LogWarning("TakuManager: Rewarded ad not ready, loading...");
                LoadRewardedAd();
                return;
            }

            Debug.Log("TakuManager: Showing rewarded ad");

            try
            {
                // Show rewarded ad using AnyThink SDK
                ATRewardedVideo.Instance.showAd(adUnitId);
                Debug.Log($"TakuManager: Rewarded ad show requested for {adUnitId}");
            }
            catch (Exception e)
            {
                Debug.LogError($"TakuManager: Failed to show rewarded ad: {e.Message}");
            }
        }

        /// <summary>
        /// Show interstitial ad
        /// </summary>
        public void ShowInterstitialAd()
        {
            if (!isInitialized)
            {
                Debug.LogWarning("TakuManager: Cannot show interstitial ad - SDK not initialized");
                return;
            }

            if (!isInterstitialAdReady)
            {
                Debug.LogWarning("TakuManager: Interstitial ad not ready, loading...");
                LoadInterstitialAd();
                return;
            }

            Debug.Log("TakuManager: Showing interstitial ad");

            try
            {
                // TODO: Show interstitial ad
                // TakuSDK.ShowInterstitialAd();
            }
            catch (Exception e)
            {
                Debug.LogError($"TakuManager: Failed to show interstitial ad: {e.Message}");
            }
        }

        /// <summary>
        /// Show banner ad
        /// </summary>
        public void ShowBannerAd()
        {
            if (!isInitialized)
            {
                Debug.LogWarning("TakuManager: Cannot show banner ad - SDK not initialized");
                return;
            }

            if (!isBannerAdReady)
            {
                Debug.LogWarning("TakuManager: Banner ad not ready, loading...");
                LoadBannerAd();
                return;
            }

            Debug.Log("TakuManager: Showing banner ad");

            try
            {
                // TODO: Show banner ad
                // TakuSDK.ShowBannerAd();
            }
            catch (Exception e)
            {
                Debug.LogError($"TakuManager: Failed to show banner ad: {e.Message}");
            }
        }

        /// <summary>
        /// Show native ad
        /// </summary>
        public void ShowNativeAd()
        {
            if (!isInitialized)
            {
                Debug.LogWarning("TakuManager: Cannot show native ad - SDK not initialized");
                return;
            }

            if (!isNativeAdReady)
            {
                Debug.LogWarning("TakuManager: Native ad not ready, loading...");
                LoadNativeAd();
                return;
            }

            Debug.Log("TakuManager: Showing native ad");

            try
            {
                // TODO: Show native ad
                // TakuSDK.ShowNativeAd();
            }
            catch (Exception e)
            {
                Debug.LogError($"TakuManager: Failed to show native ad: {e.Message}");
            }
        }

        /// <summary>
        /// Check if rewarded ad is ready
        /// </summary>
        public bool IsRewardedAdReady()
        {
            if (!isInitialized)
                return false;

            string adUnitId = config.GetRewardedAdUnitId();
            if (string.IsNullOrEmpty(adUnitId))
                return false;

            // Check if rewarded ad is ready using AnyThink SDK
            return ATRewardedVideo.Instance.hasAdReady(adUnitId);
        }

        /// <summary>
        /// Check if interstitial ad is ready
        /// </summary>
        public bool IsInterstitialAdReady()
        {
            return isInitialized && isInterstitialAdReady;
        }

        /// <summary>
        /// Check if banner ad is ready
        /// </summary>
        public bool IsBannerAdReady()
        {
            return isInitialized && isBannerAdReady;
        }

        /// <summary>
        /// Check if native ad is ready
        /// </summary>
        public bool IsNativeAdReady()
        {
            return isInitialized && isNativeAdReady;
        }

        /// <summary>
        /// Get configuration status
        /// </summary>
        public string GetConfigStatus()
        {
            if (config == null)
                return "TakuConfig: NOT ASSIGNED";
            
            return config.GetConfigStatus();
        }

        /// <summary>
        /// Check if the manager is valid
        /// </summary>
        public bool IsValid()
        {
            return config != null && config.IsValid();
        }

        /// <summary>
        /// Get initialization status
        /// </summary>
        public string GetInitializationStatus()
        {
            return $"TakuManager: Initialized={isInitialized}, " +
                   $"Rewarded Ready={isRewardedAdReady}, " +
                   $"Interstitial Ready={isInterstitialAdReady}, " +
                   $"Banner Ready={isBannerAdReady}, " +
                   $"Native Ready={isNativeAdReady}";
        }

        /// <summary>
        /// Handle ad revenue events from wrappers
        /// </summary>
        /// <param name="adUnitId">Ad unit identifier</param>
        /// <param name="adInfo">Ad info from Taku SDK</param>
        private void HandleAdRevenuePaid(string adUnitId, object adInfo)
        {
            Debug.Log($"TakuManager: Ad revenue event received for {adUnitId}");
            
            // Trigger the public event for external listeners
            OnAdRevenuePaid?.Invoke(adUnitId, adInfo);
            
            // Additional revenue handling logic can be added here
            // For example, analytics, user rewards, etc.
        }

        #region ATRewardedVideoListener Implementation

        public void onRewardedVideoAdLoaded(string placementId)
        {
            Debug.Log($"TakuManager: Rewarded video ad loaded for placement: {placementId}");
            isRewardedAdReady = true;
            OnRewardedAdReady?.Invoke();
        }

        public void onRewardedVideoAdLoadFail(string placementId, string code, string message)
        {
            Debug.LogError($"TakuManager: Rewarded video ad load failed for placement: {placementId}, code: {code}, message: {message}");
            isRewardedAdReady = false;
            OnRewardedAdLoadFailed?.Invoke(message);
        }

        public void onRewardedVideoAdPlayStart(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log($"TakuManager: Rewarded video ad play started for placement: {placementId}");
            OnRewardedAdDisplayed?.Invoke();
            
            // Track revenue using wrapper
            TakuAdWrapper.TrackRewardedAdRevenue(callbackInfo);
        }

        public void onRewardedVideoAdPlayEnd(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log($"TakuManager: Rewarded video ad play ended for placement: {placementId}");
        }

        public void onRewardedVideoAdPlayFail(string placementId, string code, string message)
        {
            Debug.LogError($"TakuManager: Rewarded video ad play failed for placement: {placementId}, code: {code}, message: {message}");
        }

        public void onRewardedVideoAdPlayClosed(string placementId, bool isReward, ATCallbackInfo callbackInfo)
        {
            Debug.Log($"TakuManager: Rewarded video ad closed for placement: {placementId}, reward: {isReward}");
            OnRewardedAdClosed?.Invoke();
            
            if (isReward)
            {
                OnRewardedAdRewarded?.Invoke();
            }
        }

        public void onRewardedVideoAdPlayClicked(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log($"TakuManager: Rewarded video ad clicked for placement: {placementId}");
            OnRewardedAdClicked?.Invoke();
        }

        public void onReward(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log($"TakuManager: Reward earned for placement: {placementId}");
            OnRewardedAdRewarded?.Invoke();
        }

        public void startLoadingADSource(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log($"TakuManager: Started loading AD source for placement: {placementId}");
        }

        public void finishLoadingADSource(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log($"TakuManager: Finished loading AD source for placement: {placementId}");
        }

        public void failToLoadADSource(string placementId, ATCallbackInfo callbackInfo, string code, string message)
        {
            Debug.LogError($"TakuManager: Failed to load AD source for placement: {placementId}, code: {code}, message: {message}");
        }

        public void startBiddingADSource(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log($"TakuManager: Started bidding AD source for placement: {placementId}");
        }

        public void finishBiddingADSource(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log($"TakuManager: Finished bidding AD source for placement: {placementId}");
        }

        public void failBiddingADSource(string placementId, ATCallbackInfo callbackInfo, string code, string message)
        {
            Debug.LogError($"TakuManager: Failed bidding AD source for placement: {placementId}, code: {code}, message: {message}");
        }

        #endregion

        #region ATInterstitialAdListener Implementation

        public void onInterstitialAdLoad(string placementId)
        {
            Debug.Log($"TakuManager: Interstitial ad loaded for placement: {placementId}");
            isInterstitialAdReady = true;
            OnInterstitialAdReady?.Invoke();
        }

        public void onInterstitialAdLoadFail(string placementId, string code, string message)
        {
            Debug.LogError($"TakuManager: Interstitial ad load failed for placement: {placementId}, code: {code}, message: {message}");
            isInterstitialAdReady = false;
            OnInterstitialAdLoadFailed?.Invoke(message);
        }

        public void onInterstitialAdShow(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log($"TakuManager: Interstitial ad shown for placement: {placementId}");
            OnInterstitialAdDisplayed?.Invoke();
            
            // Track revenue using wrapper
            TakuAdWrapper.TrackInterstitialAdRevenue(callbackInfo);
        }

        public void onInterstitialAdFailedToShow(string placementId)
        {
            Debug.LogError($"TakuManager: Interstitial ad failed to show for placement: {placementId}");
        }

        public void onInterstitialAdClose(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log($"TakuManager: Interstitial ad closed for placement: {placementId}");
            OnInterstitialAdClosed?.Invoke();
        }

        public void onInterstitialAdClick(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log($"TakuManager: Interstitial ad clicked for placement: {placementId}");
            OnInterstitialAdClicked?.Invoke();
        }

        public void onInterstitialAdStartPlayingVideo(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log($"TakuManager: Interstitial ad video started for placement: {placementId}");
        }

        public void onInterstitialAdEndPlayingVideo(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log($"TakuManager: Interstitial ad video ended for placement: {placementId}");
        }

        public void onInterstitialAdFailedToPlayVideo(string placementId, string code, string message)
        {
            Debug.LogError($"TakuManager: Interstitial ad video failed for placement: {placementId}, code: {code}, message: {message}");
        }

        #endregion

        #region ATBannerAdListener Implementation

        public void onAdLoad(string placementId)
        {
            Debug.Log($"TakuManager: Banner ad loaded for placement: {placementId}");
            isBannerAdReady = true;
            OnBannerAdReady?.Invoke();
        }

        void ATBannerAdListener.onAdLoadFail(string placementId, string code, string message)
        {
            Debug.LogError($"TakuManager: Banner ad load failed for placement: {placementId}, code: {code}, message: {message}");
            isBannerAdReady = false;
            OnBannerAdLoadFailed?.Invoke(message);
        }

        public void onAdImpress(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log($"TakuManager: Banner ad impressed for placement: {placementId}");
            OnBannerAdDisplayed?.Invoke();
            
            // Track revenue using wrapper
            TakuAdWrapper.TrackBannerAdRevenue(callbackInfo);
        }

        public void onAdClick(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log($"TakuManager: Banner ad clicked for placement: {placementId}");
            OnBannerAdClicked?.Invoke();
        }

        public void onAdAutoRefresh(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log($"TakuManager: Banner ad auto refreshed for placement: {placementId}");
        }

        public void onAdAutoRefreshFail(string placementId, string code, string message)
        {
            Debug.LogError($"TakuManager: Banner ad auto refresh failed for placement: {placementId}, code: {code}, message: {message}");
        }

        public void onAdClose(string placementId)
        {
            Debug.Log($"TakuManager: Banner ad closed for placement: {placementId}");
            OnBannerAdClosed?.Invoke();
        }

        public void onAdCloseButtonTapped(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log($"TakuManager: Banner ad close button tapped for placement: {placementId}");
        }

        #endregion

        #region ATNativeAdListener Implementation

        public void onAdLoaded(string placementId)
        {
            Debug.Log($"TakuManager: Native ad loaded for placement: {placementId}");
            isNativeAdReady = true;
            OnNativeAdReady?.Invoke();
        }

        void ATNativeAdListener.onAdLoadFail(string placementId, string code, string message)
        {
            Debug.LogError($"TakuManager: Native ad load failed for placement: {placementId}, code: {code}, message: {message}");
            isNativeAdReady = false;
            OnNativeAdLoadFailed?.Invoke(message);
        }

        public void onAdImpressed(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log($"TakuManager: Native ad impressed for placement: {placementId}");
            OnNativeAdDisplayed?.Invoke();
            
            // Track revenue using wrapper
            TakuAdWrapper.TrackNativeAdRevenue(callbackInfo);
        }

        public void onAdClicked(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log($"TakuManager: Native ad clicked for placement: {placementId}");
            OnNativeAdClicked?.Invoke();
        }

        public void onAdVideoStart(string placementId)
        {
            Debug.Log($"TakuManager: Native ad video started for placement: {placementId}");
        }

        public void onAdVideoEnd(string placementId)
        {
            Debug.Log($"TakuManager: Native ad video ended for placement: {placementId}");
        }

        public void onAdVideoProgress(string placementId, int progress)
        {
            Debug.Log($"TakuManager: Native ad video progress for placement: {placementId}, progress: {progress}");
        }

        public void onAdCloseButtonClicked(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log($"TakuManager: Native ad close button clicked for placement: {placementId}");
        }

        #endregion
    }
}
