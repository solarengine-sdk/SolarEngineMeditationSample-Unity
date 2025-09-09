using System;
using System.Collections.Generic;
using UnityEngine;
using ByteDance.Union;
using ByteDance.Union.Mediation;
using SolarEngineMeditationSample.Wrappers.Gromore;

namespace SolarEngineMeditationSample.Gromore
{
    /// <summary>
    /// Gromore (ByteDance Union) SDK manager for Unity
    /// Handles initialization, ad loading, and ad display for Gromore SDK
    /// Implements listener interfaces for event handling
    /// </summary>
    public class GromoreManager : MonoBehaviour, IRewardVideoAdListener, IRewardAdInteractionListener, 
                                  IFullScreenVideoAdListener, IFullScreenVideoAdInteractionListener, IFeedAdListener, IFeedAdInteractionListener, INativeAdListener
    {
        // Singleton instance
        private static GromoreManager _instance;
        public static GromoreManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GromoreManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("GromoreManager");
                        _instance = go.AddComponent<GromoreManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        [Header("Configuration")]
        [SerializeField] private GromoreConfig config;

        // Internal state
        private bool isInitialized = false;
        private AdNative adNative;
        private Dictionary<string, bool> adReadyFlags = new Dictionary<string, bool>();

        // Events
        public event Action OnGromoreInitialized;
        public event Action<string> OnGromoreInitializationFailed;
        public event Action<string, MediationAdEcpmInfo> OnAdRevenuePaid;

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
            // Cleanup if needed
        }

        /// <summary>
        /// Setup configuration and initialize Gromore SDK
        /// </summary>
        private void SetupConfigAndInitialize()
        {
            Debug.Log("GromoreManager: Setting up configuration and initializing...");

            // Try to load config from Resources
            if (config == null)
            {
                config = Resources.Load<GromoreConfig>("GromoreConfig");
            }

            // If still no config, create a default one
            if (config == null)
            {
                Debug.LogWarning("GromoreConfig not found in Resources folder. Creating default config.");
                config = ScriptableObject.CreateInstance<GromoreConfig>();
                config.SetDefaultValues();
            }

            if (config.IsValid())
            {
                SetConfig(config);
            }
            else
            {
                Debug.LogError($"GromoreConfig is invalid: {config.GetConfigStatus()}");
                OnGromoreInitializationFailed?.Invoke("Invalid configuration");
            }
        }

        /// <summary>
        /// Set configuration and initialize Gromore SDK
        /// </summary>
        /// <param name="newConfig">Gromore configuration</param>
        public void SetConfig(GromoreConfig newConfig)
        {
            if (newConfig == null)
            {
                Debug.LogError("GromoreManager: Cannot set null config");
                return;
            }

            if (!newConfig.IsValid())
            {
                Debug.LogError($"GromoreManager: Invalid config: {newConfig.GetConfigStatus()}");
                return;
            }

            config = newConfig;
            Debug.Log($"GromoreManager: Config set successfully: {config.GetConfigStatus()}");

            InitializeGromore();
        }

        /// <summary>
        /// Initialize Gromore SDK with current configuration
        /// </summary>
        private void InitializeGromore()
        {
            if (isInitialized)
            {
                Debug.Log("GromoreManager: SDK already initialized");
                return;
            }

            try
            {
                Debug.Log($"GromoreManager: Initializing Gromore SDK with App ID: {config.AppId}");

                // Create AdNative instance
                adNative = SDK.CreateAdNative();

                isInitialized = true;
                Debug.Log("GromoreManager: Gromore SDK initialized successfully");
                OnGromoreInitialized?.Invoke();

                // Load initial ads
                LoadAds();
            }
            catch (Exception e)
            {
                Debug.LogError($"GromoreManager: Failed to initialize Gromore SDK: {e.Message}");
                OnGromoreInitializationFailed?.Invoke(e.Message);
            }
        }

        /// <summary>
        /// Load all ad types
        /// </summary>
        public void LoadAds()
        {
            if (!isInitialized)
            {
                Debug.LogWarning("GromoreManager: Cannot load ads - SDK not initialized");
                return;
            }

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
            if (!isInitialized || string.IsNullOrEmpty(config.RewardedAdUnitId))
                return;

            try
            {
                var adSlot = new AdSlot.Builder()
                    .SetCodeId(config.RewardedAdUnitId)
                    .SetAdCount(1)
                    .Build();

                adNative.LoadRewardVideoAd(adSlot, this);
                Debug.Log($"GromoreManager: Loading rewarded ad for unit: {config.RewardedAdUnitId}");
            }
            catch (Exception e)
            {
                Debug.LogError($"GromoreManager: Failed to load rewarded ad: {e.Message}");
            }
        }

        /// <summary>
        /// Load interstitial ad
        /// </summary>
        public void LoadInterstitialAd()
        {
            if (!isInitialized || string.IsNullOrEmpty(config.InterstitialAdUnitId))
                return;

            try
            {
                var adSlot = new AdSlot.Builder()
                    .SetCodeId(config.InterstitialAdUnitId)
                    .SetAdCount(1)
                    .Build();

                adNative.LoadFullScreenVideoAd(adSlot, this);
                Debug.Log($"GromoreManager: Loading interstitial ad for unit: {config.InterstitialAdUnitId}");
            }
            catch (Exception e)
            {
                Debug.LogError($"GromoreManager: Failed to load interstitial ad: {e.Message}");
            }
        }

        /// <summary>
        /// Load banner ad
        /// </summary>
        public void LoadBannerAd()
        {
            if (!isInitialized || string.IsNullOrEmpty(config.BannerAdUnitId))
                return;

            try
            {
                var adSlot = new AdSlot.Builder()
                    .SetCodeId(config.BannerAdUnitId)
                    .SetAdCount(1)
                    .Build();

                adNative.LoadFeedAd(adSlot, this);
                Debug.Log($"GromoreManager: Loading banner ad for unit: {config.BannerAdUnitId}");
            }
            catch (Exception e)
            {
                Debug.LogError($"GromoreManager: Failed to load banner ad: {e.Message}");
            }
        }

        /// <summary>
        /// Load native ad
        /// </summary>
        public void LoadNativeAd()
        {
            if (!isInitialized || string.IsNullOrEmpty(config.NativeAdUnitId))
                return;

            try
            {
                var adSlot = new AdSlot.Builder()
                    .SetCodeId(config.NativeAdUnitId)
                    .SetAdCount(1)
                    .Build();

                adNative.LoadNativeAd(adSlot, this);
                Debug.Log($"GromoreManager: Loading native ad for unit: {config.NativeAdUnitId}");
            }
            catch (Exception e)
            {
                Debug.LogError($"GromoreManager: Failed to load native ad: {e.Message}");
            }
        }

        /// <summary>
        /// Check if rewarded ad is ready
        /// </summary>
        public bool IsRewardedAdReady()
        {
            return isInitialized && adReadyFlags.ContainsKey(config.RewardedAdUnitId) && adReadyFlags[config.RewardedAdUnitId];
        }

        /// <summary>
        /// Check if interstitial ad is ready
        /// </summary>
        public bool IsInterstitialAdReady()
        {
            return isInitialized && adReadyFlags.ContainsKey(config.InterstitialAdUnitId) && adReadyFlags[config.InterstitialAdUnitId];
        }

        /// <summary>
        /// Check if banner ad is ready
        /// </summary>
        public bool IsBannerAdReady()
        {
            return isInitialized && adReadyFlags.ContainsKey(config.BannerAdUnitId) && adReadyFlags[config.BannerAdUnitId];
        }

        /// <summary>
        /// Check if native ad is ready
        /// </summary>
        public bool IsNativeAdReady()
        {
            return isInitialized && adReadyFlags.ContainsKey(config.NativeAdUnitId) && adReadyFlags[config.NativeAdUnitId];
        }

        /// <summary>
        /// Get configuration status
        /// </summary>
        public string GetConfigStatus()
        {
            if (config == null)
                return "GromoreConfig: NOT ASSIGNED";
            
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
            var status = $"GromoreManager: Initialized={isInitialized}";
            foreach (var kvp in adReadyFlags)
            {
                status += $", {kvp.Key}={kvp.Value}";
            }
            return status;
        }

        #region IRewardVideoAdListener Implementation

        void IRewardVideoAdListener.OnError(int code, string message)
        {
            Debug.LogError($"GromoreManager: Rewarded video ad error - Code: {code}, Message: {message}");
            OnRewardedAdLoadFailed?.Invoke(message);
        }

        public void OnRewardVideoAdLoad(RewardVideoAd ad)
        {
            Debug.Log("GromoreManager: Rewarded video ad loaded successfully");
            if (config != null)
            {
                adReadyFlags[config.RewardedAdUnitId] = true;
            }
            OnRewardedAdReady?.Invoke();
        }

        public void OnRewardVideoCached()
        {
            Debug.Log("GromoreManager: Rewarded video ad cached");
        }

        public void OnRewardVideoCached(RewardVideoAd ad)
        {
            Debug.Log("GromoreManager: Rewarded video ad cached with ad instance");
            rewardedAd = ad;
        }

        #endregion
private RewardVideoAd rewardedAd;

        #region IRewardAdInteractionListener Implementation

        void IRewardAdInteractionListener.OnAdShow()
        {
            Debug.Log("GromoreManager: Rewarded ad shown");
            
            string placementId = "";
            if (config != null)
            {
                placementId = config.RewardedAdUnitId;
            }
            // Use wrapper to track ad revenue and impression
            MediationAdEcpmInfo ecpmInfo = null;
            if (rewardedAd != null)
            {
                ecpmInfo = rewardedAd.GetMediationManager().GetShowEcpm();
            }
            GromoreAdWrapper.TrackRewardedAdRevenue(placementId, ecpmInfo);
            
            OnRewardedAdDisplayed?.Invoke();
        }

        void IRewardAdInteractionListener.OnAdVideoBarClick()
        {
            Debug.Log("GromoreManager: Rewarded ad video bar clicked");
        }

        void IRewardAdInteractionListener.OnAdClose()
        {
            Debug.Log("GromoreManager: Rewarded ad closed");
            OnRewardedAdClosed?.Invoke();
        }

        void IRewardAdInteractionListener.OnVideoComplete()
        {
            Debug.Log("GromoreManager: Rewarded ad video completed");
        }

        void IRewardAdInteractionListener.OnVideoSkip()
        {
            Debug.Log("GromoreManager: Rewarded ad video skipped");
        }

        void IRewardAdInteractionListener.OnVideoError()
        {
            Debug.LogError("GromoreManager: Rewarded ad video error");
        }

        public void OnRewardArrived(bool isRewardValid, int rewardType, IRewardBundleModel extraInfo)
        {
            Debug.Log($"GromoreManager: Reward arrived - Valid: {isRewardValid}, Type: {rewardType}");

            if (isRewardValid)
            {
                OnRewardedAdRewarded?.Invoke();
            }
        }

        #endregion

        #region IFullScreenVideoAdListener Implementation

        void IFullScreenVideoAdListener.OnError(int code, string message)
        {
            Debug.LogError($"GromoreManager: Full screen video ad error - Code: {code}, Message: {message}");
            OnInterstitialAdLoadFailed?.Invoke(message);
        }

        public void OnFullScreenVideoAdLoad(FullScreenVideoAd ad)
        {
            Debug.Log("GromoreManager: Full screen video ad loaded successfully");
            if (config != null)
            {
                adReadyFlags[config.InterstitialAdUnitId] = true;
            }
            OnInterstitialAdReady?.Invoke();
        }

        public void OnFullScreenVideoCached()
        {
            Debug.Log("GromoreManager: Full screen video ad cached");
        }

        public void OnFullScreenVideoCached(FullScreenVideoAd ad)
        {
            Debug.Log("GromoreManager: Full screen video ad cached with ad instance");
            interstitialAd = ad;
        }

        #endregion

        #region IFullScreenVideoAdInteractionListener Implementation
private FullScreenVideoAd interstitialAd;
        void IFullScreenVideoAdInteractionListener.OnAdShow()
        {
            Debug.Log("GromoreManager: Full screen video ad shown");
            
            string placementId = "";
            if (config != null)
            {
                placementId = config.InterstitialAdUnitId;
            }
            // Use wrapper to track ad revenue and impression
            MediationAdEcpmInfo ecpmInfo = null;
            if (interstitialAd != null)
            {
                ecpmInfo = interstitialAd.GetMediationManager().GetShowEcpm();
            }                        
            GromoreAdWrapper.TrackInterstitialAdRevenue(placementId, ecpmInfo);

            OnInterstitialAdDisplayed?.Invoke();
        }

        void IFullScreenVideoAdInteractionListener.OnAdVideoBarClick()
        {
            Debug.Log("GromoreManager: Full screen video ad video bar clicked");
            OnInterstitialAdClicked?.Invoke();
        }

        void IFullScreenVideoAdInteractionListener.OnAdClose()
        {
            Debug.Log("GromoreManager: Full screen video ad closed");
            OnInterstitialAdClosed?.Invoke();
        }

        void IFullScreenVideoAdInteractionListener.OnVideoComplete()
        {
            Debug.Log("GromoreManager: Full screen video ad video completed");
        }

        void IFullScreenVideoAdInteractionListener.OnSkippedVideo()
        {
            Debug.Log("GromoreManager: Full screen video ad video skipped");
        }

        void IFullScreenVideoAdInteractionListener.OnVideoError()
        {
            Debug.LogError("GromoreManager: Full screen video ad video error");
        }

        #endregion

        #region IFeedAdListener Implementation

        void IFeedAdListener.OnError(int code, string message)
        {
            Debug.LogError($"GromoreManager: Feed ad error - Code: {code}, Message: {message}");
            OnBannerAdLoadFailed?.Invoke(message);
        }

        public void OnFeedAdLoad(IList<FeedAd> ads)
        {
            Debug.Log($"GromoreManager: Feed ad loaded successfully, count: {ads?.Count ?? 0}");

            
            if (config != null && ads != null && ads.Count > 0)
            {
                bannerAd = ads[0];
                adReadyFlags[config.BannerAdUnitId] = true;
            }
            OnBannerAdReady?.Invoke();
        }

        #endregion

        #region IFeedAdInteractionListener Implementation

        public void OnAdClicked()
        {
            Debug.Log("GromoreManager: Feed ad clicked");
            OnBannerAdClicked?.Invoke();
        }

        public void OnAdCreativeClick()
        {
            Debug.Log("GromoreManager: Feed ad creative clicked");
        }

private FeedAd bannerAd;
        public void OnAdShow()
        {
            Debug.Log("GromoreManager: Feed ad shown");
            
            // Use wrapper to track ad revenue and impression
            string placementId = "";
            if (config != null)
            {
                placementId = config.BannerAdUnitId;
            }

            // Get ECPM info from the ad (this would need to be stored when ad is loaded)
            // For now, calling with null ecpmInfo - will need to be updated when ad loading is implemented
            MediationAdEcpmInfo ecpmInfo = null;
            if (bannerAd != null)
            {
                ecpmInfo = bannerAd.GetMediationManager().GetShowEcpm();
            }              
            GromoreAdWrapper.TrackBannerAdRevenue(placementId, ecpmInfo);
            
            OnBannerAdDisplayed?.Invoke();
        }

        #endregion

        #region INativeAdListener Implementation

        void INativeAdListener.OnError(int code, string message)
        {
            Debug.LogError($"GromoreManager: Native ad error - Code: {code}, Message: {message}");
            OnNativeAdLoadFailed?.Invoke(message);
        }

        public void OnNativeAdLoad(NativeAd[] ads)
        {
            Debug.Log($"GromoreManager: Native ad loaded successfully, count: {ads?.Length ?? 0}");
            if (config != null && ads != null && ads.Length > 0)
            {
                nativeAd = ads[0];
                adReadyFlags[config.NativeAdUnitId] = true;
            }
            OnNativeAdReady?.Invoke();
        }
private NativeAd nativeAd;
        public void OnAdImpressed(string placementId)
        {
            Debug.Log($"GromoreManager: Native ad impressed for placement: {placementId}");
            
            // Use wrapper to track ad revenue and impression
            MediationAdEcpmInfo ecpmInfo = null;
            if (nativeAd != null)
            {
                ecpmInfo = nativeAd.GetMediationManager().GetShowEcpm();
            }               
            GromoreAdWrapper.TrackNativeAdRevenue(placementId, ecpmInfo);
            
            OnNativeAdDisplayed?.Invoke();
        }

        public void OnAdClicked(string placementId)
        {
            Debug.Log($"GromoreManager: Native ad clicked for placement: {placementId}");
            
            OnNativeAdClicked?.Invoke();
        }

        public void OnAdVideoStart(string placementId)
        {
            Debug.Log($"GromoreManager: Native ad video started for placement: {placementId}");
        }

        public void OnAdVideoEnd(string placementId)
        {
            Debug.Log($"GromoreManager: Native ad video ended for placement: {placementId}");
        }

        public void OnAdVideoProgress(string placementId, int progress)
        {
            Debug.Log($"GromoreManager: Native ad video progress for placement: {placementId}, progress: {progress}");
        }

        public void OnAdCloseButtonClicked(string placementId)
        {
            Debug.Log($"GromoreManager: Native ad close button clicked for placement: {placementId}");
        }

        #endregion

        // Additional methods for AD source loading and bidding (placeholder implementations)
        public void startLoadingADSource(string placementId) { }
        public void finishLoadingADSource(string placementId) { }
        public void failToLoadADSource(string placementId, string code, string message) { }
        public void startBiddingADSource(string placementId) { }
        public void finishBiddingADSource(string placementId) { }
        public void failBiddingADSource(string placementId, string code, string message) { }
    }
}
