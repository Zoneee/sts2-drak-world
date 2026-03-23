# 开发指南

这份文档是当前仓库的唯一完整开发说明，覆盖环境准备、构建、部署、添加卡牌和常见约束。

## 1. 环境准备

### 必要条件

- `.NET SDK 9.0+`
- 已安装的《Slay the Spire 2》
- `lib/sts2.dll`

### 获取 `sts2.dll`

`sts2.dll` 是游戏专有文件，不提交到仓库，需要本地手动复制。

Windows：

```powershell
$src = "D:\G_games\steam\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.dll"
$dst = "\\wsl$\Ubuntu\home\alphonse\projects\STS2-Dark-World\lib\sts2.dll"
Copy-Item $src $dst
```

Linux：

```bash
cp ~/.local/share/Steam/steamapps/common/Slay\ the\ Spire\ 2/data_sts2_linuxbsd_x86_64/sts2.dll \
  ~/projects/STS2-Dark-World/lib/
```

### 首次构建

```bash
git clone git@github.com:Zoneee/sts2-drak-world.git
cd sts2-drak-world
dotnet build src/
```

## 2. 项目结构

```text
src/
├── Main.cs                     # 模组入口与卡牌注册
├── Cards/                      # 卡牌类
├── localization/eng/cards.json # 本地化文本，供分析器读取
├── project.godot               # Godot 工程配置
└── STS2_Discard_Mod.csproj     # 项目文件

根目录/
├── STS2_Discard_Mod.json       # 模组 manifest
├── lib/sts2.dll                # 本地游戏 DLL
└── docs/                       # 文档
```

## 3. 构建与部署

### 本地构建

```bash
dotnet build src/ --configuration Debug
dotnet build src/ --configuration Release
dotnet clean src/ && dotnet build src/ --configuration Release
```

Release 输出路径：

```text
src/bin/Release/net9.0/STS2DiscardMod.dll
src/bin/Release/net9.0/STS2DiscardMod.pck
```

### 自动部署

项目内置 `CopyToModsFolderOnBuild`，构建成功后会自动部署到：

```text
{游戏目录}/mods/STS2_Discard_Mod/
├── STS2DiscardMod.dll
├── STS2DiscardMod.pck
├── 0Harmony.dll
└── STS2_Discard_Mod.json
```

这也是当前仓库唯一推荐的 live 目录结构。

如果你要让卡图这类 `res://STS2DiscardMod/...` 资源在游戏内可见，构建前必须提供 Godot CLI：

```bash
export GODOT_CLI_COMMAND='/path/to/godot4'
dotnet build src/ --configuration Release
```

在 WSL 下也可以把它指向可直接执行的 Windows Godot 编辑器，例如 `/mnt/c/.../Godot_v4.5.1-stable_mono_win64.exe`。

### 手动部署

只有在自动部署路径解析失败时，才需要手动复制下面两个文件：

```text
src/bin/Release/net9.0/STS2DiscardMod.dll
src/bin/Release/net9.0/STS2DiscardMod.pck
src/bin/Release/net9.0/0Harmony.dll
STS2_Discard_Mod.json
```

不要把 `src/localization/eng/cards.json` 直接复制到 live 模组目录。当前加载器会把额外 `.json` 当成 manifest 递归读取，导致加载错误。

Windows 特定路径示例见 [WINDOWS_DEPLOYMENT.md](WINDOWS_DEPLOYMENT.md)。

## 4. VS Code 工作流

日常开发最常用的命令只有一个：

```text
Ctrl+Shift+B
```

它会运行默认的 `Build: Release` 任务。

如果要调试，使用：

- `Build: Debug`
- `Build + Deploy: Debug`

完整任务说明见 [VSCODE_WORKFLOW.md](VSCODE_WORKFLOW.md)。

## 5. 关键 STS2 API

### `CardModel`

所有卡牌继承自 `MegaCrit.Sts2.Core.Models.CardModel`。

```csharp
public class MyCard : CardModel
{
    public MyCard()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true) { }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        DiscardModMain.Logger.Info("MyCard played");
        await Task.CompletedTask;
    }

    public override void OnUpgrade()
    {
    }
}
```

### `ModHelper.AddModelToPool`

在 `Main.cs` 中把卡牌注册到卡池：

```csharp
ModHelper.AddModelToPool(typeof(RegentCardPool), typeof(MyCard));
```

## 6. 添加新卡牌

### 步骤

1. 在 `src/Cards/` 新建卡牌类
2. 在 `src/localization/eng/cards.json` 添加本地化键
3. 在 `Main.cs` 中调用 `ModHelper.AddModelToPool(...)`
4. 执行 `dotnet build src/`

### 卡牌模板

```csharp
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System.Threading.Tasks;

namespace DiscardMod.Cards;

public class YourCard : CardModel
{
    public YourCard()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true) { }

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        DiscardModMain.Logger.Info("YourCard played");
        await Task.CompletedTask;
    }

    public override void OnUpgrade()
    {
    }
}
```

### 本地化格式

当前仓库使用扁平键：

```json
{
  "YOUR_CARD.title": "卡牌名",
  "YOUR_CARD.description": "效果描述。"
}
```

键名规则：类名转大写下划线，例如 `YourCard` 对应 `YOUR_CARD`。

## 7. manifest 约定

`STS2_Discard_Mod.json` 至少应保持这几个字段正确：

```json
{
  "id": "STS2DiscardMod",
  "has_pck": true,
  "has_dll": true,
  "affects_gameplay": true
}
```

注意：

- `id` 必须与代码中的 `DiscardModMain.ModId` 一致
- 当前 DLL 输出名必须是 `STS2DiscardMod.dll`

## 8. 调试与排障

常见问题优先看 [DEBUGGING.md](DEBUGGING.md)。

最常见的两个坑：

1. DLL 名和 `id` 不一致，导致模组声明加载程序集但找不到 DLL
2. live 模组目录里混入 `cards.json` 等额外 JSON，被加载器误判为 manifest
