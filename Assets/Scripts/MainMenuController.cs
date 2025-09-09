using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button bannerAdButton;
    public Button interstitialAdButton;
    public Button rewardedAdButton;
    public Button nativeAdButton;
    public Button splashAdButton;
    public Button appOpenAdButton;

    [Header("Button Labels")]
    public Text bannerAdText;
    public Text interstitialAdText;
    public Text rewardedAdText;
    public Text nativeAdText;
    public Text splashAdText;
    public Text appOpenAdText;

    void Start()
    {
        SetupButtons();
        SetupButtonLabels();
    }

    void SetupButtons()
    {
        // Banner Ad Button
        if (bannerAdButton != null)
        {
            bannerAdButton.onClick.AddListener(() => {
                if (SceneController.Instance != null)
                {
                    SceneController.Instance.LoadBannerAdScene();
                }
            });
        }

        // Interstitial Ad Button
        if (interstitialAdButton != null)
        {
            interstitialAdButton.onClick.AddListener(() => {
                if (SceneController.Instance != null)
                {
                    SceneController.Instance.LoadInterstitialAdScene();
                }
            });
        }

        // Rewarded Ad Button
        if (rewardedAdButton != null)
        {
            rewardedAdButton.onClick.AddListener(() => {
                if (SceneController.Instance != null)
                {
                    SceneController.Instance.LoadRewardedAdScene();
                }
            });
        }

        // Native Ad Button
        if (nativeAdButton != null)
        {
            nativeAdButton.onClick.AddListener(() => {
                if (SceneController.Instance != null)
                {
                    SceneController.Instance.LoadNativeAdScene();
                }
            });
        }

        // Splash Ad Button
        if (splashAdButton != null)
        {
            splashAdButton.onClick.AddListener(() => {
                if (SceneController.Instance != null)
                {
                    SceneController.Instance.LoadSplashAdScene();
                }
            });
        }

        // App Open Ad Button
        if (appOpenAdButton != null)
        {
            appOpenAdButton.onClick.AddListener(() => {
                if (SceneController.Instance != null)
                {
                    SceneController.Instance.LoadAppOpenAdScene();
                }
            });
        }
    }

    void SetupButtonLabels()
    {
        if (bannerAdText != null) bannerAdText.text = "Banner Ad";
        if (interstitialAdText != null) interstitialAdText.text = "Interstitial Ad";
        if (rewardedAdText != null) rewardedAdText.text = "Rewarded Ad";
        if (nativeAdText != null) nativeAdText.text = "Native Ad";
        if (splashAdText != null) splashAdText.text = "Splash Ad";
        if (appOpenAdText != null) appOpenAdText.text = "App Open Ad";
    }
}
