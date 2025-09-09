using UnityEngine;
using UnityEngine.UI;

namespace SolarEngineMeditationSample.AdMob
{
    /// <summary>
    /// Sample AdMob ad manager that integrates with the existing UI system
    /// </summary>
    public class AdMobSampleAdManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button bannerAdButton;
        [SerializeField] private Button interstitialAdButton;
        [SerializeField] private Button rewardedAdButton;
        [SerializeField] private Button splashAdButton;
        [SerializeField] private Button nativeAdButton;
        
        [Header("Status Display")]
        [SerializeField] private Text statusText;
        
        private AdMobManager adMobManager;
        private bool isAdMobReady = false;

        private void Start()
        {
            // Find AdMobManager in the scene
            adMobManager = AdMobManager.Instance;
            
            if (adMobManager == null)
            {
                Debug.LogError("AdMobManager not found in scene!");
                return;
            }

            // Subscribe to AdMob events
            adMobManager.OnAdMobInitialized += OnAdMobInitialized;
            adMobManager.OnAdMobInitializationFailed += OnAdMobInitializationFailed;
            
            // Setup UI buttons
            SetupUIButtons();
            
            // Update status
            UpdateStatus("Initializing AdMob...");
        }

        private void SetupUIButtons()
        {
            if (bannerAdButton != null)
                bannerAdButton.onClick.AddListener(ShowBannerAd);
            
            if (interstitialAdButton != null)
                interstitialAdButton.onClick.AddListener(ShowInterstitialAd);
            
            if (rewardedAdButton != null)
                rewardedAdButton.onClick.AddListener(ShowRewardedAd);
            
            if (splashAdButton != null)
                splashAdButton.onClick.AddListener(ShowSplashAd);
            
            if (nativeAdButton != null)
                nativeAdButton.onClick.AddListener(ShowNativeAd);
            
            // Initially disable buttons until AdMob is ready
            SetButtonsInteractable(false);
        }

        private void OnAdMobInitialized()
        {
            isAdMobReady = true;
            SetButtonsInteractable(true);
            UpdateStatus("AdMob initialized successfully! Ads are ready.");
            
            Debug.Log("AdMob initialized - all ad buttons are now enabled");
        }

        private void OnAdMobInitializationFailed(string error)
        {
            isAdMobReady = false;
            SetButtonsInteractable(false);
            UpdateStatus($"AdMob initialization failed: {error}");
            
            Debug.LogError($"AdMob initialization failed: {error}");
        }

        private void SetButtonsInteractable(bool interactable)
        {
            if (bannerAdButton != null)
                bannerAdButton.interactable = interactable;
            
            if (interstitialAdButton != null)
                interstitialAdButton.interactable = interactable;
            
            if (rewardedAdButton != null)
                rewardedAdButton.interactable = interactable;
            
            if (splashAdButton != null)
                splashAdButton.interactable = interactable;
            
            if (nativeAdButton != null)
                nativeAdButton.interactable = interactable;
        }

        private void UpdateStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
                Debug.Log($"AdMob Status: {message}");
            }
        }

        #region Ad Display Methods

        public void ShowBannerAd()
        {
            if (!isAdMobReady)
            {
                UpdateStatus("AdMob not ready yet");
                return;
            }

            UpdateStatus("Loading banner ad...");
            adMobManager.LoadAndShowBannerAd();
            UpdateStatus("Banner ad loaded and displayed");
        }

        public void ShowInterstitialAd()
        {
            if (!isAdMobReady)
            {
                UpdateStatus("AdMob not ready yet");
                return;
            }

            UpdateStatus("Showing interstitial ad...");
            adMobManager.ShowInterstitialAd();
            UpdateStatus("Interstitial ad request sent");
        }

        public void ShowRewardedAd()
        {
            if (!isAdMobReady)
            {
                UpdateStatus("AdMob not ready yet");
                return;
            }

            UpdateStatus("Showing rewarded ad...");
            adMobManager.ShowRewardedAd();
            UpdateStatus("Rewarded ad request sent");
        }

        public void ShowSplashAd()
        {
            if (!isAdMobReady)
            {
                UpdateStatus("AdMob not ready yet");
                return;
            }

            UpdateStatus("Showing splash/app open ad...");
            adMobManager.ShowAppOpenAd();
            UpdateStatus("Splash ad request sent");
        }

        public void ShowNativeAd()
        {
            if (!isAdMobReady)
            {
                UpdateStatus("AdMob not ready yet");
                return;
            }

            UpdateStatus("Native ads not implemented yet - coming soon!");
            // TODO: Implement native ad display
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Hide banner ad
        /// </summary>
        public void HideBannerAd()
        {
            if (adMobManager != null)
            {
                adMobManager.HideBannerAd();
                UpdateStatus("Banner ad hidden");
            }
        }

        /// <summary>
        /// Check if AdMob is ready
        /// </summary>
        public bool IsAdMobReady()
        {
            return isAdMobReady && adMobManager != null && adMobManager.IsInitialized;
        }

        /// <summary>
        /// Get AdMob configuration
        /// </summary>
        public AdMobConfig GetConfig()
        {
            return adMobManager?.Config;
        }

        #endregion

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (adMobManager != null)
            {
                adMobManager.OnAdMobInitialized -= OnAdMobInitialized;
                adMobManager.OnAdMobInitializationFailed -= OnAdMobInitializationFailed;
            }
        }
    }
}
