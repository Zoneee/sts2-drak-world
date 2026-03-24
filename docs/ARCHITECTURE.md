# 项目架构说明

这份文档描述当前仓库的目录结构、核心模块职责，以及实现弃牌体系时采用的关键约定。

## 1. 仓库结构

```text
STS2-Dark-World/
├── src/
│   ├── Main.cs
│   ├── STS2_Discard_Mod.csproj
│   ├── Cards/
│   ├── Patches/
│   ├── STS2DiscardMod/images/
│   └── localization/eng/cards.json
├── docs/
├── lib/
├── STS2_Discard_Mod.json
└── deploy.ps1
```

## 2. 核心模块

### `Main.cs`

入口负责：

- 建立 `STS2DiscardMod` 日志实例
- 反射发现全部 `DiscardModCard` 子类并输出日志
- 通过 `Harmony.PatchAll()` 启用全部补丁

### `Cards/DiscardModCard.cs`

这是当前卡组的统一基类，负责：

- 统一卡图路径
- 自动发现所有弃牌体系卡牌类型
- 统一的打出/弃牌触发日志
- 统一的辅助动作：抽牌、弃牌、随机敌人攻击、群体攻击、群体上状态、固定格挡
- 默认把 `AfterCardDiscarded()` 分发到 `OnSelfDiscarded()`

### `Cards/*.cs`

每张牌类负责：

- 费用、类型、稀有度、目标定义
- `OnPlay()`
- `OnUpgrade()`
- 需要时实现 `OnSelfDiscarded()`
- 少量数值字段（例如弃牌伤害、弃牌格挡、弃牌上毒层数）

### `Patches/*.cs`

- `LocalizationRuntimePatch`：卡牌文本兜底
- `CardLibraryVisibilityPatch`：保证自定义卡在卡库里可见
- `ModelDbDiagnosticsPatch`：检查是否成功注册到 `ModelDb` / `RegentCardPool`
- `DiscardDiagnosticsPatch`：记录弃牌命令链路

## 3. 注册方式

当前仓库使用声明式注册：

```csharp
[Pool(typeof(RegentCardPool))]
public class MyCard : DiscardModCard
{
}
```

这样能减少手工维护注册表时的漏改风险。

## 4. 构建与部署链路

构建成功后，会输出并尝试部署：

```text
{game}/mods/STS2_Discard_Mod/
├── STS2DiscardMod.dll
├── STS2DiscardMod.pck
├── 0Harmony.dll
└── STS2_Discard_Mod.json
```

如果没有设置 `GODOT_CLI_COMMAND`，代码仍可构建，但 `.pck` 不会导出，卡图资源不会挂载到 `res://`。

## 5. 当前实现边界

当前仓库已经完成：

- 10 张牌的注册与实现
- 初始化/建模/弃牌链路日志
- 本地化输入与运行时文本兜底
- 自动构建与自动部署

当前仍需继续验证的内容：

- 游戏内数值平衡
- “任何弃牌都触发”是否需要再细分规则
- 头像资源导出在不同开发机上的稳定性
