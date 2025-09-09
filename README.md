## SolarEngine Meditation Sample – Unity

### Why this sample exists

- Demonstrate end‑to‑end integration of multiple ad mediation SDKs (MAX, AdMob, Gromore, IronSource, Taku, TopOn) in a Unity project.
- Show a consistent wrapper pattern to isolate SDK specifics and report ad revenue/impressions to SolarEngine through one API.
- Provide minimal, runnable examples for common ad types (Rewarded, Interstitial, Banner, Native, Splash/App Open) and where to hook revenue callbacks in Unity.

### What the wrappers are for

- Encapsulate each network’s SDK events/listeners and map revenue/impression data to SolarEngine’s `Analytics.trackAdImpression`.
- Normalize parameters (ad type enums, ad unit/placement info, currency, eCPM) across networks before sending to SolarEngine.
- Keep managers/controllers clean: managers trigger load/show; wrappers handle SDK events and tracking.

### Key points by network

- AdMob: handle OnPaid event → use `AdMobAdWrapper.Build*AdPaidEventHandler`.
- MAX: handle per‑ad‑type OnAdRevenuePaid → call the matching `Max*AdWrapper`.
- Gromore: on ad show → call `GromoreAdWrapper.track*AdImpression` (wrapper reads eCPM internally).
- IronSource: use `LevelPlayImpressionData` → call `IronSourceWrapper`.
- Taku/TopOn: in show callbacks → call `TakuAdWrapper.Track*AdRevenue` / `TopOnAdWrapper.Track*AdRevenue`.

### Project layout (high‑level)

- `Assets/Scripts/`
  - `Wrappers/` Wrapper + tracker classes per mediation (C#)
  - `AdMob/`, `Max/`, `Gromore/`, `IronSource/`, `Taku/`, `TopOn/` manager scripts
  - `Resources/` ScriptableObject configs (e.g., Taku/Gromore configs)
  - `UI/` simple demo UI

### How to use in your app

1) Integrate the mediation SDK you choose (packages/plugins per vendor) following that SDK’s Unity guide.
2) Use this sample as a reference for wiring each SDK’s C# events/listeners.
3) When revenue/impression callbacks fire, call the corresponding wrapper:
   - AdMob: OnPaid ➜ `AdMobAdWrapper.Build*AdPaidEventHandler`
     
     Example (Rewarded):
     ```csharp   
     // After loading rewardedAd
     rewardedAd.OnAdPaid += AdMobAdWrapper.BuildRewardedAdPaidEventHandler(
         userCallback: null,
         responseInfo: rewardedAd.GetResponseInfo());
     ```
   - MAX: ad revenue per ad type ➜ `Max*AdWrapper`
     
     Example (Rewarded):
     ```csharp
      MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += MaxRewardedAdWrapper.BuildAdRevenueHandler(OnAdRevenuePaid);    
     ```
   - Gromore: on ad show ➜ `GromoreAdWrapper.track*AdImpression`

      Example (Rewarded):
      ```csharp
        #region IRewardAdInteractionListener Implementation

        void IRewardAdInteractionListener.OnAdShow()
        {
            Debug.Log("GromoreManager: Rewarded ad shown");
            
            string placementId = "";
            if (config != null)
            {
                placementId = config.RewardedAdUnitId;
            }
            // Use wrapper to track ad revenue and impression
            MediationAdEcpmInfo ecpmInfo = null;
            if (rewardedAd != null)
            {
                ecpmInfo = rewardedAd.GetMediationManager().GetShowEcpm();
            }
            GromoreAdWrapper.TrackRewardedAdRevenue(placementId, ecpmInfo);
            
            OnRewardedAdDisplayed?.Invoke();
        }
        ```
   - IronSource: LevelPlay impression data ➜ `IronSourceWrapper`
      Example:
      ```csharp
          LevelPlay.OnImpressionDataReady += IronSourceWrapper.BuildImpressionDataHandler();
      ```
   - Taku/TopOn: callback info ➜ `TakuAdWrapper` / `TopOnAdWrapper`
      Example (Rewarded):

      ```csharp
        #region ATRewardedVideoListener Implementation

        public void onRewardedVideoAdPlayStart(string placementId, ATCallbackInfo callbackInfo)
        {
            Debug.Log($"TakuManager: Rewarded video ad play started for placement: {placementId}");
            OnRewardedAdDisplayed?.Invoke();
            
            // Track revenue using wrapper
            TakuAdWrapper.TrackRewardedAdRevenue(callbackInfo);
        }
      ```
4) Keep your game code focused on load/show logic; let wrappers normalize and report to SolarEngine.

### Out of scope

- Production error handling, consent/ATT/UMP flows, and complete monetization strategy. This sample focuses on wiring and tracking.

### Notes

- appID/appKey and unit IDs are placeholders.
- Replace test IDs with your real IDs before release.


