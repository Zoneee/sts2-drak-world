<!--
  格式规范：
    ## vX.Y.Z — YYYY-MM-DD
    ### 新增 / 调整与平衡 / 修复 / 已知问题（可选）
    - **卡牌中文名**（英文 ID）：说明

  运行 python3 scripts/bootstrap-changelog.py 可重新生成草稿
  （会覆盖现有文件，请注意备份手动编辑的内容）
-->

# CHANGELOG

## v1.2.1 — 2026-03-28

<!-- 来源：弃牌体系 V1.2 后：能力牌本地化、图标与升级描述修复 -->
- Bug #1 — 升级预览不更新描述
- Bug #2 — 打出动画文字显示 ID
- Bug #3 — 玩家状态栏缺少能力图标
- Bug #4 — 状态栏 tooltip 描述错误

## v1.2.0 — 2026-03-27

<!-- 来源：弃牌体系 V1.2：能力牌新增与卡牌重设计 -->
- 完成 5 张牌重设计实现
- 完成 3 张 Power 卡与 3 个 PowerModel 实现
- 通过反射发现并修复所有 API 调用问题（PowerModel 抽象成员、CreatureCmd、PlayerCmd 签名等）
- 完成 CardLocalizationGenerator 扩展，生成 powers.json
- 完成 cards.catalog.json 更新（6 条新条目）
- `dotnet build` 验证通过：**0 Error(s)**，1 可接受 nullable warning
- 同步更新 card-details.md、mod-registry.md、discard-system.md

## v1.1.0 — 2026-03-26

<!-- 来源：弃牌体系 V1.1 实施 -->
- 完成设计方案收敛，进入实现阶段
- 完成弃牌触发边界运行时改造，加入弃牌来源与本回合弃牌次数跟踪
- 完成 10 张牌的费用、稀有度、打出/弃牌效果与升级方向同步
- 完成本地化、产品规格、设计文档、注册表与 README 同步更新
- `Build: Debug` 构建成功，并重新导出 Debug `.pck`
- 为 `RecallSurge`、`DarkFlameFragment`、`ShatteredEcho` 增加打出前可弃牌数量校验，并将待生效的额外弃牌次数纳入检查
- 再次执行 `Build: Debug`，确认新约束编译与导包均通过

## v1.0.0 — 2026-03-24

<!-- 来源：文档模板迁移计划 -->
- 完成旧文档与新模板目录的映射分析，准备落第一轮核心文档。
- 完成核心文档迁移、索引更新和顶层入口收口，并修正 `SETUP_COMPLETE.md` 的旧路径引用。

<!-- 来源：本地化与卡图修复计划 -->
- 完成日志到代码路径的映射，确定三条问题链：初始化阻断、本地化显示、卡图资源加载。
- 完成 `DiscardDiagnosticsPatch`、`scripts/common.sh`、`STS2_Discard_Mod.csproj` 和 `cards.catalog.json` 的第一轮修复。
- 使用临时目录验证新的部署函数，确认 `.godot/imported` 与卡图资源会被正确复制，且 `localization/` 不会再进入运行时目录。
- 构建成功，但 live 部署因目标 DLL 被占用而中断，等待关闭游戏后重试以收集最终运行时证据。

<!-- 来源：图鉴 Debug 卡池过滤回归修复计划 -->
- 对照最新日志确认图鉴回归链路。
- 移除 `ModelDbDiagnosticsPatch` 中对 `RegentCardPool` 的全局缓存篡改。
- 将 Debug 默认行为收敛为起始牌组替换与商店替换，并重新构建、部署。
- 核对 `godot2026-03-25T20.08.50.log`、当前源码和 live `BUILD_FLAVOR.txt` 后，确认该日志反映的是旧运行行为，不是本轮部署后的有效验证结果。
- 核对 `godot2026-03-25T20.52.37.log` / `godot.log` 与 live `BUILD_FLAVOR.txt` 后，确认当前部署已关闭 DebugCardPoolFilter，但日志还未覆盖卡牌总览卡死现场。
- 进一步将 Debug 默认行为收敛为“只保留起始牌组替换与商店替换”，同时关闭图鉴强制可见 override。
- 结合 `godot2026-03-25T21.06.07.log` 与 `godot.log`，确认 `Debug` / `Release` 都复现卡死，开始从通用卡图资源路径问题收敛根因，并修正 `DiscardModCard` 的 Godot 资源路径格式。
- 重新验证 `Release` 与 `Debug` 构建；其中 `Debug` 一度被 Godot `ExportGodotPack` 的临时文件问题干扰，但随后已成功完成构建和部署。
- 统一调整 20 张自定义卡图的导入参数为压缩纹理 + mipmaps，并重新构建 `Release` / `Debug`、部署新的 Debug 运行时到 live 模组目录。
- 基于“非 Mod 卡也会卡顿”的新证据，收紧 `LocalizationRuntimePatch` 到 `DISCARDMOD-` key，并缓存语言解析结果；同时加入一次性聚合统计用于验证热路径是否真正被切窄。
- 审计卡图来源后确认 `generate_card_art.py` 只覆盖了 6 张牌，而源目录实际混有 10 张图片；现已补齐缺失的 4 张规格，并为全部 10 张牌重新生成 small/big 卡图后重新构建部署。
- 复核当前代码库与依赖文档后，仍未找到安全的进度/奖励 API 用来持久标记模组卡“已发现”；因此本轮继续保留“起始牌组替换 + 商店替换”作为唯一默认开启的 Debug 快速验证路径。
- 结合最新日志里的 `CARD.DISCARDMOD-*` 形态，补齐运行时本地化 key 归一化逻辑，使模组卡标题/描述不再因为完整模型 ID 前缀而回退英文。
