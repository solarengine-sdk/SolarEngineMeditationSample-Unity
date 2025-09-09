using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Pages")]
    public GameObject platformListPage;
    public GameObject adTypePage;
    public GameObject currentPage;

    [Header("Current Platform Info")]
    public string currentPlatform = "";

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("UIManager Instance set and DontDestroyOnLoad enabled");
        }
        else
        {
            Debug.LogWarning("UIManager Instance already exists, destroying duplicate");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Delay one frameShow平台列表Page，确保所有组件都已初始化
        StartCoroutine(ShowPlatformListPageDelayed());
    }

    System.Collections.IEnumerator ShowPlatformListPageDelayed()
    {
        yield return null; // Wait one frame
        ShowPlatformListPage();
    }

    public void ShowPlatformListPage()
    {
        Debug.Log("=== Switching to Platform List Page ===");
        
        // Hide所有 other pages
        if (adTypePage != null)
        {
            adTypePage.SetActive(false);
            Debug.Log("Hidden Ad Type Page");
            
            // Also hide the Canvas to ensure it's not visible
            Canvas adTypeCanvas = adTypePage.GetComponentInChildren<Canvas>();
            if (adTypeCanvas != null)
            {
                adTypeCanvas.enabled = false;
                adTypeCanvas.sortingOrder = 0; // Set to lowest priority
                Debug.Log("Disabled Ad Type Canvas and set sortingOrder to 0");
            }
        }
        
        if (currentPage != null && currentPage != platformListPage)
        {
            currentPage.SetActive(false);
            Debug.Log($"Hidden current page: {currentPage.name}");
        }

        // Show平台列表Page
        if (platformListPage == null)
        {
            CreatePlatformListPage();
            Debug.Log("Created new Platform List Page");
            // Don't try to show the page immediately - wait for PlatformListUI to be ready
            return;
        }

        // Check if the PlatformListUI is ready before trying to show the page
        PlatformListUI platformListUI = platformListPage.GetComponent<PlatformListUI>();
        if (platformListUI != null && platformListUI.IsUIReady())
        {
            // Check if the Canvas exists before activating the page
            Canvas platformCanvas = platformListPage.GetComponentInChildren<Canvas>();
            if (platformCanvas != null)
            {
                platformListPage.SetActive(true);
                currentPage = platformListPage;
                Debug.Log($"Platform List Page is now active: {platformListPage.activeInHierarchy}");
                Debug.Log($"Platform List Page activeSelf: {platformListPage.activeSelf}");
                Debug.Log($"Platform Canvas active: {platformCanvas.gameObject.activeInHierarchy}");
                Debug.Log($"Platform Canvas sortingOrder: {platformCanvas.sortingOrder}");
                
                // Force the Canvas to be visible and on top
                platformCanvas.enabled = true;
                platformCanvas.sortingOrder = 300; // Higher than Ad Type Page (200) to ensure it's on top
                Debug.Log($"Platform Canvas sortingOrder set to: {platformCanvas.sortingOrder}");
                
                // Additional debugging for Canvas visibility
                Debug.Log($"Platform Canvas renderMode: {platformCanvas.renderMode}");
                Debug.Log($"Platform Canvas overrideSorting: {platformCanvas.overrideSorting}");
                Debug.Log($"Platform Canvas gameObject active: {platformCanvas.gameObject.activeInHierarchy}");
                Debug.Log($"Platform Canvas gameObject activeSelf: {platformCanvas.gameObject.activeSelf}");
            }
            else
            {
                Debug.LogError("No Canvas found in Platform List Page! Cannot show page.");
                // Try to recreate the page
                Debug.Log("Attempting to recreate Platform List Page...");
                Destroy(platformListPage);
                platformListPage = null;
                CreatePlatformListPage();
            }
        }
        else
        {
            Debug.Log("PlatformListUI not ready yet, waiting for it to initialize...");
        }

        currentPlatform = "";
    }

    public void ShowAdTypePage(string platformName)
    {
        Debug.Log($"=== Switching to Ad Type Page for {platformName} ===");
        
        // Hide平台列表Page
        if (platformListPage != null)
        {
            platformListPage.SetActive(false);
            Debug.Log("Hidden Platform List Page");
        }
        
        if (currentPage != null && currentPage != adTypePage)
        {
            currentPage.SetActive(false);
            Debug.Log($"Hidden current page: {currentPage.name}");
        }

        // Show广告类型Page
        if (adTypePage == null)
        {
            CreateAdTypePage();
            Debug.Log("Created new Ad Type Page");
        }

        if (adTypePage != null)
        {
            adTypePage.SetActive(true);
            currentPage = adTypePage;
            currentPlatform = platformName;
            
            // Set the current platform in SceneController so it knows which ad manager to use
            if (GameManager.Instance != null && GameManager.Instance.sceneController != null)
            {
                GameManager.Instance.sceneController.SetCurrentPlatform(platformName);
                Debug.Log($"Set current platform in SceneController to: {platformName}");
            }
            
            Debug.Log($"Ad Type Page active: {adTypePage.activeInHierarchy}");
            Debug.Log($"Ad Type Page activeSelf: {adTypePage.activeSelf}");
            
            // 更新广告类型Page的Title
            UpdateAdTypePageTitle(platformName);
            
            // 确保广告类型Page在最上层
            Canvas adTypeCanvas = adTypePage.GetComponentInChildren<Canvas>();
            if (adTypeCanvas != null)
            {
                adTypeCanvas.sortingOrder = 200;
                Debug.Log($"Ad Type Canvas sortingOrder set to: {adTypeCanvas.sortingOrder}");
            }
            else
            {
                Debug.LogWarning("No Canvas found in Ad Type Page!");
            }
        }
    }

    void CreatePlatformListPage()
    {
        Debug.Log("=== Creating Platform List Page ===");
        
        // Clean up old page if it exists
        if (platformListPage != null)
        {
            Debug.Log($"Destroying old Platform List Page: {platformListPage.name}");
            DestroyImmediate(platformListPage);
            platformListPage = null;
        }
        
        GameObject pageObj = new GameObject("PlatformListPage");
        pageObj.transform.SetParent(transform);
        
        // Add平台列表UI组件
        pageObj.AddComponent<PlatformListUI>();
        
        platformListPage = pageObj;
        Debug.Log($"Platform List Page created: {pageObj.name}");
        Debug.Log($"Page parent: {pageObj.transform.parent?.name}");
        Debug.Log("PlatformListUI component added - it will notify UIManager when ready");
    }
    
    // Helper method to get transform hierarchy for debugging
    string GetTransformHierarchy(Transform transform, int depth = 0)
    {
        if (transform == null) return "null";
        
        string indent = new string(' ', depth * 2);
        string result = $"{indent}{transform.name}";
        
        for (int i = 0; i < transform.childCount; i++)
        {
            result += "\n" + GetTransformHierarchy(transform.GetChild(i), depth + 1);
        }
        
        return result;
    }
    

    
    // Callback method called by PlatformListUI when it's ready
    public void OnPlatformListUIReady()
    {
        Debug.Log("PlatformListUI notified UIManager that it's ready!");
        
        if (platformListPage != null)
        {
            Debug.Log($"PlatformListPage exists: {platformListPage.name}, active: {platformListPage.activeInHierarchy}");
            Debug.Log($"Page transform hierarchy: {GetTransformHierarchy(platformListPage.transform)}");
            
            // Try multiple ways to find the Canvas
            Canvas platformCanvas = platformListPage.GetComponentInChildren<Canvas>();
            if (platformCanvas == null)
            {
                Debug.Log("Canvas not found with GetComponentInChildren, trying direct search...");
                platformCanvas = platformListPage.GetComponent<Canvas>();
            }
            if (platformCanvas == null)
            {
                Debug.Log("Canvas not found on page itself, searching all children...");
                Transform[] allChildren = platformListPage.GetComponentsInChildren<Transform>();
                foreach (Transform child in allChildren)
                {
                    Canvas childCanvas = child.GetComponent<Canvas>();
                    if (childCanvas != null)
                    {
                        Debug.Log($"Found Canvas on child: {child.name}");
                        platformCanvas = childCanvas;
                        break;
                    }
                }
            }
            
            if (platformCanvas != null)
            {
                Debug.Log($"Platform Canvas confirmed ready: {platformCanvas.name}");
                Debug.Log($"Canvas active: {platformCanvas.gameObject.activeInHierarchy}, sortingOrder: {platformCanvas.sortingOrder}");
                Debug.Log($"Canvas transform: {platformCanvas.transform.position}, parent: {platformCanvas.transform.parent?.name}");
                
                // Show the page
                platformListPage.SetActive(true);
                currentPage = platformListPage;
                
                Debug.Log($"Platform List Page is now ACTIVE! Current page: {currentPage?.name}");
                Debug.Log($"Page active state: {platformListPage.activeInHierarchy}");
            }
            else
            {
                Debug.LogError("Platform Canvas not found in OnPlatformListUIReady!");
                Debug.LogError($"All children of {platformListPage.name}:");
                Transform[] allChildren = platformListPage.GetComponentsInChildren<Transform>();
                foreach (Transform child in allChildren)
                {
                    Debug.LogError($"  - {child.name} (active: {child.gameObject.activeInHierarchy})");
                }
            }
        }
        else
        {
            Debug.LogError("PlatformListPage is null in OnPlatformListUIReady!");
        }
    }

    void CreateAdTypePage()
    {
        Debug.Log("=== Creating Ad Type Page ===");
        GameObject pageObj = new GameObject("AdTypePage");
        pageObj.transform.SetParent(transform);
        
        // Add广告类型UI组件
        pageObj.AddComponent<MainMenuUI>();
        
        // Wait one frame让MainMenuUI完成初始化，然后AddBackButton
        StartCoroutine(CreateBackButtonDelayed(pageObj));
        
        adTypePage = pageObj;
        Debug.Log($"Ad Type Page created: {pageObj.name}");
    }
    
    System.Collections.IEnumerator CreateBackButtonDelayed(GameObject parent)
    {
        yield return null; // Wait one frame
        Debug.Log("Creating back button after delay...");
        CreateBackButton(parent);
        Debug.Log("Back button creation completed");
    }

    void CreateBackButton(GameObject parent)
    {
        Debug.Log($"CreateBackButton called with parent: {parent.name}");
        
        // 根据屏幕尺寸动态计算BackButton大小
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float buttonWidth = Mathf.Clamp(screenWidth * 0.12f, 150f, 250f);
        float buttonHeight = Mathf.Clamp(screenHeight * 0.08f, 80f, 120f);
        
        Debug.Log($"Screen: {screenWidth}x{screenHeight}, Button: {buttonWidth}x{buttonHeight}");
        
        // 创建BackButton
        GameObject backButtonObj = new GameObject("BackButton");
        RectTransform backButtonRect = backButtonObj.AddComponent<RectTransform>();
        
        // 检查parent是否有Canvas组件
        Canvas parentCanvas = parent.GetComponentInChildren<Canvas>();
        if (parentCanvas != null)
        {
            Debug.Log($"Found Canvas in parent: {parentCanvas.name}, setting back button as child");
            backButtonRect.SetParent(parentCanvas.transform, false);
        }
        else
        {
            Debug.Log("No Canvas found in parent, setting as direct child");
            backButtonRect.SetParent(parent.transform, false);
        }
        
        backButtonRect.sizeDelta = new Vector2(buttonWidth, buttonHeight);
        backButtonRect.anchorMin = new Vector2(0, 1);
        backButtonRect.anchorMax = new Vector2(0, 1);
        backButtonRect.anchoredPosition = new Vector2(buttonWidth * 0.7f, -buttonHeight * 0.7f);
        
        // 确保BackButton在最上层
        backButtonRect.SetAsLastSibling();

        Button backButton = backButtonObj.AddComponent<Button>();
        Image backButtonImage = backButtonObj.AddComponent<Image>();
        backButtonImage.color = new Color(0.9f, 0.3f, 0.3f, 1f); // 更亮的红色
        
        // Add边框效果
        backButtonImage.type = Image.Type.Sliced;

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
            Debug.Log("Back button clicked! Returning to platform list...");
            ShowPlatformListPage();
        });
        
        Debug.Log($"Back button created - Size: {buttonWidth}x{buttonHeight}, Position: {backButtonRect.anchoredPosition}");
    }

    void UpdateAdTypePageTitle(string platformName)
    {
        // 查找并更新PageTitle
        if (adTypePage != null)
        {
            // 查找现有的Title文本，如果没有则创建一个
            Text titleText = adTypePage.GetComponentInChildren<Text>();
            if (titleText == null)
            {
                CreatePageTitle(adTypePage, platformName);
            }
            else
            {
                titleText.text = $"{platformName} - 广告类型选择";
            }
        }
    }

    void CreatePageTitle(GameObject parent, string platformName)
    {
        GameObject titleObj = new GameObject("PageTitle");
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.SetParent(parent.transform, false);
        
        // 动态计算Title尺寸
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float titleWidth = Mathf.Clamp(screenWidth * 0.6f, 300f, 600f);
        float titleHeight = Mathf.Clamp(screenHeight * 0.08f, 50f, 100f);
        
        titleRect.sizeDelta = new Vector2(titleWidth, titleHeight);
        titleRect.anchorMin = new Vector2(0.5f, 1);
        titleRect.anchorMax = new Vector2(0.5f, 1);
        titleRect.anchoredPosition = new Vector2(0, -titleHeight * 0.8f);

        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = $"{platformName} - Ad Type Selection";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = Mathf.RoundToInt(titleHeight * 0.4f);
        titleText.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.fontStyle = FontStyle.Bold;
    }

    public void GoBack()
    {
        if (currentPage == adTypePage)
        {
            ShowPlatformListPage();
        }
        else
        {
            ShowPlatformListPage();
        }
    }
}
