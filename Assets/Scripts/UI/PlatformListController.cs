using UnityEngine;
using UnityEngine.UI;

public class PlatformListController : MonoBehaviour
{
    [Header("Platform Buttons")]
    public Button maxButton;
    public Button gromoreButton;
    public Button ironSourceButton;
    public Button adMobButton;
    public Button takuButton;
    public Button topOnButton;

    [Header("Platform Button Texts")]
    public Text maxText;
    public Text gromoreText;
    public Text ironSourceText;
    public Text adMobText;
    public Text takuText;
    public Text topOnText;

    void Start()
    {
        // 延迟设置Button，确保所有Button都已创建
        StartCoroutine(SetupButtonsDelayed());
    }

    System.Collections.IEnumerator SetupButtonsDelayed()
    {
        yield return new WaitForEndOfFrame(); // 等待当前帧结束
        SetupPlatformButtons();
    }

    // 公共方法，供外部调用
    public void ForceSetupButtons()
    {
        SetupPlatformButtons();
    }

    void SetupPlatformButtons()
    {
        // Max Button
        if (maxButton != null)
        {
            Debug.Log("Setting up Max button click listener");
            
            maxButton.onClick.AddListener(() => {
                Debug.Log("Max Platform Button Clicked");
                if (UIManager.Instance != null)
                {
                    Debug.Log("UIManager.Instance found, calling ShowAdTypePage");
                    UIManager.Instance.ShowAdTypePage("Max");
                }
                else
                {
                    Debug.LogError("UIManager.Instance is null!");
                }
            });
            

            
        }

        // Gromore Button
        if (gromoreButton != null)
        {
            gromoreButton.onClick.AddListener(() => {
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowAdTypePage("Gromore");
                }
            });
        }

        // IronSource Button
        if (ironSourceButton != null)
        {
            ironSourceButton.onClick.AddListener(() => {
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowAdTypePage("IronSource");
                }
            });
        }

        // AdMob Button
        if (adMobButton != null)
        {
            adMobButton.onClick.AddListener(() => {
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowAdTypePage("AdMob");
                }
            });
        }

        // Taku Button
        if (takuButton != null)
        {
            takuButton.onClick.AddListener(() => {
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowAdTypePage("Taku");
                }
            });
        }

        // TopOn Button
        if (topOnButton != null)
        {
            topOnButton.onClick.AddListener(() => {
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowAdTypePage("TopOn");
                }
            });
        }
        

    }
}
