using UnityEngine;
using SolarEngineMeditationSample.IronSource;

namespace SolarEngineMeditationSample.IronSource
{
    /// <summary>
    /// Sample ad manager for IronSource integration
    /// </summary>
    public class IronSourceSampleAdManager : MonoBehaviour
    {
        [Header("IronSource Manager Reference")]
        [SerializeField] private IronSourceManager ironSourceManager;
        
        [Header("UI Integration")]
        [SerializeField] private bool showStatusInConsole = true;
        
        private void Start()
        {
            // Find IronSource manager if not assigned
            if (ironSourceManager == null)
            {
                ironSourceManager = IronSourceManager.Instance;
                if (ironSourceManager == null)
                {
                    Debug.LogWarning("IronSource: IronSourceManager not found. Creating one...");
                    CreateIronSourceManager();
                }
            }
            
            // Subscribe to initialization events
            IronSourceManager.OnInitializationSuccess += OnIronSourceInitialized;
            IronSourceManager.OnInitializationFailed += OnIronSourceInitializationFailed;
            
            if (showStatusInConsole)
                Debug.Log("IronSource: Sample ad manager initialized");
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            IronSourceManager.OnInitializationSuccess -= OnIronSourceInitialized;
            IronSourceManager.OnInitializationFailed -= OnIronSourceInitializationFailed;
        }
        
        #region IronSource Event Handlers
        
        private void OnIronSourceInitialized()
        {
            if (showStatusInConsole)
                Debug.Log("IronSource: SDK initialized successfully in sample manager");
        }
        
        private void OnIronSourceInitializationFailed(string error)
        {
            if (showStatusInConsole)
                Debug.LogError($"IronSource: SDK initialization failed in sample manager: {error}");
        }
        
        #endregion
        
        #region Public API for UI Integration
        
        /// <summary>
        /// Check if IronSource is ready to show ads
        /// </summary>
        public bool IsIronSourceReady()
        {
            return ironSourceManager != null && ironSourceManager.IsInitialized;
        }
        
        /// <summary>
        /// Get IronSource initialization status
        /// </summary>
        public string GetIronSourceStatus()
        {
            if (ironSourceManager == null)
                return "IronSource Manager not found";
            
            if (!ironSourceManager.IsInitialized)
                return "IronSource not initialized";
            
            return "IronSource ready";
        }
        
        /// <summary>
        /// Show IronSource test suite
        /// </summary>
        public void ShowIronSourceTestSuite()
        {
            if (IsIronSourceReady())
            {
                if (showStatusInConsole)
                    Debug.Log("IronSource: Launching test suite...");
                // The test suite is automatically launched after initialization
            }
            else
            {
                Debug.LogWarning("IronSource: Cannot show test suite - SDK not ready");
            }
        }
        
        /// <summary>
        /// Get IronSource manager instance
        /// </summary>
        public IronSourceManager GetIronSourceManager()
        {
            return ironSourceManager;
        }
        
        #endregion
        
        #region Private Helper Methods
        
        private void CreateIronSourceManager()
        {
            GameObject managerObj = new GameObject("IronSourceManager");
            ironSourceManager = managerObj.AddComponent<IronSourceManager>();
            
            if (showStatusInConsole)
                Debug.Log("IronSource: Created new IronSourceManager instance");
        }
        
        #endregion
    }
}
