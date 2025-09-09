using UnityEngine;

namespace SolarEngineMeditationSample.Gromore
{
    /// <summary>
    /// Configuration for Gromore (ByteDance Union) SDK
    /// </summary>
    [CreateAssetMenu(fileName = "GromoreConfig", menuName = "SolarEngine/Gromore Config")]
    public class GromoreConfig : ScriptableObject
    {
        [Header("SDK Configuration")]
        [SerializeField] private string appId = "";
        [SerializeField] private string appName = "";
        [SerializeField] private bool useMediation = true;
        [SerializeField] private bool allowShowNotify = true;
        [SerializeField] private bool debug = false;
        [SerializeField] private int themeStatus = 0;
        [SerializeField] private bool supportMultiProcess = false;
        [SerializeField] private int ageGroup = 0;
        [SerializeField] private bool paid = false;
        [SerializeField] private string keyWords = "";
        [SerializeField] private int titleBarTheme = 0;

        [Header("Ad Unit IDs")]
        [SerializeField] private string rewardedAdUnitId = "";
        [SerializeField] private string interstitialAdUnitId = "";
        [SerializeField] private string bannerAdUnitId = "";
        [SerializeField] private string nativeAdUnitId = "";
        [SerializeField] private string splashAdUnitId = "";
        [SerializeField] private string fullScreenVideoAdUnitId = "";

        [Header("Privacy Configuration")]
        [SerializeField] private bool canUseLocation = false;
        [SerializeField] private bool canUsePhoneState = false;
        [SerializeField] private bool canUseWifiState = false;
        [SerializeField] private bool canUseWriteExternalStorage = false;
        [SerializeField] private bool canUseReadPhoneState = false;
        [SerializeField] private bool canUseReadExternalStorage = false;
        [SerializeField] private bool canUseWriteExternalStorageUntilAppInstallFinish = false;
        [SerializeField] private bool canUseReadExternalStorageUntilAppInstallFinish = false;
        [SerializeField] private bool canUseInstallShortcutReceiver = false;
        [SerializeField] private bool canUseAccessWifiState = false;
        [SerializeField] private bool canUseAccessNetworkState = false;
        [SerializeField] private bool canUseAccessFineLocation = false;
        [SerializeField] private bool canUseAccessCoarseLocation = false;
        [SerializeField] private bool canUseVibrate = false;
        [SerializeField] private bool canUseCamera = false;
        [SerializeField] private bool canUseRecordAudio = false;
        [SerializeField] private bool canUseModifyAudioSettings = false;
        [SerializeField] private bool canUseReadLogs = false;
        [SerializeField] private bool canUseWriteLogs = false;
        [SerializeField] private bool canUseWakeLock = false;
        [SerializeField] private bool canUseGetTasks = false;
        [SerializeField] private bool canUseGetAccounts = false;
        [SerializeField] private bool canUseGetPackageSize = false;
        [SerializeField] private bool canUseGetInstalledPackages = false;
        [SerializeField] private bool canUseGetRunningTasks = false;
        [SerializeField] private bool canUseGetRunningAppProcesses = false;
        [SerializeField] private bool canUseGetDeviceId = false;
        [SerializeField] private bool canUseGetSubscriberId = false;
        [SerializeField] private bool canUseGetSimSerialNumber = false;
        [SerializeField] private bool canUseGetLine1Number = false;
        [SerializeField] private bool canUseGetNeighboringCellInfo = false;
        [SerializeField] private bool canUseGetAllCellInfo = false;
        [SerializeField] private bool canUseGetCellLocation = false;
        [SerializeField] private bool canUseGetLastKnownLocation = false;
        [SerializeField] private bool canUseGetLocation = false;
        [SerializeField] private bool canUseGetImei = false;
        [SerializeField] private bool canUseGetMacAddress = false;
        [SerializeField] private bool canUseGetAndroidId = false;
        [SerializeField] private bool canUseGetSerial = false;
        [SerializeField] private bool canUseGetBuildSerial = false;
        [SerializeField] private bool canUseGetBuildFingerprint = false;
        [SerializeField] private bool canUseGetBuildHardware = false;
        [SerializeField] private bool canUseGetBuildProduct = false;
        [SerializeField] private bool canUseGetBuildBrand = false;
        [SerializeField] private bool canUseGetBuildDevice = false;
        [SerializeField] private bool canUseGetBuildModel = false;
        [SerializeField] private bool canUseGetBuildManufacturer = false;
        [SerializeField] private bool canUseGetBuildHost = false;
        [SerializeField] private bool canUseGetBuildUser = false;
        [SerializeField] private bool canUseGetBuildDisplay = false;
        [SerializeField] private bool canUseGetBuildId = false;
        [SerializeField] private bool canUseGetBuildTags = false;
        [SerializeField] private bool canUseGetBuildType = false;
        [SerializeField] private bool canUseGetBuildTime = false;

        // Properties
        public string AppId => appId;
        public string AppName => appName;
        public bool UseMediation => useMediation;
        public bool AllowShowNotify => allowShowNotify;
        public bool Debug => debug;
        public int ThemeStatus => themeStatus;
        public bool SupportMultiProcess => supportMultiProcess;
        public int AgeGroup => ageGroup;
        public bool Paid => paid;
        public string KeyWords => keyWords;
        public int TitleBarTheme => titleBarTheme;

        // Ad Unit IDs
        public string RewardedAdUnitId => rewardedAdUnitId;
        public string InterstitialAdUnitId => interstitialAdUnitId;
        public string BannerAdUnitId => bannerAdUnitId;
        public string NativeAdUnitId => nativeAdUnitId;
        public string SplashAdUnitId => splashAdUnitId;
        public string FullScreenVideoAdUnitId => fullScreenVideoAdUnitId;

        // Privacy Configuration
        public bool CanUseLocation => canUseLocation;
        public bool CanUsePhoneState => canUsePhoneState;
        public bool CanUseWifiState => canUseWifiState;
        public bool CanUseWriteExternalStorage => canUseWriteExternalStorage;
        public bool CanUseReadPhoneState => canUseReadPhoneState;
        public bool CanUseReadExternalStorage => canUseReadExternalStorage;
        public bool CanUseWriteExternalStorageUntilAppInstallFinish => canUseWriteExternalStorageUntilAppInstallFinish;
        public bool CanUseReadExternalStorageUntilAppInstallFinish => canUseReadExternalStorageUntilAppInstallFinish;
        public bool CanUseInstallShortcutReceiver => canUseInstallShortcutReceiver;
        public bool CanUseAccessWifiState => canUseAccessWifiState;
        public bool CanUseAccessNetworkState => canUseAccessNetworkState;
        public bool CanUseAccessFineLocation => canUseAccessFineLocation;
        public bool CanUseAccessCoarseLocation => canUseAccessCoarseLocation;
        public bool CanUseVibrate => canUseVibrate;
        public bool CanUseCamera => canUseCamera;
        public bool CanUseRecordAudio => canUseRecordAudio;
        public bool CanUseModifyAudioSettings => canUseModifyAudioSettings;
        public bool CanUseReadLogs => canUseReadLogs;
        public bool CanUseWriteLogs => canUseWriteLogs;
        public bool CanUseWakeLock => canUseWakeLock;
        public bool CanUseGetTasks => canUseGetTasks;
        public bool CanUseGetAccounts => canUseGetAccounts;
        public bool CanUseGetPackageSize => canUseGetPackageSize;
        public bool CanUseGetInstalledPackages => canUseGetInstalledPackages;
        public bool CanUseGetRunningTasks => canUseGetRunningTasks;
        public bool CanUseGetRunningAppProcesses => canUseGetRunningAppProcesses;
        public bool CanUseGetDeviceId => canUseGetDeviceId;
        public bool CanUseGetSubscriberId => canUseGetSubscriberId;
        public bool CanUseGetSimSerialNumber => canUseGetSimSerialNumber;
        public bool CanUseGetLine1Number => canUseGetLine1Number;
        public bool CanUseGetNeighboringCellInfo => canUseGetNeighboringCellInfo;
        public bool CanUseGetAllCellInfo => canUseGetAllCellInfo;
        public bool CanUseGetCellLocation => canUseGetCellLocation;
        public bool CanUseGetLastKnownLocation => canUseGetLastKnownLocation;
        public bool CanUseGetLocation => canUseGetLocation;
        public bool CanUseGetImei => canUseGetImei;
        public bool CanUseGetMacAddress => canUseGetMacAddress;
        public bool CanUseGetAndroidId => canUseGetAndroidId;
        public bool CanUseGetSerial => canUseGetSerial;
        public bool CanUseGetBuildSerial => canUseGetBuildSerial;
        public bool CanUseGetBuildFingerprint => canUseGetBuildFingerprint;
        public bool CanUseGetBuildHardware => canUseGetBuildHardware;
        public bool CanUseGetBuildProduct => canUseGetBuildProduct;
        public bool CanUseGetBuildBrand => canUseGetBuildBrand;
        public bool CanUseGetBuildDevice => canUseGetBuildDevice;
        public bool CanUseGetBuildModel => canUseGetBuildModel;
        public bool CanUseGetBuildManufacturer => canUseGetBuildManufacturer;
        public bool CanUseGetBuildHost => canUseGetBuildHost;
        public bool CanUseGetBuildUser => canUseGetBuildUser;
        public bool CanUseGetBuildDisplay => canUseGetBuildDisplay;
        public bool CanUseGetBuildId => canUseGetBuildId;
        public bool CanUseGetBuildTags => canUseGetBuildTags;
        public bool CanUseGetBuildType => canUseGetBuildType;
        public bool CanUseGetBuildTime => canUseGetBuildTime;

        /// <summary>
        /// Check if the configuration is valid
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(appId) && !string.IsNullOrEmpty(appName);
        }

        /// <summary>
        /// Get configuration status for debugging
        /// </summary>
        public string GetConfigStatus()
        {
            if (!IsValid())
                return "GromoreConfig: INVALID - Missing App ID or App Name";
            
            return $"GromoreConfig: App ID={appId}, App Name={appName}, Mediation={useMediation}, Debug={debug}";
        }

        /// <summary>
        /// Set default values for the configuration
        /// </summary>
        public void SetDefaultValues()
        {
            appId = "your_gromore_app_id_here";
            appName = "Your App Name";
            useMediation = true;
            allowShowNotify = true;
            debug = true; // Enable debug for development
            themeStatus = 0;
            supportMultiProcess = false;
            ageGroup = 0;
            paid = false;
            keyWords = "";
            titleBarTheme = 0;

            // Ad Unit IDs
            rewardedAdUnitId = "your_rewarded_ad_unit_id";
            interstitialAdUnitId = "your_interstitial_ad_unit_id";
            bannerAdUnitId = "your_banner_ad_unit_id";
            nativeAdUnitId = "your_native_ad_unit_id";
            splashAdUnitId = "your_splash_ad_unit_id";
            fullScreenVideoAdUnitId = "your_fullscreen_video_ad_unit_id";

            // Privacy Configuration - Set to false by default for privacy
            canUseLocation = false;
            canUsePhoneState = false;
            canUseWifiState = false;
            canUseWriteExternalStorage = false;
            canUseReadPhoneState = false;
            canUseReadExternalStorage = false;
            canUseWriteExternalStorageUntilAppInstallFinish = false;
            canUseReadExternalStorageUntilAppInstallFinish = false;
            canUseInstallShortcutReceiver = false;
            canUseAccessWifiState = false;
            canUseAccessNetworkState = false;
            canUseAccessFineLocation = false;
            canUseAccessCoarseLocation = false;
            canUseVibrate = false;
            canUseCamera = false;
            canUseRecordAudio = false;
            canUseModifyAudioSettings = false;
            canUseReadLogs = false;
            canUseWriteLogs = false;
            canUseWakeLock = false;
            canUseGetTasks = false;
            canUseGetAccounts = false;
            canUseGetPackageSize = false;
            canUseGetInstalledPackages = false;
            canUseGetRunningTasks = false;
            canUseGetRunningAppProcesses = false;
            canUseGetDeviceId = false;
            canUseGetSubscriberId = false;
            canUseGetSimSerialNumber = false;
            canUseGetLine1Number = false;
            canUseGetNeighboringCellInfo = false;
            canUseGetAllCellInfo = false;
            canUseGetCellLocation = false;
            canUseGetLastKnownLocation = false;
            canUseGetLocation = false;
            canUseGetImei = false;
            canUseGetMacAddress = false;
            canUseGetAndroidId = false;
            canUseGetSerial = false;
            canUseGetBuildSerial = false;
            canUseGetBuildFingerprint = false;
            canUseGetBuildHardware = false;
            canUseGetBuildProduct = false;
            canUseGetBuildBrand = false;
            canUseGetBuildDevice = false;
            canUseGetBuildModel = false;
            canUseGetBuildManufacturer = false;
            canUseGetBuildHost = false;
            canUseGetBuildUser = false;
            canUseGetBuildDisplay = false;
            canUseGetBuildId = false;
            canUseGetBuildTags = false;
            canUseGetBuildType = false;
            canUseGetBuildTime = false;
        }

        /// <summary>
        /// Get setup instructions for the user
        /// </summary>
        public string GetSetupInstructions()
        {
            return "Gromore SDK Setup:\n" +
                   "1. Set your App ID from ByteDance Union console\n" +
                   "2. Set your App Name\n" +
                   "3. Configure ad unit IDs for each ad type\n" +
                   "4. Adjust privacy settings as needed\n" +
                   "5. Set debug=true during development, false for production";
        }
    }
}
