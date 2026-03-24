# 本地化与卡图修复计划

## 状态
执行中

## 背景
2026-03-24 晚间多次游戏日志显示，本模组同时存在本地化显示异常、卡牌描述缺失或混搭、以及卡图资源加载失败。日志还暴露出两个前置阻断项：运行时会把 `localization/**/*.json` 误当作 manifest 递归扫描，以及 `DiscardDiagnosticsPatch` 对 `CardCmd.DiscardAndDraw` 的 Harmony 参数名不匹配，导致初始化阶段抛错。

## 问题摘要
- live 模组目录中的 `localization/eng/cards.json` 与 `localization/zhs/cards.json` 被游戏当作 manifest 扫描，触发缺失 `id` 的加载错误。
- `DiscardDiagnosticsPatch` 的 `DiscardAndDraw` prefix 使用了错误的参数名 `cards`，Harmony 无法绑定到真实参数 `cardsToDiscard`。
- 自定义卡牌名称与描述在日志中出现缺失，导致中英文混搭和回退显示。
- 自定义卡图在日志中出现 `No loader found for resource: res://STS2DiscardMod/images/card_portraits/...png`。
- 英文 `ToxinRecord` 描述会触发富文本解析错误，日志中出现 `Found end tag center, expected Poison`。

## 范围
- 修复初始化阻断项，使模组能稳定完成 `PatchAll()` 和 manifest 扫描。
- 修复本地化运行时查找链路中与本任务直接相关的问题。
- 修复或验证卡图构建产物与部署链路中的运行时问题。
- 同步更新与本任务直接相关的运行手册。

## 非范围
- 不调整卡牌数值平衡。
- 不更改“任何弃牌都会触发效果”的当前设计规则。
- 不处理与本模组无关的基础游戏缺字日志。
- 不把本次修复扩展成一次通用文档或模板清理。

## 受影响区域
- `src/Patches/DiscardDiagnosticsPatch.cs`
- `src/Patches/LocalizationRuntimePatch.cs`
- `src/Utils/CardCatalog.cs`
- `src/Data/cards.catalog.json`
- `scripts/common.sh`
- `src/STS2_Discard_Mod.csproj`
- `docs/runbooks/deployment.md`

## 约束
- 已采用 Skills：
  - `skills/plan-before-code.md`
  - `skills/repo-as-source-of-truth.md`
  - `skills/evidence-driven-delivery.md`
  - `skills/small-safe-prs.md`
  - `skills/observability-first-debugging.md`
  - `skills/boundary-validation.md`
  - `skills/fix-the-system-not-just-the-ticket.md`
  - `skills/docs-update-required.md`
  - `skills/self-review-loop.md`
  - `skills/failure-recovery.md`
- 不回退用户刚同步进来的无关模板/文档改动。
- 若 live 游戏目录文件被占用，优先保留构建证据并记录部署阻塞，不做破坏性绕过。

## 验收标准
- 构建成功，且 `DiscardDiagnosticsPatch` 不再因参数名不匹配而破坏初始化。
- 未来部署不再把 `localization/**/*.json` 复制到 live 模组目录，并会清理历史残留目录。
- 英文 `ToxinRecord` 描述不再走到日志中已知的富文本解析失败路径。
- 构建产物中包含 `STS2DiscardMod.pck`，并保留 `.godot/imported` 资源供部署使用。

## 验证计划
- 运行 `Build + Deploy: Debug` 或等效构建，确认项目可编译。
- 检查 `src/bin/Debug/net9.0/` 是否生成 `STS2DiscardMod.dll`、`STS2DiscardMod.pck`、`0Harmony.dll`、`BUILD_FLAVOR.txt`。
- 检查 live 模组目录是否仍残留 `localization/`；若文件被占用无法部署，记录为环境阻塞并提示关闭游戏后重试。
- 重新查看游戏日志，重点关注：manifest 扫描错误、Harmony 参数绑定错误、自定义卡牌文本缺失警告、卡图加载错误。

## 验证结果
- 已完成代码修复：`DiscardDiagnosticsPatch` 参数名已改为 `cardsToDiscard`。
- 已完成代码修复：脚本与 MSBuild 自动部署不再复制 `localization/**/*.json`，且部署前会清理 live 目录中遗留的 `localization/`。
- 已完成代码修复：`ToxinRecord` 英文描述改为 `Apply 4 poison stacks. Discard: apply 2 poison stacks to all enemies.`。
- 已验证静态诊断：相关文件编辑后无编辑器错误。
- 已验证构建：`Build + Deploy: Debug` 在构建阶段成功产出 `STS2DiscardMod.dll` 与 `STS2DiscardMod.pck`。
- 已验证临时目录部署：对 `/tmp/sts2-discard-verify` 运行同一套 `deploy_runtime Debug` 后，目录中包含 `STS2DiscardMod.pck`、`.godot/imported`、`STS2DiscardMod/images/...`，且不包含 `localization/`。
- 当前阻塞：部署到 `/mnt/d/G_games/steam/steamapps/common/Slay the Spire 2/mods/STS2_Discard_Mod` 时，`STS2DiscardMod.dll` 被占用，复制阶段报 `Input/output error`，因此尚未获得新的运行时日志证据。

## 风险
- 在获得新的游戏日志前，运行时本地化混搭与卡图问题只能部分依赖构建/目录证据推断，不能宣称完全关闭。
- live 目录中旧版本遗留文件可能继续污染验证，直到下一次成功部署。

## 决策日志
- 2026-03-24：将“误部署本地化 JSON”和“Harmony 参数名错误”视为本轮显示/卡图问题的前置阻断项，一并修复。
- 2026-03-24：对英文描述采用最小安全改写，而不是先调整运行时富文本系统。
- 2026-03-24：发现 live 目录旧 `localization/` 残留会让问题持续复现，因此把部署前清理纳入系统性修复。

## 进展日志
- 2026-03-24：完成日志到代码路径的映射，确定三条问题链：初始化阻断、本地化显示、卡图资源加载。
- 2026-03-24：完成 `DiscardDiagnosticsPatch`、`scripts/common.sh`、`STS2_Discard_Mod.csproj` 和 `cards.catalog.json` 的第一轮修复。
- 2026-03-24：使用临时目录验证新的部署函数，确认 `.godot/imported` 与卡图资源会被正确复制，且 `localization/` 不会再进入运行时目录。
- 2026-03-24：构建成功，但 live 部署因目标 DLL 被占用而中断，等待关闭游戏后重试以收集最终运行时证据。

## 后续事项
- 关闭游戏后重新执行一次 `Build + Deploy: Debug` 或 `Deploy: Debug Runtime`，收集新的日志证据。
- 若新日志仍有中英混搭或卡图加载失败，再针对 `LocalizationRuntimePatch` 的语言识别和 `.pck`/`.godot/imported` 实际挂载状态做第二轮最小修复。