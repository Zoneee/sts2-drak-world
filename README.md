# STS2 弃牌触发 Mod

为《Slay the Spire 2》添加一套围绕“弃牌后触发效果”设计的实验性卡组，并用 Agent 优先的文档结构管理构建、调试、设计和交付流程。

## 当前状态
当前仓库已经具备一套可编译、可部署、可打出、可记录日志的弃牌体系：
- 已实现 10 张牌，全部进入 `RegentCardPool`
- 已实现 `OnPlay()` 与弃牌触发逻辑
- 已补全初始化、建模、弃牌命令、单卡触发等运行时诊断日志
- 已建立新的 `docs/` 结构，收敛快速上手、开发、调试、架构与设计文档

当前仍建议继续做游戏内数值验证。当前规则边界已经收紧为：默认只有因卡牌效果导致的弃牌才会触发弃牌效果，`FadingFormula` 是牌面明确写出的显式例外。
另外，带有打出时弃牌要求的卡会在打出前检查当前手中是否已有足够其他手牌可弃；待生效的额外弃牌次数也会计入该检查。

> **玩家指南 / Player Guide** → [docs/mod-intro.md](docs/mod-intro.md)（设计意图 · 故事背景 · 卡牌一览 · 安装方式）

## 当前卡组
| 卡牌     | 类名                  | 类型   | 稀有度   | 费用 | 核心效果                                 |
| -------- | --------------------- | ------ | -------- | ---- | ---------------------------------------- |
| 迅影斩   | `SwiftCut`            | Attack | Common   | 1    | 打出单体伤害；弃牌时随机高伤补刀         |
| 暗焰残页 | `DarkFlameFragment`   | Skill  | Uncommon | 2    | 抽 1 弃 2；弃牌时继续推进弃牌链          |
| 毒素记录 | `ToxinRecord`         | Skill  | Uncommon | 1    | 打出上中毒；弃牌时群体上毒               |
| 破碎回响 | `ShatteredEcho`       | Skill  | Rare     | 3    | 抽 2 弃 3；弃牌时抽 1 弃 1               |
| 灰烬庇护 | `AshenAegis`          | Skill  | Common   | 1    | 打出得格挡；弃牌时也得格挡               |
| 崩坏手稿 | `CripplingManuscript` | Skill  | Uncommon | 1    | 打出上虚弱/易伤；弃牌时群体双减益        |
| 余烬连射 | `EmberVolley`         | Attack | Uncommon | 2    | 打出伤害；弃牌时随机高伤                 |
| 回收思路 | `RecallSurge`         | Skill  | Common   | 1    | 抽 1 弃 1；弃牌时得较高格挡              |
| 褪色公式 | `FadingFormula`       | Skill  | Rare     | 3    | 打出伤害并得格挡；弃牌时按奇偶次数结算   |
| 终稿余烬 | `FinalDraft`          | Attack | Rare     | 3    | 打出单体高伤；弃牌时全体高伤并吃弃牌阈值 |

## 快速开始
### 1. 准备依赖
- 安装 `.NET SDK 9.0+`
- 准备 `sts2.dll`

最直接的方式是把游戏目录里的 `sts2.dll` 复制到当前仓库的 `lib/sts2.dll`。也可以在构建时通过 `-p:Sts2DataDir=...` 指向现有目录。

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

### 3. 导出卡图资源
如果要让 `res://STS2DiscardMod/...` 卡图真正显示，需要提供 Godot CLI：

```bash
export GODOT_CLI_COMMAND='/path/to/godot4'
dotnet build src/STS2_Discard_Mod.csproj --configuration Release
```

### 4. 自动部署
构建成功后，项目会尝试把运行时文件部署到：

```text
{游戏目录}/mods/STS2_Discard_Mod/
├── STS2DiscardMod.dll
├── STS2DiscardMod.pck
├── 0Harmony.dll
├── BUILD_FLAVOR.txt
└── STS2_Discard_Mod.json
```

## 关键注意事项
- `DiscardModMain.ModId`、manifest 的 `id` 与 `AssemblyName` 必须都等于 `STS2DiscardMod`
- 不要把 `src/localization/eng/cards.json` 复制到 live 模组目录
- `BaseLib` 是独立依赖 mod，当前仓库不会替你安装到游戏目录
- 没有 `GODOT_CLI_COMMAND` 时，代码仍可构建，但头像资源不会被打进 `.pck`

## 文档入口
- `AGENTS.md`：Agent 执行入口与规则
- `docs/index.md`：文档总索引
- `docs/runbooks/quick-start.md`：3 分钟快速上手
- `docs/runbooks/local-dev.md`：本地开发说明
- `docs/runbooks/debug-failures.md`：调试与故障排查
- `docs/architecture/project-structure.md`：项目结构与构建链路
- `docs/design-docs/discard-system.md`：弃牌体系设计
- `docs/product-specs/mod-registry.md`：卡牌注册表
- `docs/product-specs/card-details.md`：卡牌详细设计
