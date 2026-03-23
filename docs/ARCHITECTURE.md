# 项目架构说明

这份文档描述仓库的目录结构、核心模块职责，以及当前实现依赖的关键约定。

## 1. 仓库结构

```text
STS2-Dark-World/
├── src/
│   ├── Main.cs
│   ├── STS2_Discard_Mod.csproj
│   ├── project.godot
│   ├── Cards/
│   └── localization/eng/cards.json
├── docs/
├── lib/sts2.dll
├── STS2_Discard_Mod.json
└── .github/workflows/build.yml
```

### 关键目录职责

- `src/`：全部源码与本地化输入
- `src/Cards/`：每张卡一个类
- `src/localization/eng/cards.json`：供分析器读取的本地化数据
- `lib/sts2.dll`：本地游戏依赖，不提交到 git
- `STS2_Discard_Mod.json`：模组 manifest
- `docs/`：项目文档

## 2. 核心模块

### `Main.cs`

模组入口负责三件事：

1. 定义 `ModId`
2. 初始化日志
3. 把卡牌注册到 `RegentCardPool`

核心结构如下：

```csharp
[ModInitializer(nameof(Initialize))]
public static class DiscardModMain
{
    public const string ModId = "STS2DiscardMod";
    public static Logger Logger { get; } = new(ModId, LogType.Generic);

    public static void Initialize()
    {
        Logger.Info("loading...");
        RegisterCards();
        Logger.Info("loaded!");
    }
}
```

### `Cards/*.cs`

每张卡都是 `CardModel` 的子类。

当前仓库里卡牌类只负责：

- 定义费用、类型、稀有度、目标
- 提供 `OnPlay()` 占位实现
- 预留 `OnUpgrade()`

### `cards.json`

当前仓库使用扁平键本地化格式：

```json
{
  "SWIFT_CUT.title": "迅影斩",
  "SWIFT_CUT.description": "描述"
}
```

这份文件是构建输入，不是运行时必须部署的 live 文件。

## 3. 构建与部署链路

### 构建产物

```text
src/bin/{Configuration}/net9.0/STS2DiscardMod.dll
```

### 自动部署目标

构建成功后，MSBuild 会把下面两个文件部署到游戏目录：

```text
{game}/mods/STS2_Discard_Mod/
├── STS2DiscardMod.dll
└── STS2_Discard_Mod.json
```

当前约定里，live 模组目录不要出现额外的 `cards.json`。

### 命名约束

以下三者必须保持一致：

- `DiscardModMain.ModId = "STS2DiscardMod"`
- 构建输出：`STS2DiscardMod.dll`
- `STS2_Discard_Mod.json` 中的 `id = "STS2DiscardMod"`

只要其中一项不一致，就会出现“声明加载 DLL 但找不到程序集”的问题。

## 4. 关键依赖

- `MegaCrit.Sts2.Core.Modding`：`ModInitializer`、`ModHelper`
- `MegaCrit.Sts2.Core.Logging`：`Logger`
- `MegaCrit.Sts2.Core.Models.CardPools`：`RegentCardPool`
- `MegaCrit.Sts2.Core.Models`：`CardModel`

## 5. 当前实现边界

当前仓库已经完成：

- 模组初始化
- 4 张卡注册
- 本地化键输入
- 自动构建与自动部署

当前仍未完成：

- 弃牌事件监听
- 弃牌触发效果
- 大部分 `OnPlay()` 真实逻辑

## 6. 相关文档

- 完整开发说明：见 [DEV_GUIDE.md](DEV_GUIDE.md)
- 调试与故障排查：见 [DEBUGGING.md](DEBUGGING.md)
- 设计目标：见 [DESIGN.md](DESIGN.md)
