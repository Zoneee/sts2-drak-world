# 数据流

状态：有效
负责人：工程团队
最后评审：2026-03-28

## 用途
描述系统中关键数据的流转路径，包括输入来源、处理节点、输出去向。

## 关键流程

### 本地化文本流
```
cards.catalog.json
    → [构建前] CardLocalizationGenerator
        → localization/eng/cards.json（普通卡牌：无 smartDescription 字段）
        → localization/eng/powers.json（能力牌：有 smartDescription 字段）
    → [运行时] BaseLib analyzer 读取 localization/*.json，注入 LocTable
    → [运行时] LocTable.GetRawText("cards" | "powers", key) 被调用
        → LocalizationRuntimePatch [HarmonyBefore("BaseLib"), Priority.First] 拦截
            → CardCatalog.TryNormalizeCardLocalizationKey() 查找 DISCARDMOD-* 条目
            → 若命中，直接返回本模组文本 (__result = text, 跳过 BaseLib)
        → 若未拦截，BaseLib 默认逻辑处理
```

**关键约束**：`LocalizationRuntimePatch` 必须同时处理 `cards` 和 `powers` 两张表，否则能力牌的名称（动画文本）和描述（tooltip）会退化为原始 key。

### 卡牌图标流（普通卡牌）
```
src/STS2DiscardMod/images/cards/{name}.png
    → [构建后] Godot CLI 导出 STS2DiscardMod.pck
    → [运行时] res://STS2DiscardMod/images/cards/{name}.png 解析
```

### 能力图标流（Power 卡牌）
```
src/STS2DiscardMod/images/powers/{name}_64.png   (64×64)
src/STS2DiscardMod/images/powers/{name}.png      (256×256)
    → [构建后] Godot CLI 导出 STS2DiscardMod.pck
    → [运行时] PowerModel.PackedIconPath getter 被调用
        → BaseLib ICustomPower Harmony patch 拦截
            → 调用 CustomPackedIconPath（Power 类重写）
            → 若非 null，返回自定义路径
        → AtlasResourceLoader 加载 PNG
```

**关键约束**：若 Power 类未重写 `CustomPackedIconPath`，BaseLib 回退到 atlas 路径（`atlases/power_atlas.sprites/discardmod-{name}.tres`），该路径不存在，导致 atlas missing warning 且图标不显示。

### 弃牌触发流
```
游戏执行 CardCmd.Discard / CardCmd.DiscardAndDraw
    → DiscardDiagnosticsPatch 记录日志
    → DiscardTriggerRuntime.MarkPowerDiscardEvent(card) 标记待处理
    → 触发 AfterCardDiscarded(card) 回调（各 Power 类）
        → Power.ConsumePowerDiscardEvent(card) 消费标记
            → 若为卡牌效果弃牌：执行 Power 效果
            → 若为其他弃牌（回合结束等）：跳过
```

### 构建与部署流
```
dotnet build
    → [pre-build] GenerateLocalizationFiles（生成 cards.json / powers.json）
    → [compile] 生成 STS2DiscardMod.dll
    → [post-build] 写出 BUILD_FLAVOR.txt
    → [post-build] 若 GODOT_CLI_COMMAND 已设置：导出 STS2DiscardMod.pck
    → [post-build] 若 ModsPath 已配置：部署 DLL + manifest + pck + BaseLib 依赖
```

## 数据边界标注
- **边界进入点**：`cards.catalog.json` 是唯一的本地化与目录数据输入，任何新卡牌必须在此声明。
- **边界退出点**：`localization/` 目录下的文件由生成器维护，不允许手工修改。
- **敏感边界**：Godot 资源路径（`res://`）必须与 PCK 内路径完全一致，否则资源静默加载失败。
