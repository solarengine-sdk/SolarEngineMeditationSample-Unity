using UnityEngine;
using SolarEngine;
using SolarEngineMeditationSample.AdMob;
using SolarEngineMeditationSample.IronSource;
using SolarEngineMeditationSample.Max;
using SolarEngineMeditationSample.Taku;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Manager References")]
    public UIManager uiManager;
    public SceneController sceneController;
    public ComingSoonUI comingSoonUI;
    public AdMobManager adMobManager;
    public IronSourceManager ironSourceManager;
    public MaxManager maxManager;
    public TakuManager takuManager;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeGame()
    {
        // 初始化UI管理器
        if (uiManager == null)
        {
            GameObject uiManagerObj = new GameObject("UIManager");
            uiManager = uiManagerObj.AddComponent<UIManager>();
            uiManagerObj.transform.SetParent(transform);
        }

        // 初始化场景控制器
        if (sceneController == null)
        {
            GameObject sceneControllerObj = new GameObject("SceneController");
            sceneController = sceneControllerObj.AddComponent<SceneController>();
            sceneControllerObj.transform.SetParent(transform);
        }

        // 初始化通知UI
        if (comingSoonUI == null)
        {
            GameObject comingSoonUIObj = new GameObject("ComingSoonUI");
            comingSoonUI = comingSoonUIObj.AddComponent<ComingSoonUI>();
            comingSoonUIObj.transform.SetParent(transform);
        }
        
        // 初始化AdMob管理器
        if (adMobManager == null)
        {
            // Try to find existing AdMobConfig or create one FIRST
            AdMobConfig config = FindObjectOfType<AdMobConfig>();
            if (config == null)
            {
                // Create a new AdMobConfig if none exists
                config = ScriptableObject.CreateInstance<AdMobConfig>();
                config.name = "AdMobConfig";
            }
            
            // Create the GameObject and add component
            GameObject adMobManagerObj = new GameObject("AdMobManager");
            adMobManager = adMobManagerObj.AddComponent<AdMobManager>();
            adMobManagerObj.transform.SetParent(transform);
            
            // Assign the config to AdMobManager AFTER component is added
            adMobManager.SetConfig(config);
            
            Debug.Log("AdMobManager created and initialized");
        }
        
        // 初始化IronSource管理器
        if (ironSourceManager == null)
        {
            // Create the GameObject and add component
            GameObject ironSourceManagerObj = new GameObject("IronSourceManager");
            ironSourceManager = ironSourceManagerObj.AddComponent<IronSourceManager>();
            ironSourceManagerObj.transform.SetParent(transform);
            
            Debug.Log("IronSourceManager created and initialized");
        }
        
        // 初始化Max管理器
        if (maxManager == null)
        {
            // Create the GameObject and add component
            GameObject maxManagerObj = new GameObject("MaxManager");
            maxManager = maxManagerObj.AddComponent<MaxManager>();
            maxManagerObj.transform.SetParent(transform);
            
            Debug.Log("MaxManager created and initialized");
        }

        // 初始化Taku管理器
        if (takuManager == null)
        {
            // Create the GameObject and add component
            GameObject takuManagerObj = new GameObject("TakuManager");
            takuManager = takuManagerObj.AddComponent<TakuManager>();
            takuManagerObj.transform.SetParent(transform);
            
            Debug.Log("TakuManager created and initialized");
        }

        // 设置目标帧率
        Application.targetFrameRate = 60;

        // 防止屏幕休眠
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

    /*How to init SolarEngine SDK? 
    Refer to https://help.solar-engine.com/en/docs/Unity-SDK-Integration-Guide
    */
        //Initialization
        string appkey = "YOUR_SOLARENGINE_APP_KEY";
        SolarEngine.Analytics.preInitSeSdk(appkey);    //PreInit should be called first.
        SEConfig seConfig = new SEConfig();
        seConfig.logEnabled = true;
        seConfig.initCompletedCallback = onInitCallback;     //Here put initCallback into SEConfig.
        SolarEngine.Analytics.initSeSdk(appkey, seConfig);      
    }

    //InitCallback
    private void onInitCallback(int code) {
            //please refer to the codes below
    }  
    void Start()
    {
        // UI管理器会自动创建所需的Page，无需手动创建
    }

    void OnApplicationPause(bool pauseStatus)
    {
        // Application paused
    }

    void OnApplicationFocus(bool hasFocus)
    {
        // Application focus changed
    }
}
