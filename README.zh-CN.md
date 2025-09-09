## SolarEngine 冥想示例 – Unity

### 为什么有这个示例

- 在一个 Unity 工程中演示多种聚合平台（MAX、AdMob、Gromore、IronSource、Taku、TopOn）的端到端接入。
- 展示统一的 Wrapper 模式：拦截各平台收益/曝光回调，并用一个 API 上报到 SolarEngine。
- 提供常见广告类型（激励、插屏、横幅、原生、开屏/App Open）的最小可运行示例与接入位置。

### Wrapper 的作用

- 封装各平台 C# 事件/监听，归一化参数（广告类型、广告位/单元、货币、eCPM），调用 `SolarEngine.Analytics.trackAdImpression`。
- 保持业务层只负责加载/展示，Wrapper 负责拦截与上报。

### 各平台要点

- AdMob：处理 OnPaid 事件 → 使用 `AdMobAdWrapper.Build*AdPaidEventHandler`。
- MAX：处理各广告类型的 OnAdRevenuePaid → 调用相应 `Max*AdWrapper`。
- Gromore：在广告显示回调中调用 `GromoreAdWrapper.track*AdImpression`（Wrapper 内部读取 eCPM）。
- IronSource：使用 `LevelPlayImpressionData` → 调用 `IronSourceWrapper`。
- Taku/TopOn：在展示回调中调用 `TakuAdWrapper.Track*AdRevenue` / `TopOnAdWrapper.Track*AdRevenue`。

### 目录结构（概览）

- `Assets/Scripts/`
  - `Wrappers/` 各聚合平台的 Wrapper + Tracker（C#）
  - 各平台管理脚本：`AdMob/`、`Max/`、`Gromore/`、`IronSource/`、`Taku/`、`TopOn/`
  - `Resources/` 可序列化配置（如 Taku/Gromore 配置）
  - `UI/` 简单演示 UI

### 如何使用

1) 按官方指南导入所选聚合 SDK（Unity 插件/Package）。
2) 参考本示例绑定各 SDK 的 C# 事件/监听。(您可以从[这里](https://github.com/solarengine-sdk/SolarEngineMeditationSample-Unity/blob/main/Wrappers.zip)下载各聚合平台的wrapper,从中移除您不需要的文件,仅保留您要使用的聚合对应的wrapper文件即可)。
3) 当触发收益/曝光回调时，调用对应 Wrapper：
   - AdMob：OnPaid ➜ `AdMobAdWrapper.Build*AdPaidEventHandler`

     示例（激励）：
     ```csharp
     // 加载完成后
     rewardedAd.OnAdPaid += AdMobAdWrapper.BuildRewardedAdPaidEventHandler(
         userCallback: null,
         responseInfo: rewardedAd.GetResponseInfo());
     ```
   - MAX：各广告类型收益 ➜ `Max*AdWrapper`

     示例（激励）：
     ```csharp
     MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += MaxRewardedAdWrapper.BuildAdRevenueHandler(OnAdRevenuePaid);
     ```
   - Gromore：显示回调 ➜ `GromoreAdWrapper.track*AdImpression`

     示例（激励 显示时）：
     ```csharp
     void IRewardAdInteractionListener.OnAdShow()
     {
         string placementId = config != null ? config.RewardedAdUnitId : "";
         MediationAdEcpmInfo ecpmInfo = rewardedAd != null ? rewardedAd.GetMediationManager().GetShowEcpm() : null;
         GromoreAdWrapper.TrackRewardedAdRevenue(placementId, ecpmInfo);
     }
     ```
   - IronSource：LevelPlay 曝光数据 ➜ `IronSourceWrapper`

     示例：
     ```csharp
     LevelPlay.OnImpressionDataReady += IronSourceWrapper.BuildImpressionDataHandler();
     ```
   - Taku/TopOn：回调信息 ➜ `TakuAdWrapper` / `TopOnAdWrapper`

     示例（Taku 激励展示）：
     ```csharp
     public void onRewardedVideoAdPlayStart(string placementId, ATCallbackInfo callbackInfo)
     {
         TakuAdWrapper.TrackRewardedAdRevenue(callbackInfo);
     }
     ```
4) 业务层只关注加载/展示，Wrapper 负责归一化与上报。

### 范围之外

- 生产级错误处理、合规（ATT/UMP）与完整变现策略不在此示例范围内。

### 说明

- appID/appKey 与广告单元 ID 为占位符，请替换为真实 ID。
## SolarEngine 冥想示例 – Unity

### 为什么有这个示例

- 在一个 Unity 工程中演示多种聚合平台（MAX、AdMob、Gromore、IronSource、Taku、TopOn）的端到端集成。
- 展示统一的 Wrapper 模式，用一个 API 将各平台的广告收益/曝光数据上报到 SolarEngine。
- 提供常见广告类型（激励、插屏、横幅、原生、开屏/App Open）的最小可运行示例，并标注 Unity 侧收益回调接入点。

### Wrapper 的作用

- 封装各平台 SDK 的 C# 事件/监听器，将其收益/曝光数据映射为 SolarEngine 的 `Analytics.trackAdImpression` 上报。
- 统一参数（广告类型枚举、广告位/单元、货币、eCPM）后再上报。
- 使管理器/控制脚本更干净：只负责加载/展示；Wrapper 负责事件与上报。

### 各平台要点

- AdMob：订阅 OnPaid 事件 → `AdMobAdWrapper` → `AdmobSolarEngineTracker`（含 `AdMobAdType`）。
- MAX：按广告类型的收益回调 → `Max*AdWrapper` → `MaxSolarEngineTracker`（含 `MaxAdType`）。
- Gromore：使用聚合 eCPM 信息 → `GromoreAdWrapper` → `GromoreSolarEngineTracker`（含 `GromoreAdType`）。
- IronSource：使用 `LevelPlayImpressionData` → `IronSourceWrapper` → `IronSourceSolarEngineTracker`。
- Taku/TopOn：使用回调信息对象 → `TakuAdWrapper` / `TopOnAdWrapper` → 各自 Tracker。

### 目录结构（概览）

- `Assets/Scripts/`
  - `Wrappers/` 各聚合平台 Wrapper 与 Tracker（C#）
  - 各平台管理脚本（`AdMob/`、`Max/`、`Gromore/`、`IronSource/`、`Taku/`、`TopOn/`）
  - `Resources/` 可序列化配置（如 Taku/Gromore 配置）
  - `UI/` 简单演示 UI

### 如何在你的项目中使用

1) 按官方指南导入所选聚合 SDK（Unity Package/插件）。
2) 参考本示例接入各 SDK 的 C# 事件/监听。
3) 当触发收益/曝光回调时，调用相应 Wrapper 将数据上报到 SolarEngine：
   - AdMob：OnPaid → `AdMobAdWrapper.*` → `AdmobSolarEngineTracker`
   - MAX：按广告类型收益回调 → `Max*AdWrapper` → `MaxSolarEngineTracker`
   - Gromore：聚合 eCPM → `GromoreAdWrapper` → `GromoreSolarEngineTracker`
   - IronSource：LevelPlay 曝光数据 → `IronSourceWrapper` → `IronSourceSolarEngineTracker`
   - Taku/TopOn：回调信息 → `TakuAdWrapper` / `TopOnAdWrapper` → 各自 Tracker
4) 业务层只关注加载/展示，Wrapper 负责归一化与上报。

### 范围之外

- 生产级错误处理、ATT/UMP/合规流程与完整变现策略不在此示例范围内。

### 说明

- appID/appKey 与广告单元 ID 仅为占位符。
- 发布前请替换为你的真实 ID。


