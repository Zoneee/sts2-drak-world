# 项目结构与架构说明

状态：有效
负责人：工程团队
最后评审：2026-03-24

## 概览
本仓库实现的是《Slay the Spire 2》实验性弃牌触发卡组模组。当前目标不是构建通用引擎，而是在可编译、可部署、可观察的前提下，围绕“打出收益 + 弃牌收益”这条核心机制持续扩展卡牌与调试能力。

## 仓库结构
```text
STS2-Dark-World/
├── src/
│   ├── Main.cs
│   ├── STS2_Discard_Mod.csproj
│   ├── Cards/
│   ├── Patches/
│   ├── Data/
│   ├── STS2DiscardMod/
│   ├── localization/
│   └── .godot/
├── scripts/
├── docs/
├── lib/
├── tools/
└── STS2_Discard_Mod.json
```

## 核心模块
### `src/Main.cs`
入口只保留最小职责：
- 读取 `DiscardModCard.AllCardTypes`
- 输出模组初始化日志
- 执行 `Harmony.PatchAll()` 注册补丁

当前的 `ModId` 为 `STS2DiscardMod`，必须与程序集名称和 manifest `id` 保持一致。

### `src/Cards/`
卡牌层以 `DiscardModCard` 为统一基类，集中处理：
- 所有弃牌体系卡牌的反射发现
- 统一卡图路径
- 统一的打出与弃牌触发日志
- 抽牌、弃牌、随机伤害、群体伤害、群体上状态、固定格挡等辅助动作
- 默认把 `AfterCardDiscarded()` 分发到 `OnSelfDiscarded()`

每张具体卡牌类只负责：
- 数值与元数据
- `OnPlay()`
- `OnUpgrade()`
- 需要时实现 `OnSelfDiscarded()`

### `src/Patches/`
补丁层负责把 BaseLib、游戏运行时与本模组的诊断能力接起来：
- `LocalizationRuntimePatch`：运行时文本兜底
- `CardLibraryVisibilityPatch`：保证卡牌在卡库中可见
- `ModelDbDiagnosticsPatch`：在 `ModelDb.Init()` 后验证进库与进池情况
- `DiscardDiagnosticsPatch`：记录弃牌命令链路
- `DebugCardPoolSettings`：管理调试构建下的卡池过滤开关
- `DebugStartingDeckPatch`：调试构建下替换初始牌组
- `DebugMerchantInventoryPatch`：调试构建下替换商店 colorless 槽位

### `src/Data/` 与 `src/localization/`
- `Data/cards.catalog.json` 是本地化生成输入与卡牌目录数据源之一。
- `localization/eng/`、`localization/zhs/` 由构建前的生成步骤维护，供 analyzer 与运行时文本使用。

### `src/STS2DiscardMod/` 与 `src/.godot/`
- `STS2DiscardMod/` 保存 Godot 侧资源。
- `.godot/imported/` 保存导入缓存，调试松散资源时需要一起部署。

## 注册与装配约定
### 声明式注册
卡牌通过 `[Pool(typeof(RegentCardPool))]` 声明进入 `RegentCardPool`，避免在入口文件中手工维护卡牌列表。

### 模组装配一致性
以下标识必须一致，否则模组可能无法加载：
- `DiscardModMain.ModId`
- `STS2_Discard_Mod.csproj` 中的 `AssemblyName`
- `STS2_Discard_Mod.json` 中的 `id`

### 资源装配
若 manifest `has_pck=true`，则 `STS2DiscardMod.pck` 必须成功导出并部署；否则 `res://STS2DiscardMod/...` 下的资源会解析失败。

## 构建与部署链路
### 构建前
- `GenerateLocalizationFiles` 先根据 `Data/cards.catalog.json` 生成本地化文件。

### 构建后
- 写出 `BUILD_FLAVOR.txt`
- 若存在 `GODOT_CLI_COMMAND`，导出 `STS2DiscardMod.pck`
- 若探测到 `ModsPath`，自动复制 DLL、manifest、Harmony 依赖、`.pck`、Godot 资源和导入缓存到 live 模组目录

## 当前不变量
- 当前共有 10 张自定义卡牌，且全部属于 `RegentCardPool`。
- `BaseLib` 作为独立依赖必须已安装到游戏目录。
- `cards.json` 不能直接复制进 live 模组目录。
- 调试构建与发布构建的行为差异以 `BUILD_FLAVOR.txt` 为准。

## 主要风险
- 旧文档与实际代码可能在路径、任务名、卡牌数量上存在漂移，必须以当前仓库文件为准。
- Godot CLI 缺失时，构建成功不代表卡图资源可用。
- 若在错误时机向 `ModelDb` 注入自定义模型，可能引发重复注册问题，当前做法是只在 `ModelDb.Init()` 后做 presence 诊断。