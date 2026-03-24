# 项目总览：STS2 弃牌触发 Mod

为《Slay the Spire 2》添加一套围绕“弃牌后触发效果”设计的实验性卡组。

## 当前状态

当前仓库已经具备一套可编译、可注册、可打出、可记录日志的弃牌体系：

- 已实现 10 张牌，全部加入 `RegentCardPool`
- 已实现 `OnPlay()` 效果与 `AfterCardDiscarded()` 弃牌触发
- 已补全运行时诊断日志，覆盖初始化、建模、弃牌命令、单卡打出与单卡触发
- 已补齐本地化键与卡牌注册表文档

当前仍然建议继续做游戏内数值验证。另一个已知约束是：现在采用“任何弃牌都会触发这张牌的弃牌效果”的规则，还没有再细分“主动弃牌 / 回合结束弃牌 / 其他来源弃牌”。

## 当前卡组（10 张）

| 卡牌 | 类名 | 类型 | 稀有度 | 费用 | 核心效果 |
| --- | --- | --- | --- | --- | --- |
| 迅影斩 | `SwiftCut` | Attack | Common | 0 | 打出单体伤害；弃牌时随机补刀 |
| 暗焰残页 | `DarkFlameFragment` | Skill | Common | 1 | 抽 1 弃 1；弃牌时全体伤害 |
| 毒素记录 | `ToxinRecord` | Skill | Uncommon | 1 | 打出上中毒；弃牌时群体上毒 |
| 破碎回响 | `ShatteredEcho` | Skill | Rare | 2 | 抽 2 弃 1；弃牌时再抽牌 |
| 灰烬庇护 | `AshenAegis` | Skill | Common | 1 | 打出得格挡；弃牌时也得格挡 |
| 崩坏手稿 | `CripplingManuscript` | Skill | Uncommon | 1 | 打出上虚弱/易伤；弃牌时群体虚弱 |
| 余烬连射 | `EmberVolley` | Attack | Common | 1 | 打出伤害；弃牌时随机伤害并抽牌 |
| 回收思路 | `RecallSurge` | Skill | Uncommon | 1 | 抽 2 弃 1；弃牌时得格挡 |
| 褪色公式 | `FadingFormula` | Skill | Common | 0 | 打出抽牌；回合结束自动弃掉自身并触发 |
| 终稿余烬 | `FinalDraft` | Attack | Rare | 2 | 打出单体伤害；弃牌时全体伤害并抽牌 |

详细注册表见 [docs/MOD_REGISTRY.md](docs/MOD_REGISTRY.md)。

## 快速开始

### 1. 准备依赖

- 安装 `.NET SDK 9.0+`
- 准备 `sts2.dll`

最直接的方式是把游戏目录里的 `sts2.dll` 复制到当前 worktree 的 `lib/sts2.dll`。

如果你在 git worktree 中开发，也可以直接在构建命令里覆盖路径：

```bash
dotnet build src/STS2_Discard_Mod.csproj \
  -p:Sts2DataDir=/absolute/path/to/lib
```

### 2. 构建项目

```bash
dotnet build src/STS2_Discard_Mod.csproj --configuration Debug
```

或：

```bash
dotnet build src/STS2_Discard_Mod.csproj \
  --configuration Debug \
  -p:Sts2DataDir=/absolute/path/to/lib
```

### 3. 导出卡图资源（可选但推荐）

如果要让 `res://STS2DiscardMod/...` 卡图真正显示，需要提供 Godot CLI：

```bash
export GODOT_CLI_COMMAND='/path/to/godot4'
dotnet build src/STS2_Discard_Mod.csproj --configuration Release
```

### 4. 自动部署

构建成功后，项目会自动部署到：

```text
{游戏目录}/mods/STS2_Discard_Mod/
├── STS2DiscardMod.dll
├── STS2DiscardMod.pck
├── 0Harmony.dll
└── STS2_Discard_Mod.json
```

## 关键注意事项

- `DiscardModMain.ModId`、manifest 的 `id` 和输出 DLL 名必须都等于 `STS2DiscardMod`
- 不要把 `src/localization/eng/cards.json` 复制到 live 模组目录
- `BaseLib` 是独立依赖 mod，当前仓库不会代替你把它复制到游戏目录
- 没有 `GODOT_CLI_COMMAND` 时，代码仍可构建，但头像资源不会被打进 `.pck`

## 这次实现验证出来的成功经验

- 用 `[Pool(typeof(RegentCardPool))]` 做声明式注册，比手工维护注册表更稳
- 把弃牌共用逻辑集中到 `DiscardModCard`，能显著减少重复代码和漏日志风险
- `CardCmd.Discard` 级别的 Harmony 日志非常适合定位“为什么没触发”
- `ModelDb.Init` 后再做卡牌 presence 诊断，能快速区分“没注册”与“资源没挂载”
- 在 worktree 里构建时，直接通过 `-p:Sts2DataDir=...` 指向已有 `sts2.dll`，比拷贝来拷贝去更稳

## 文档索引

- [docs/QUICK_START.md](docs/QUICK_START.md)：快速上手
- [docs/DEV_GUIDE.md](docs/DEV_GUIDE.md)：开发指南
- [docs/DEBUGGING.md](docs/DEBUGGING.md)：调试与故障排查
- [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)：架构说明
- [docs/DESIGN.md](docs/DESIGN.md)：卡组设计与后续方向
- [docs/MOD_REGISTRY.md](docs/MOD_REGISTRY.md)：当前 10 张牌的真实注册表
- [docs/CHANGELOG.md](docs/CHANGELOG.md)：版本历史
