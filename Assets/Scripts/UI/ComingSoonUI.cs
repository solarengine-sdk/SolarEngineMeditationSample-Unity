using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ComingSoonUI : MonoBehaviour
{
    public static ComingSoonUI Instance;

    [Header("UI Components")]
    public Canvas notificationCanvas;
    public Text messageText;
    public Button closeButton;
    
    [Header("Settings")]
    public float displayDuration = 2f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreateNotificationUI();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void CreateNotificationUI()
    {
        // 创建通知Canvas
        if (notificationCanvas == null)
        {
            GameObject canvasObj = new GameObject("NotificationCanvas");
            notificationCanvas = canvasObj.AddComponent<Canvas>();
            notificationCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            notificationCanvas.sortingOrder = 999; // Ensure on top
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            canvasObj.transform.SetParent(transform);
        }

        // 创建背景
        GameObject backgroundObj = new GameObject("Background");
        RectTransform backgroundRect = backgroundObj.AddComponent<RectTransform>();
        backgroundRect.SetParent(notificationCanvas.transform, false);
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;

        Image backgroundImage = backgroundObj.AddComponent<Image>();
        backgroundImage.color = new Color(0, 0, 0, 0.7f); // 半透明黑色背景

        // 创建消息面板
        GameObject panelObj = new GameObject("MessagePanel");
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.SetParent(backgroundRect, false);
        
        // 动态计算面板大小
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float panelWidth = Mathf.Clamp(screenWidth * 0.6f, 500f, 1000f);
        float panelHeight = Mathf.Clamp(screenHeight * 0.4f, 300f, 600f);
        
        panelRect.sizeDelta = new Vector2(panelWidth, panelHeight);
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;

        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = Color.white;

        // 创建消息文本
        GameObject textObj = new GameObject("MessageText");
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.SetParent(panelRect, false);
        textRect.sizeDelta = new Vector2(panelWidth * 0.8f, panelHeight * 0.5f);
        textRect.anchoredPosition = new Vector2(0, panelHeight * 0.1f);

        messageText = textObj.AddComponent<Text>();
        messageText.text = "Coming Soon!";
        messageText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        messageText.fontSize = Mathf.RoundToInt(panelHeight * 0.15f);
        messageText.color = Color.black;
        messageText.alignment = TextAnchor.MiddleCenter;

        // 创建关闭Button
        GameObject buttonObj = new GameObject("CloseButton");
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.SetParent(panelRect, false);
        
        float buttonWidth = Mathf.Clamp(panelWidth * 0.25f, 120f, 250f);
        float buttonHeight = Mathf.Clamp(panelHeight * 0.15f, 60f, 100f);
        
        buttonRect.sizeDelta = new Vector2(buttonWidth, buttonHeight);
        buttonRect.anchoredPosition = new Vector2(0, -panelHeight * 0.3f);

        closeButton = buttonObj.AddComponent<Button>();
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.6f, 1f, 1f);

        // Button文本
        GameObject buttonTextObj = new GameObject("ButtonText");
        RectTransform buttonTextRect = buttonTextObj.AddComponent<RectTransform>();
        buttonTextRect.SetParent(buttonRect, false);
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;

        Text buttonText = buttonTextObj.AddComponent<Text>();
        buttonText.text = "OK";
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = Mathf.RoundToInt(buttonHeight * 0.4f);
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;

        // 设置Button点击事件
        closeButton.onClick.AddListener(Hide);

        // 默认Hide
        notificationCanvas.gameObject.SetActive(false);
    }

    public void Show(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }

        if (notificationCanvas != null)
        {
            notificationCanvas.gameObject.SetActive(true);
            StartCoroutine(AutoHide());
        }
    }

    public void Hide()
    {
        if (notificationCanvas != null)
        {
            notificationCanvas.gameObject.SetActive(false);
        }
    }

    private IEnumerator AutoHide()
    {
        yield return new WaitForSeconds(displayDuration);
        Hide();
    }
}
