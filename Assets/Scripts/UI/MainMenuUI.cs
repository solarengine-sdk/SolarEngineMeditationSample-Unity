using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI References")]
    public Canvas mainCanvas;
    public RectTransform buttonContainer;
    public Button buttonPrefab;

    [Header("UI Settings")]
    public float buttonSpacing = 25f;
    public Vector2 buttonSize;
    public Color buttonColor = new Color(0.1f, 0.4f, 0.8f, 1f);
    public Color buttonHoverColor = new Color(0.2f, 0.5f, 0.9f, 1f);

    private MainMenuController menuController;

    void Start()
    {
        // 根据屏幕尺寸动态计算Button大小
        CalculateButtonSize();
        CreateUI();
    }

    void CreateUI()
    {
        // 确保有Canvas
        if (mainCanvas == null)
        {
            GameObject canvasObj = new GameObject("MainCanvas");
            mainCanvas = canvasObj.AddComponent<Canvas>();
            mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            mainCanvas.sortingOrder = 200; // 确保在平台列表Page之上
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            GraphicRaycaster raycaster = canvasObj.AddComponent<GraphicRaycaster>();
            raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
            
            // Add白色背景
            GameObject bgObj = new GameObject("Background");
            RectTransform bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.SetParent(mainCanvas.transform, false);
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            Image bgImage = bgObj.AddComponent<Image>();
            bgImage.color = Color.white;
        }

        // 创建Button容器
        if (buttonContainer == null)
        {
            GameObject containerObj = new GameObject("ButtonContainer");
            buttonContainer = containerObj.AddComponent<RectTransform>();
            buttonContainer.SetParent(mainCanvas.transform, false);
            buttonContainer.anchorMin = new Vector2(0.5f, 0.5f);
            buttonContainer.anchorMax = new Vector2(0.5f, 0.5f);
            buttonContainer.anchoredPosition = Vector2.zero;
        }

        // 创建Button
        CreateAdButtons();

        // Add控制器
        if (menuController == null)
        {
            menuController = gameObject.AddComponent<MainMenuController>();
        }
        
        // 创建BackButton（确保在MainMenuUI的Canvas中）
        CreateBackButtonInMainMenu();
    }
    
    void CreateBackButtonInMainMenu()
    {
        Debug.Log("Creating back button in MainMenuUI...");
        
        // 计算BackButton大小
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float buttonWidth = Mathf.Clamp(screenWidth * 0.18f, 200f, 350f);
        float buttonHeight = Mathf.Clamp(screenHeight * 0.12f, 120f, 180f);
        
        // 创建BackButton
        GameObject backButtonObj = new GameObject("BackButton_MainMenu");
        RectTransform backButtonRect = backButtonObj.AddComponent<RectTransform>();
        backButtonRect.SetParent(mainCanvas.transform, false);
        backButtonRect.sizeDelta = new Vector2(buttonWidth, buttonHeight);
        backButtonRect.anchorMin = new Vector2(0, 1);
        backButtonRect.anchorMax = new Vector2(0, 1);
        backButtonRect.anchoredPosition = new Vector2(buttonWidth * 0.7f, -buttonHeight * 0.7f);
        
        Button backButton = backButtonObj.AddComponent<Button>();
        Image backButtonImage = backButtonObj.AddComponent<Image>();
        backButtonImage.color = new Color(0.9f, 0.3f, 0.3f, 1f);
        
        // BackButton文本
        GameObject backTextObj = new GameObject("BackText");
        RectTransform backTextRect = backTextObj.AddComponent<RectTransform>();
        backTextRect.SetParent(backButtonRect, false);
        backTextRect.anchorMin = Vector2.zero;
        backTextRect.anchorMax = Vector2.one;
        backTextRect.offsetMin = Vector2.zero;
        backTextRect.offsetMax = Vector2.zero;
        
        Text backText = backTextObj.AddComponent<Text>();
        backText.text = "Back";
        backText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        backText.fontSize = Mathf.RoundToInt(buttonHeight * 0.4f);
        backText.color = Color.white;
        backText.alignment = TextAnchor.MiddleCenter;
        
        // 设置BackButton点击事件
        backButton.onClick.AddListener(() => {
            Debug.Log("=== Back button clicked from MainMenuUI! ===");
            Debug.Log($"UIManager.Instance exists: {UIManager.Instance != null}");
            
            // Try to find UIManager in the scene if Instance is null
            UIManager uiManager = UIManager.Instance;
            if (uiManager == null)
            {
                Debug.LogWarning("UIManager.Instance is null, searching for UIManager in scene...");
                uiManager = FindObjectOfType<UIManager>();
                Debug.Log($"Found UIManager in scene: {uiManager != null}");
            }
            
            if (uiManager != null)
            {
                Debug.Log("Calling uiManager.ShowPlatformListPage()");
                uiManager.ShowPlatformListPage();
            }
            else
            {
                Debug.LogError("No UIManager found! Cannot go back.");
                // Try direct navigation as backup
                TryDirectNavigation();
            }
        });
        
        Debug.Log($"Back button created in MainMenuUI - Size: {buttonWidth}x{buttonHeight}");
    }
    
    void TryDirectNavigation()
    {
        Debug.Log("Attempting direct navigation...");
        
        // Try to find and hide the current ad type page
        GameObject adTypePage = GameObject.Find("AdTypePage");
        if (adTypePage != null)
        {
            Debug.Log("Found AdTypePage, hiding it...");
            adTypePage.SetActive(false);
        }
        
        // Try to find and show the platform list page
        GameObject platformListPage = GameObject.Find("PlatformListPage");
        if (platformListPage != null)
        {
            Debug.Log("Found PlatformListPage, showing it...");
            platformListPage.SetActive(true);
        }
        else
        {
            Debug.LogWarning("PlatformListPage not found!");
        }
    }

    void CalculateButtonSize()
    {
        // 获取屏幕尺寸
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        
        // Button宽度为屏幕宽度的70%，高度为屏幕高度的20%
        float buttonWidth = screenWidth * 0.7f;
        float buttonHeight = screenHeight * 0.2f;
        
        // 设置最小和最大尺寸限制
        buttonWidth = Mathf.Clamp(buttonWidth, 450f, 900f);
        buttonHeight = Mathf.Clamp(buttonHeight, 140f, 250f);
        
        buttonSize = new Vector2(buttonWidth, buttonHeight);
        
        // Button间距为Button高度的40%
        buttonSpacing = buttonHeight * 0.4f;
        
        Debug.Log($"MainMenuUI - Screen: {Screen.width}x{Screen.height}, Button: {buttonSize}, Spacing: {buttonSpacing}");
    }

    void CreateAdButtons()
    {
        string[] buttonNames = {
            "BannerAd",
            "InterstitialAd", 
            "RewardedAd",
            "NativeAd",
            "SplashAd",
            "AppOpenAd"
        };

        string[] buttonLabels = {
            "Banner Ad",
            "Interstitial Ad",
            "Rewarded Ad",
            "Native Ad",
            "Splash Ad",
            "App Open Ad"
        };

        for (int i = 0; i < buttonNames.Length; i++)
        {
            CreateButton(buttonNames[i], buttonLabels[i], i, buttonNames.Length);
        }
    }

    void CreateButton(string buttonName, string buttonLabel, int index, int totalButtons)
    {
        GameObject buttonObj = new GameObject(buttonName + "Button");
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.SetParent(buttonContainer, false);
        buttonRect.sizeDelta = buttonSize;

        // 设置Button位置
        float yPos = (totalButtons - 1) * (buttonSize.y + buttonSpacing) / 2f - index * (buttonSize.y + buttonSpacing);
        buttonRect.anchoredPosition = new Vector2(0, yPos);

        // AddButton组件
        Button button = buttonObj.AddComponent<Button>();
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = buttonColor;

        // 创建Button文本
        GameObject textObj = new GameObject("Text");
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.SetParent(buttonRect, false);
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = buttonLabel;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = Mathf.RoundToInt(buttonSize.y * 0.45f);
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;

        // 设置Button点击效果
        ColorBlock colors = button.colors;
        colors.normalColor = buttonColor;
        colors.highlightedColor = buttonHoverColor;
        colors.pressedColor = new Color(0.1f, 0.5f, 0.9f, 1f);
        button.colors = colors;

        // 将Button引用设置到控制器
        SetButtonReference(buttonName, button, buttonText);
    }

    void SetButtonReference(string buttonName, Button button, Text buttonText)
    {
        if (menuController == null)
        {
            menuController = gameObject.AddComponent<MainMenuController>();
        }

        switch (buttonName)
        {
            case "BannerAd":
                menuController.bannerAdButton = button;
                menuController.bannerAdText = buttonText;
                break;
            case "InterstitialAd":
                menuController.interstitialAdButton = button;
                menuController.interstitialAdText = buttonText;
                break;
            case "RewardedAd":
                menuController.rewardedAdButton = button;
                menuController.rewardedAdText = buttonText;
                break;
            case "NativeAd":
                menuController.nativeAdButton = button;
                menuController.nativeAdText = buttonText;
                break;
            case "SplashAd":
                menuController.splashAdButton = button;
                menuController.splashAdText = buttonText;
                break;
            case "AppOpenAd":
                menuController.appOpenAdButton = button;
                menuController.appOpenAdText = buttonText;
                break;
        }
    }
}
