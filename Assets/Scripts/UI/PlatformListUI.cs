using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlatformListUI : MonoBehaviour
{
    [Header("UI References")]
    public Canvas mainCanvas;
    public RectTransform platformContainer;
    public Button platformButtonPrefab;

    [Header("UI Settings")]
    public float buttonSpacing = 30f;
    public Vector2 buttonSize;
    public Color buttonColor = new Color(0.1f, 0.4f, 0.8f, 1f);
    public Color buttonHoverColor = new Color(0.2f, 0.5f, 0.9f, 1f);

    private PlatformListController platformController;

    void Start()
    {
        Debug.Log("PlatformListUI Start called");
        // 根据屏幕尺寸动态计算Button大小
        CalculateButtonSize();
        CreatePlatformListUI();
        Debug.Log("PlatformListUI Start completed");
    }

    void CalculateButtonSize()
    {
        // 获取屏幕尺寸
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        
        // Button宽度为屏幕宽度的60%，高度为屏幕高度的18%
        float buttonWidth = screenWidth * 0.6f;
        float buttonHeight = screenHeight * 0.18f;
        
        // 设置最小和最大尺寸限制
        buttonWidth = Mathf.Clamp(buttonWidth, 400f, 800f);
        buttonHeight = Mathf.Clamp(buttonHeight, 120f, 200f);
        
        buttonSize = new Vector2(buttonWidth, buttonHeight);
        
        // Button间距为Button高度的25%
        buttonSpacing = buttonHeight * 0.25f;
        
        Debug.Log($"PlatformListUI - Screen: {Screen.width}x{Screen.height}, Button: {buttonSize}, Spacing: {buttonSpacing}");
    }

    void CreatePlatformListUI()
    {
        // 确保有Canvas
        if (mainCanvas == null)
        {
            GameObject canvasObj = new GameObject("PlatformCanvas");
            
            // 重要：将Canvas设置为PlatformListPage的子对象
            canvasObj.transform.SetParent(transform, false);
            
            mainCanvas = canvasObj.AddComponent<Canvas>();
            mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            // AddCanvasScaler
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            // AddGraphicRaycaster
            GraphicRaycaster raycaster = canvasObj.AddComponent<GraphicRaycaster>();
            raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
            
            // 确保GraphicRaycaster设置正确
            raycaster.enabled = true;
            
            // 设置Canvas属性
            mainCanvas.sortingOrder = 100; // Ensure on top
            mainCanvas.overrideSorting = true;
            
            // 确保Canvas可见
            mainCanvas.enabled = true;
            
            Debug.Log($"Platform Canvas created - sortingOrder: {mainCanvas.sortingOrder}, enabled: {mainCanvas.enabled}");
            Debug.Log($"Canvas parent: {mainCanvas.transform.parent?.name}");
            
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
            
            Debug.Log("PlatformListUI setup completed - Canvas and background ready!");
            
            // Wait one frame确保Canvas完全初始化，然后通知UIManager
            StartCoroutine(NotifyUIManagerDelayed());
        }
        
        // 检查EventSystem
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
        }
        else
        {
            Debug.Log("PlatformCanvas already exists");
        }

        // 创建平台容器
        if (platformContainer == null)
        {
            GameObject containerObj = new GameObject("PlatformContainer");
            platformContainer = containerObj.AddComponent<RectTransform>();
            platformContainer.SetParent(mainCanvas.transform, false);
            platformContainer.anchorMin = new Vector2(0.5f, 0.5f);
            platformContainer.anchorMax = new Vector2(0.5f, 0.5f);
            platformContainer.anchoredPosition = Vector2.zero;
        }

        // 创建平台Button
        CreatePlatformButtons();

        // Add控制器
        if (platformController == null)
        {
            platformController = gameObject.AddComponent<PlatformListController>();
        }
    }
    
    System.Collections.IEnumerator NotifyUIManagerDelayed()
    {
        yield return null; // Wait one frame for Canvas to fully initialize
        Debug.Log("Notifying UIManager that PlatformListUI is ready...");
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OnPlatformListUIReady();
        }
        else
        {
            Debug.LogError("UIManager.Instance is null when trying to notify!");
        }
    }

    void CreatePlatformButtons()
    {
        string[] platformNames = {
            "Max",
            "Gromore", 
            "IronSource",
            "AdMob",
            "Taku",
            "TopOn"
        };

        string[] platformLabels = {
            "Max",
            "Gromore",
            "IronSource",
            "AdMob",
            "Taku",
            "TopOn"
        };

        for (int i = 0; i < platformNames.Length; i++)
        {
            CreatePlatformButton(platformNames[i], platformLabels[i], i, platformNames.Length);
        }
    }

    void CreatePlatformButton(string platformName, string platformLabel, int index, int totalPlatforms)
    {
        GameObject buttonObj = new GameObject(platformName + "PlatformButton");
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.SetParent(platformContainer, false);
        buttonRect.sizeDelta = buttonSize;

        // 设置Button位置
        float yPos = (totalPlatforms - 1) * (buttonSize.y + buttonSpacing) / 2f - index * (buttonSize.y + buttonSpacing);
        buttonRect.anchoredPosition = new Vector2(0, yPos);

        // AddButton组件
        Button button = buttonObj.AddComponent<Button>();
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = buttonColor;
        
        // 确保Button可以接收点击事件
        buttonImage.raycastTarget = true;
        button.interactable = true;

        // 创建Button文本
        GameObject textObj = new GameObject("Text");
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.SetParent(buttonRect, false);
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = platformLabel;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = Mathf.RoundToInt(buttonSize.y * 0.35f);
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;

        // 设置Button点击效果
        ColorBlock colors = button.colors;
        colors.normalColor = buttonColor;
        colors.highlightedColor = buttonHoverColor;
        colors.pressedColor = new Color(0.1f, 0.5f, 0.9f, 1f);
        button.colors = colors;

        // 将Button引用设置到控制器
        SetPlatformButtonReference(platformName, button, buttonText);
    }

    void SetPlatformButtonReference(string platformName, Button button, Text buttonText)
    {
        if (platformController == null)
        {
            platformController = gameObject.AddComponent<PlatformListController>();
        }

        switch (platformName)
        {
            case "Max":
                platformController.maxButton = button;
                platformController.maxText = buttonText;
                break;
            case "Gromore":
                platformController.gromoreButton = button;
                platformController.gromoreText = buttonText;
                break;
            case "IronSource":
                platformController.ironSourceButton = button;
                platformController.ironSourceText = buttonText;
                break;
            case "AdMob":
                platformController.adMobButton = button;
                platformController.adMobText = buttonText;
                break;
            case "Taku":
                platformController.takuButton = button;
                platformController.takuText = buttonText;
                break;
            case "TopOn":
                platformController.topOnButton = button;
                platformController.topOnText = buttonText;
                break;
        }
    }
    
    // Public method to check if UI is ready
    public bool IsUIReady()
    {
        return mainCanvas != null && mainCanvas.enabled;
    }
}
