using UnityEngine;
using UnityEngine.SceneManagement;
using SolarEngineMeditationSample.AdMob;
using SolarEngineMeditationSample.IronSource;
using SolarEngineMeditationSample.Max;
using SolarEngineMeditationSample.Taku;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;
    
    // Track which platform is currently selected
    private string currentPlatform = "AdMob"; // Default to AdMob
    
    public void SetCurrentPlatform(string platform)
    {
        currentPlatform = platform;
        Debug.Log($"SceneController: Platform set to {platform}");
    }

    void Awake()
    {
        // Singleton pattern
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

    public void LoadBannerAdScene()
    {
        Debug.Log($"Loading Banner Ad Scene for {currentPlatform}...");
        ShowAdForCurrentPlatform("Banner Ad", AdType.Banner);
    }

    public void LoadInterstitialAdScene()
    {
        Debug.Log($"Loading Interstitial Ad Scene for {currentPlatform}...");
        ShowAdForCurrentPlatform("Interstitial Ad", AdType.Interstitial);
    }

    public void LoadRewardedAdScene()
    {
        Debug.Log($"Loading Rewarded Ad Scene for {currentPlatform}...");
        ShowAdForCurrentPlatform("Rewarded Ad", AdType.Rewarded);
    }

    public void LoadNativeAdScene()
    {
        Debug.Log($"Loading Native Ad Scene for {currentPlatform}...");
        ShowAdForCurrentPlatform("Native Ad", AdType.Native);
    }

    public void LoadSplashAdScene()
    {
        Debug.Log($"Loading Splash Ad Scene for {currentPlatform}...");
        ShowAdForCurrentPlatform("Splash Ad", AdType.Splash);
    }

    public void LoadAppOpenAdScene()
    {
        Debug.Log($"Loading App Open Ad Scene for {currentPlatform}...");
        ShowAdForCurrentPlatform("App Open Ad", AdType.Splash);
    }

    public void LoadMainMenu()
    {
        Debug.Log("Loading Main Menu...");
        SceneManager.LoadScene("MainMenuScene");
    }

    private void ShowAdForCurrentPlatform(string adTypeName, AdType adType)
    {
        Debug.Log($"Showing {currentPlatform} {adTypeName}...");
        
        // Route to the appropriate ad manager based on current platform
        switch (currentPlatform.ToLower())
        {
            case "ironsource":
                ShowIronSourceAd(adTypeName, adType);
                break;
                    case "max":
            ShowMaxAd(adTypeName, adType);
            break;
        case "taku":
            ShowTakuAd(adTypeName, adType);
            break;
            case "admob":
            default:
                ShowAdMobAd(adTypeName, adType);
                break;
        }
    }
    
    private void ShowAdMobAd(string adTypeName, AdType adType)
    {
        Debug.Log($"Showing AdMob {adTypeName}...");
        
        // Check if AdMob is available
        if (AdMobManager.Instance == null)
        {
            Debug.LogWarning("AdMobManager not found! Showing coming soon message.");
            ShowComingSoon(adTypeName);
            return;
        }
        
        if (!AdMobManager.Instance.IsInitialized)
        {
            Debug.LogWarning("AdMob not initialized yet! Showing coming soon message.");
            ShowComingSoon(adTypeName);
            return;
        }
        
        // Show the appropriate ad type
        switch (adType)
        {
            case AdType.Banner:
                AdMobManager.Instance.LoadAndShowBannerAd();
                Debug.Log("Banner ad requested");
                break;
                
            case AdType.Interstitial:
                AdMobManager.Instance.ShowInterstitialAd();
                Debug.Log("Interstitial ad requested");
                break;
                
            case AdType.Rewarded:
                AdMobManager.Instance.ShowRewardedAd();
                Debug.Log("Rewarded ad requested");
                break;
                
            case AdType.Splash:
                AdMobManager.Instance.ShowAppOpenAd();
                Debug.Log("Splash/App Open ad requested");
                break;
                
            case AdType.Native:
                Debug.Log("Native ads not implemented yet - showing coming soon");
                ShowComingSoon(adTypeName);
                break;
                
            default:
                Debug.LogWarning($"Unknown ad type: {adType}");
                ShowComingSoon(adTypeName);
                break;
        }
    }
    
    private void ShowIronSourceAd(string adTypeName, AdType adType)
    {
        Debug.Log($"Showing IronSource {adTypeName}...");
        
        // Check if IronSource is available
        if (IronSourceManager.Instance == null)
        {
            Debug.LogWarning("IronSourceManager not found! Showing coming soon message.");
            ShowComingSoon(adTypeName);
            return;
        }
        
        // Check configuration validity first
        if (!IronSourceManager.Instance.IsConfigValid)
        {
            Debug.LogWarning("IronSource configuration is invalid! App key not set.");
            IronSourceManager.Instance.LogConfigurationStatus(); // Debug info
            ShowComingSoon($"IronSource {adTypeName} - Invalid configuration. Please set app key in IronSourceConfig.");
            return;
        }
        
        if (!IronSourceManager.Instance.IsInitialized)
        {
            string status = IronSourceManager.Instance.GetInitializationStatus();
            Debug.LogWarning($"IronSource not initialized: {status}");
            IronSourceManager.Instance.LogConfigurationStatus(); // Debug info
            ShowComingSoon($"IronSource {adTypeName} - {status}");
            return;
        }
        
        // Show the appropriate ad type
        switch (adType)
        {
            case AdType.Banner:
                IronSourceManager.Instance.LoadAndShowBannerAd();
                Debug.Log("IronSource Banner ad requested");
                break;
                
            case AdType.Interstitial:
                IronSourceManager.Instance.LoadAndShowInterstitialAd();
                Debug.Log("IronSource Interstitial ad requested");
                break;
                
            case AdType.Rewarded:
                IronSourceManager.Instance.LoadAndShowRewardedAd();
                Debug.Log("IronSource Rewarded ad requested");
                break;
                
            case AdType.Splash:
                IronSourceManager.Instance.LoadAndShowAppOpenAd();
                Debug.Log("IronSource App Open ad requested");
                break;
                
            case AdType.Native:
                Debug.Log("IronSource Native ads not yet implemented - showing coming soon");
                ShowComingSoon($"IronSource {adTypeName} - Native not yet implemented");
                break;
                
            default:
                Debug.LogWarning($"Unknown IronSource ad type: {adType}");
                ShowComingSoon(adTypeName);
                break;
        }
    }
    
    private void ShowMaxAd(string adTypeName, AdType adType)
    {
        Debug.Log($"Showing Max {adTypeName}...");
        
        // Check if Max is available
        if (MaxManager.Instance == null)
        {
            Debug.LogWarning("MaxManager not found! Showing coming soon message.");
            ShowComingSoon(adTypeName);
            return;
        }
        
        // Check configuration validity first
        if (!MaxManager.Instance.IsConfigValid)
        {
            Debug.LogWarning("Max configuration is invalid! SDK key not set.");
            MaxManager.Instance.LogConfigurationStatus(); // Debug info
            ShowComingSoon($"Max {adTypeName} - Invalid configuration. Please set SDK key in MaxConfig.");
            return;
        }
        
        if (!MaxManager.Instance.IsInitialized)
        {
            string status = MaxManager.Instance.GetInitializationStatus();
            Debug.LogWarning($"Max not initialized: {status}");
            MaxManager.Instance.LogConfigurationStatus(); // Debug info
            ShowComingSoon($"Max {adTypeName} - {status}");
            return;
        }
        
        // Show the appropriate ad type
        switch (adType)
        {
            case AdType.Banner:
                MaxManager.Instance.ShowBannerAd();
                Debug.Log("Max Banner ad requested");
                break;
                
            case AdType.Interstitial:
                MaxManager.Instance.ShowInterstitialAd();
                Debug.Log("Max Interstitial ad requested");
                break;
                
            case AdType.Rewarded:
                MaxManager.Instance.ShowRewardedAd();
                Debug.Log("Max Rewarded ad requested");
                break;
                
            case AdType.Splash:
                MaxManager.Instance.ShowAppOpenAd();
                Debug.Log("Max App Open ad requested");
                break;
                
            case AdType.Native:
                Debug.Log("Max Native ads not yet implemented - showing coming soon");
                ShowComingSoon($"Max {adTypeName} - Native not yet implemented");
                break;
                
            default:
                Debug.LogWarning($"Unknown Max ad type: {adType}");
                ShowComingSoon(adTypeName);
                break;
        }
    }
    
    private void ShowComingSoon(string adType)
    {
        // 临时Show即将推出的消息
        Debug.Log($"{adType} scene coming soon!");
        
        // 可以在这里AddUI提示
        if (ComingSoonUI.Instance != null)
        {
            ComingSoonUI.Instance.Show($"{adType} feature coming soon！");
        }
    }

    private void ShowTakuAd(string adTypeName, AdType adType)
    {
        Debug.Log($"Showing Taku {adTypeName}...");
        
        // Check if Taku is available
        if (GameManager.Instance.takuManager == null)
        {
            Debug.LogWarning("TakuManager not found! Showing coming soon message.");
            ShowComingSoon(adTypeName);
            return;
        }
        
        // Check configuration validity first
        if (!GameManager.Instance.takuManager.IsValid())
        {
            Debug.LogWarning("Taku configuration is invalid! App ID or App Key not set.");
            Debug.Log(GameManager.Instance.takuManager.GetConfigStatus());
            ShowComingSoon($"Taku {adTypeName} - Invalid configuration. Please set App ID and App Key in TakuConfig.");
            return;
        }
        
        if (!GameManager.Instance.takuManager.IsInitialized)
        {
            string status = GameManager.Instance.takuManager.GetInitializationStatus();
            Debug.LogWarning($"Taku not initialized: {status}");
            Debug.Log(GameManager.Instance.takuManager.GetConfigStatus());
            ShowComingSoon($"Taku {adTypeName} - {status}");
            return;
        }
        
        // Show the appropriate ad type
        switch (adType)
        {
            case AdType.Banner:
                GameManager.Instance.takuManager.ShowBannerAd();
                Debug.Log("Taku Banner ad requested");
                break;
                
            case AdType.Interstitial:
                GameManager.Instance.takuManager.ShowInterstitialAd();
                Debug.Log("Taku Interstitial ad requested");
                break;
                
            case AdType.Rewarded:
                GameManager.Instance.takuManager.ShowRewardedAd();
                Debug.Log("Taku Rewarded ad requested");
                break;
                
            case AdType.Native:
                GameManager.Instance.takuManager.ShowNativeAd();
                Debug.Log("Taku Native ad requested");
                break;
                
            case AdType.Splash:
                Debug.Log("Taku App Open ads not yet implemented - showing coming soon");
                ShowComingSoon($"Taku {adTypeName} - App Open not yet implemented");
                break;
                
            default:
                Debug.LogWarning($"Unknown Taku ad type: {adType}");
                ShowComingSoon(adTypeName);
                break;
        }
    }
}
