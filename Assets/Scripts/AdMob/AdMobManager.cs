using UnityEngine;
using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;
using SolarEngineMeditationSample.Wrappers.AdMob;

namespace SolarEngineMeditationSample.AdMob
{
    /// <summary>
    /// Main AdMob manager that handles SDK initialization and ad management
    /// </summary>
    public class AdMobManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private AdMobConfig config;
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogging = true;
        
        // Singleton instance
        public static AdMobManager Instance { get; private set; }
        
        // Ad instances
        private BannerView bannerView;
        private InterstitialAd interstitialAd;
        private RewardedAd rewardedAd;
        private AppOpenAd appOpenAd;
        
        // Events
        public event Action OnAdMobInitialized;
        public event Action<string> OnAdMobInitializationFailed;
        
        // Properties
        public bool IsInitialized { get; private set; }
        public AdMobConfig Config => config;
        
        // Setter for config (for runtime assignment)
        public void SetConfig(AdMobConfig newConfig)
        {
            config = newConfig;
            
            // Initialize AdMob after config is set
            if (!IsInitialized)
            {
                InitializeAdMob();
            }
        }

        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                // Don't initialize here - wait for config to be set
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (enableDebugLogging)
                Debug.Log("AdMobManager started");
            
            // If config was set before Start, initialize now
            if (config != null && !IsInitialized)
            {
                InitializeAdMob();
            }
        }

        /// <summary>
        /// Initialize AdMob SDK
        /// </summary>
        private void InitializeAdMob()
        {
            if (config == null)
            {
                Debug.LogError("AdMobConfig is not assigned!");
                OnAdMobInitializationFailed?.Invoke("AdMobConfig is not assigned");
                return;
            }

            try
            {
                // Initialize the Google Mobile Ads SDK
                MobileAds.Initialize(initStatus =>
                {
                    if (enableDebugLogging)
                        Debug.Log("AdMob SDK initialization completed");
                    
                    IsInitialized = true;
                    OnAdMobInitialized?.Invoke();
                    
                    // Preload ads after initialization
                    PreloadAds();
                });
            }
            catch (Exception e)
            {
                Debug.LogError($"AdMob SDK initialization failed: {e.Message}");
                OnAdMobInitializationFailed?.Invoke(e.Message);
            }
        }

        /// <summary>
        /// Preload all ad types
        /// </summary>
        private void PreloadAds()
        {
            if (enableDebugLogging)
                Debug.Log("Preloading ads...");
            
            LoadInterstitialAd();
            LoadRewardedAd();
            LoadAppOpenAd();
        }

        #region Banner Ads

        /// <summary>
        /// Load and show banner ad
        /// </summary>
        public void LoadAndShowBannerAd(AdPosition position = AdPosition.Bottom)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("AdMob not initialized yet");
                return;
            }

            // Destroy existing banner
            if (bannerView != null)
            {
                bannerView.Destroy();
                bannerView = null;
            }

            // Create banner view
            bannerView = new BannerView(config.GetBannerAdUnitId(), AdSize.Banner, position);
            
            // Use AdMob wrapper for paid events
            bannerView.OnAdPaid += AdMobAdWrapper.BuildBannerAdPaidEventHandler(
                (AdValue adValue) => {
                    if (enableDebugLogging)
                        Debug.Log($"Banner ad paid event: {adValue.Value} {adValue.CurrencyCode}");
                },
                bannerView.GetResponseInfo()
            );
            
            // Create ad request
            var adRequest = new AdRequest();
            
            // Load banner
            bannerView.LoadAd(adRequest);
            
            if (enableDebugLogging)
                Debug.Log($"Banner ad loading for position: {position}");
        }

        /// <summary>
        /// Hide banner ad
        /// </summary>
        public void HideBannerAd()
        {
            if (bannerView != null)
            {
                bannerView.Hide();
                if (enableDebugLogging)
                    Debug.Log("Banner ad hidden");
            }
        }

        /// <summary>
        /// Destroy banner ad
        /// </summary>
        public void DestroyBannerAd()
        {
            if (bannerView != null)
            {
                bannerView.Destroy();
                bannerView = null;
                if (enableDebugLogging)
                    Debug.Log("Banner ad destroyed");
            }
        }

        #endregion

        #region Interstitial Ads

        /// <summary>
        /// Load interstitial ad
        /// </summary>
        public void LoadInterstitialAd()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("AdMob not initialized yet");
                return;
            }

            // Destroy existing interstitial
            if (interstitialAd != null)
            {
                interstitialAd.Destroy();
                interstitialAd = null;
            }

            if (enableDebugLogging)
                Debug.Log("Loading interstitial ad...");

            // Create ad request
            var adRequest = new AdRequest();

            // Load interstitial using the new v10.x API
            InterstitialAd.Load(config.GetInterstitialAdUnitId(), adRequest, (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    Debug.LogError($"Interstitial ad failed to load: {error.GetMessage()}");
                    return;
                }

                if (ad == null)
                {
                    Debug.LogError("Unexpected error: Interstitial load event fired with null ad and null error.");
                    return;
                }

                // Successfully loaded
                interstitialAd = ad;
                
                // Use AdMob wrapper for paid events
                interstitialAd.OnAdPaid += AdMobAdWrapper.BuildInterstitialAdPaidEventHandler(
                    (AdValue adValue) => {
                        if (enableDebugLogging)
                            Debug.Log($"Interstitial ad paid event: {adValue.Value} {adValue.CurrencyCode}");
                    },
                    interstitialAd.GetResponseInfo()
                );
                
                if (enableDebugLogging)
                    Debug.Log("Interstitial ad loaded successfully");

                // Register event handlers
                RegisterInterstitialEventHandlers(ad);
            });
        }

        /// <summary>
        /// Show interstitial ad
        /// </summary>
        public void ShowInterstitialAd()
        {
            if (interstitialAd != null && interstitialAd.CanShowAd())
            {
                interstitialAd.Show();
                if (enableDebugLogging)
                    Debug.Log("Interstitial ad shown");
            }
            else
            {
                Debug.LogWarning("Interstitial ad not ready. Loading new one...");
                LoadInterstitialAd();
            }
        }

        #endregion

        #region Rewarded Ads

        /// <summary>
        /// Load rewarded ad
        /// </summary>
        public void LoadRewardedAd()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("AdMob not initialized yet");
                return;
            }

            // Destroy existing rewarded ad
            if (rewardedAd != null)
            {
                rewardedAd.Destroy();
                rewardedAd = null;
            }

            if (enableDebugLogging)
                Debug.Log("Loading rewarded ad...");

            // Create ad request
            var adRequest = new AdRequest();

            // Load rewarded ad using the new v10.x API
            RewardedAd.Load(config.GetRewardedAdUnitId(), adRequest, (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    Debug.LogError($"Rewarded ad failed to load: {error.GetMessage()}");
                    return;
                }

                if (ad == null)
                {
                    Debug.LogError("Unexpected error: Rewarded ad load event fired with null ad and null error.");
                    return;
                }

                // Successfully loaded
                rewardedAd = ad;
                
                // Use AdMob wrapper for paid events
                rewardedAd.OnAdPaid += AdMobAdWrapper.BuildRewardedAdPaidEventHandler(
                    (AdValue adValue) => {
                        if (enableDebugLogging)
                            Debug.Log($"Rewarded ad paid event: {adValue.Value} {adValue.CurrencyCode}");
                    },
                    rewardedAd.GetResponseInfo()
                );
                
                if (enableDebugLogging)
                    Debug.Log("Rewarded ad loaded successfully");

                // Register event handlers
                RegisterRewardedEventHandlers(ad);
            });
        }

        /// <summary>
        /// Show rewarded ad
        /// </summary>
        public void ShowRewardedAd()
        {
            if (rewardedAd != null && rewardedAd.CanShowAd())
            {
                rewardedAd.Show((Reward reward) =>
                {
                    if (enableDebugLogging)
                        Debug.Log($"User earned reward: {reward.Amount} {reward.Type}");
                });
                
                if (enableDebugLogging)
                    Debug.Log("Rewarded ad shown");
            }
            else
            {
                Debug.LogWarning("Rewarded ad not ready. Loading new one...");
                LoadRewardedAd();
            }
        }

        #endregion

        #region App Open Ads (Splash)

        /// <summary>
        /// Load app open ad
        /// </summary>
        public void LoadAppOpenAd()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("AdMob not initialized yet");
                return;
            }

            // Destroy existing app open ad
            if (appOpenAd != null)
            {
                appOpenAd.Destroy();
                appOpenAd = null;
            }

            if (enableDebugLogging)
                Debug.Log("Loading app open ad...");

            // Create ad request
            var adRequest = new AdRequest();

            // Load app open ad using the new v10.x API
            AppOpenAd.Load(config.GetSplashAdUnitId(), adRequest, (AppOpenAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    Debug.LogError($"App open ad failed to load: {error.GetMessage()}");
                    return;
                }

                if (ad == null)
                {
                    Debug.LogError("Unexpected error: App open ad load event fired with null ad and null error.");
                    return;
                }

                // Successfully loaded
                appOpenAd = ad;
                
                // Use AdMob wrapper for paid events
                appOpenAd.OnAdPaid += AdMobAdWrapper.BuildAppOpenAdPaidEventHandler(
                    (AdValue adValue) => {
                        if (enableDebugLogging)
                            Debug.Log($"App open ad paid event: {adValue.Value} {adValue.CurrencyCode}");
                    },
                    appOpenAd.GetResponseInfo()
                );
                
                if (enableDebugLogging)
                    Debug.Log("App open ad loaded successfully");
            });
        }

        /// <summary>
        /// Show app open ad
        /// </summary>
        public void ShowAppOpenAd()
        {
            if (appOpenAd != null && appOpenAd.CanShowAd())
            {
                appOpenAd.Show();
                if (enableDebugLogging)
                    Debug.Log("App open ad shown");
            }
            else
            {
                Debug.LogWarning("App open ad not ready. Loading new one...");
                LoadAppOpenAd();
            }
        }

        #endregion

        #region Event Handlers Registration

        private void RegisterInterstitialEventHandlers(InterstitialAd ad)
        {
            ad.OnAdFullScreenContentClosed += () =>
            {
                if (enableDebugLogging)
                    Debug.Log("Interstitial ad closed");
                
                // Load new interstitial
                LoadInterstitialAd();
            };

            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError($"Interstitial ad failed to show: {error.GetMessage()}");
            };
        }

        private void RegisterRewardedEventHandlers(RewardedAd ad)
        {
            ad.OnAdFullScreenContentClosed += () =>
            {
                if (enableDebugLogging)
                    Debug.Log("Rewarded ad closed");
                
                // Load new rewarded ad
                LoadRewardedAd();
            };

            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError($"Rewarded ad failed to show: {error.GetMessage()}");
            };

            // Note: UserEarnedReward is now handled in the Show() callback
            // No need to register this event separately
        }

        #endregion

        #region Native Ads

        /// <summary>
        /// Load native ad (placeholder for future implementation)
        /// </summary>
        public void LoadNativeAd()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("AdMob not initialized yet");
                return;
            }

            if (enableDebugLogging)
                Debug.Log("Native ad loading not yet implemented - use AdMobAdWrapper.BuildNativeAdPaidEventHandler when ready");
        }

        #endregion

        private void OnDestroy()
        {
            // Clean up ads
            if (bannerView != null)
                bannerView.Destroy();
            
            if (interstitialAd != null)
                interstitialAd.Destroy();
            
            if (rewardedAd != null)
                rewardedAd.Destroy();
            
            if (appOpenAd != null)
                appOpenAd.Destroy();
        }
    }
}
