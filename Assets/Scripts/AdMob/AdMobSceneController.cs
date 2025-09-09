using UnityEngine;
using UnityEngine.SceneManagement;

namespace SolarEngineMeditationSample.AdMob
{
    /// <summary>
    /// AdMob scene controller that handles scene-specific ad logic
    /// </summary>
    public class AdMobSceneController : MonoBehaviour
    {
        [Header("AdMob Integration")]
        [SerializeField] private AdMobSampleAdManager adManager;
        
        [Header("Scene Settings")]
        [SerializeField] private bool showSplashAdOnStart = true;
        [SerializeField] private bool showInterstitialOnSceneLoad = false;
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogging = true;
        
        private AdMobManager adMobManager;
        private bool isSceneReady = false;

        private void Start()
        {
            if (enableDebugLogging)
                Debug.Log("AdMobSceneController started");
            
            // Find AdMobManager
            adMobManager = AdMobManager.Instance;
            
            if (adMobManager == null)
            {
                Debug.LogError("AdMobManager not found in scene!");
                return;
            }

            // Subscribe to AdMob events
            adMobManager.OnAdMobInitialized += OnAdMobInitialized;
            
            // Setup scene
            SetupScene();
        }

        private void SetupScene()
        {
            if (enableDebugLogging)
                Debug.Log("Setting up AdMob scene...");
            
            // Wait for AdMob to be ready
            if (adMobManager.IsInitialized)
            {
                OnAdMobInitialized();
            }
        }

        private void OnAdMobInitialized()
        {
            isSceneReady = true;
            
            if (enableDebugLogging)
                Debug.Log("AdMob initialized in scene - ready to show ads");
            
            // Show splash ad if enabled
            if (showSplashAdOnStart)
            {
                ShowSplashAd();
            }
            
            // Show interstitial if enabled
            if (showInterstitialOnSceneLoad)
            {
                ShowInterstitialAd();
            }
        }

        #region Public Ad Methods

        /// <summary>
        /// Show splash/app open ad
        /// </summary>
        public void ShowSplashAd()
        {
            if (!isSceneReady)
            {
                Debug.LogWarning("Scene not ready yet");
                return;
            }

            if (enableDebugLogging)
                Debug.Log("Showing splash ad...");
            
            adMobManager.ShowAppOpenAd();
        }

        /// <summary>
        /// Show interstitial ad
        /// </summary>
        public void ShowInterstitialAd()
        {
            if (!isSceneReady)
            {
                Debug.LogWarning("Scene not ready yet");
                return;
            }

            if (enableDebugLogging)
                Debug.Log("Showing interstitial ad...");
            
            adMobManager.ShowInterstitialAd();
        }

        /// <summary>
        /// Show rewarded ad
        /// </summary>
        public void ShowRewardedAd()
        {
            if (!isSceneReady)
            {
                Debug.LogWarning("Scene not ready yet");
                return;
            }

            if (enableDebugLogging)
                Debug.Log("Showing rewarded ad...");
            
            adMobManager.ShowRewardedAd();
        }

        /// <summary>
        /// Show banner ad
        /// </summary>
        public void ShowBannerAd()
        {
            if (!isSceneReady)
            {
                Debug.LogWarning("Scene not ready yet");
                return;
            }

            if (enableDebugLogging)
                Debug.Log("Showing banner ad...");
            
            adMobManager.LoadAndShowBannerAd();
        }

        /// <summary>
        /// Hide banner ad
        /// </summary>
        public void HideBannerAd()
        {
            if (enableDebugLogging)
                Debug.Log("Hiding banner ad...");
            
            adMobManager.HideBannerAd();
        }

        #endregion

        #region Scene Management

        /// <summary>
        /// Load a new scene with optional interstitial ad
        /// </summary>
        public void LoadScene(string sceneName, bool showInterstitial = true)
        {
            if (enableDebugLogging)
                Debug.Log($"Loading scene: {sceneName}");
            
            if (showInterstitial && isSceneReady)
            {
                // Show interstitial before loading scene
                adMobManager.ShowInterstitialAd();
                
                // Wait a bit for the ad to show, then load scene
                StartCoroutine(LoadSceneAfterAd(sceneName));
            }
            else
            {
                // Load scene immediately
                SceneManager.LoadScene(sceneName);
            }
        }

        private System.Collections.IEnumerator LoadSceneAfterAd(string sceneName)
        {
            // Wait for a short time to allow ad to display
            yield return new WaitForSeconds(0.5f);
            
            if (enableDebugLogging)
                Debug.Log($"Loading scene after ad: {sceneName}");
            
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Return to previous scene
        /// </summary>
        public void ReturnToPreviousScene()
        {
            if (enableDebugLogging)
                Debug.Log("Returning to previous scene");
            
            // Show interstitial before returning
            if (isSceneReady)
            {
                adMobManager.ShowInterstitialAd();
                StartCoroutine(ReturnAfterAd());
            }
            else
            {
                // Return immediately if ads not ready
                if (Application.CanStreamedLevelBeLoaded("SampleScene"))
                {
                    SceneManager.LoadScene("SampleScene");
                }
            }
        }

        private System.Collections.IEnumerator ReturnAfterAd()
        {
            // Wait for a short time to allow ad to display
            yield return new WaitForSeconds(0.5f);
            
            if (enableDebugLogging)
                Debug.Log("Returning to previous scene after ad");
            
            // Return to main scene
            if (Application.CanStreamedLevelBeLoaded("SampleScene"))
            {
                SceneManager.LoadScene("SampleScene");
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Check if scene is ready for ads
        /// </summary>
        public bool IsSceneReady()
        {
            return isSceneReady && adMobManager != null && adMobManager.IsInitialized;
        }

        /// <summary>
        /// Get AdMob manager instance
        /// </summary>
        public AdMobManager GetAdMobManager()
        {
            return adMobManager;
        }

        /// <summary>
        /// Get AdMob sample ad manager
        /// </summary>
        public AdMobSampleAdManager GetSampleAdManager()
        {
            return adManager;
        }

        #endregion

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (adMobManager != null)
            {
                adMobManager.OnAdMobInitialized -= OnAdMobInitialized;
            }
        }
    }
}
