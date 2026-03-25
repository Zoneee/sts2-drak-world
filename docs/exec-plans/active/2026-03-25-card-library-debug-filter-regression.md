# 图鉴 Debug 卡池过滤回归修复计划

## 状态
执行中

## 背景
2026-03-25 最新运行日志显示，前一轮本地化、manifest 扫描、Harmony 参数名和卡图相关的前置阻断项已消失。新的主问题变为 Debug 模式下打开图鉴 / Card Library 时触发 `MockCardPool` 与 `You monster!` 异常。

## 问题摘要
- 当前 live 构建为 Debug，且 `BUILD_FLAVOR.txt` 显示 `DebugCardPoolFilter=true`。
- `ModelDbDiagnosticsPatch` 在 `ModelDb.Init()` 后通过反射直接改写 `RegentCardPool` 的 `_allCards` 与 `_allCardIds` 内部缓存。
- 打开图鉴 / Card Library 时，排序与过滤链路访问 `CardModel.Pool`，随后落入 `MockCardPool`，触发 `NeverEverCallThisOutsideOfTests_ClearOwner()` 并抛出 `You monster!`。
- 用户要求保留 Debug 快速验证能力，但不能继续以破坏图鉴链路为代价。
- 2026-03-25 晚间新现象：在已部署 `DebugCardPoolFilter=false` 的构建上，进入百科大全中的卡牌总览页面仍会卡死，无新异常日志落盘，说明风险点不再局限于旧的全局卡池裁剪链路。
- 2026-03-25 晚间补充定位：`Debug` 与 `Release` 都会卡死，且自定义卡的 `PortraitPath` / `CustomPortraitPath` 当前通过 `Path.Join(...)` 构造，Windows 运行时会生成 `\` 分隔符，与 Godot 资源路径格式不一致。
- 2026-03-25 深夜补充定位：用户确认局内单张抽牌时也会因卡图逐步加载而明显掉帧，随后再逐渐恢复响应，这进一步指向同步卡图资源加载，而不是仅限图鉴筛选页面的问题。
- 2026-03-25 深夜进一步定位：用户确认即使查看的不是储君卡池或 Mod 卡，卡牌总览仍会出现明显长时间卡顿；当前仍活跃的 `LocalizationRuntimePatch` 会拦截所有 `cards` 表文本查询，并在热路径上反射解析语言标记，因此需要先收紧到 `DISCARDMOD-` key 并缓存语言解析结果。
- 2026-03-25 深夜最新进展：用户已确认“加载速度正常”；当前剩余问题收敛为卡图错配、模组卡中文显示待复测，以及是否需要在不回退旧全局卡池方案的前提下增强 Debug 快速验证能力。

## 范围
- 修复 Debug 构建下图鉴 / Card Library 因不安全卡池裁剪导致的可见性与崩溃问题。
- 保留 Debug 起始牌组替换与商店替换这两条更安全的快速验证路径。
- 同步更新与该回归直接相关的运行手册与执行计划。

## 非范围
- 不重新处理 2026-03-24 已修复的 manifest、本地化 JSON 误部署、卡图导入或 Harmony 参数名问题。
- 不调整卡牌数值平衡、角色解锁规则或一般性 UI 行为。
- 不扩展为一次通用重构。

## 受影响区域
- `src/Patches/ModelDbDiagnosticsPatch.cs`
- `src/Patches/DebugCardPoolSettings.cs`
- `src/Patches/CardLibraryVisibilityPatch.cs`
- `src/Cards/DiscardModCard.cs`
- `src/Patches/LocalizationRuntimePatch.cs`
- `src/Utils/CardCatalog.cs`
- `scripts/generate_card_art.py`
- `src/STS2DiscardMod/images/card_portraits/*.png.import`
- `src/STS2DiscardMod/images/card_portraits/big/*.png.import`
- `src/Patches/DebugStartingDeckPatch.cs`
- `src/Patches/DebugMerchantInventoryPatch.cs`
- `src/STS2_Discard_Mod.csproj`
- `docs/runbooks/debug-failures.md`

## 约束
- 已采用 Skills：
  - `skills/plan-before-code.md`
  - `skills/repo-as-source-of-truth.md`
  - `skills/evidence-driven-delivery.md`
  - `skills/small-safe-prs.md`
  - `skills/observability-first-debugging.md`
  - `skills/boundary-validation.md`
  - `skills/fix-the-system-not-just-the-ticket.md`
- 不回退与本回归无关的既有修复。
- 若仓库内未发现安全的奖励生成挂点，本轮优先关闭高风险功能，而不是继续篡改 `CardPoolModel` 内部状态。

## 验收标准
- Debug 构建打开图鉴 / Card Library 时，不再出现 `MockCardPool`、`NeverEverCallThisOutsideOfTests_ClearOwner()` 或 `You monster!` 调用链。
- Debug 默认不再启用全局 `DebugCardPoolFilter`。
- Debug 默认不再启用图鉴强制可见 override。
- 自定义卡图资源路径统一使用 Godot 正斜杠路径格式，不再依赖平台路径分隔符。
- 自定义卡图导入参数切换到压缩纹理 + mipmaps，降低卡牌总览与抽牌时的同步加载成本。
- `LocalizationRuntimePatch` 只对 `DISCARDMOD-` 卡牌 key 尝试运行时本地化，不再为所有 `cards` 表查询执行语言解析与 catalog 查找。
- 运行时语言标记在首次解析后会被缓存复用，不再按文本查询次数重复扫描 `LocTable` 反射元数据。
- 运行时本地化必须同时兼容 `DISCARDMOD-*` 与 `CARD.DISCARDMOD-*` 两种 key 形态，避免游戏传入完整模型 ID 时重新回退到英文。
- 卡图生成脚本必须覆盖 `cards.catalog.json` 中全部 10 张牌；若脚本规格与 catalog 不一致，应在生成阶段直接失败，而不是继续产出混合来源的卡图文件。
- Debug 起始牌组替换与商店替换仍然保留并可用于快速验证模组卡。
- 运行手册与执行计划对 Debug 行为的描述与当前实现一致。

## 验证计划
- 构建 Debug 产物，确认项目可编译。
- 构建 Release 产物，确认项目可编译。
- 检查 `BUILD_FLAVOR.txt`，确认 `DebugCardPoolFilter=false`。
- 部署到 live 模组目录后，再次检查 live `BUILD_FLAVOR.txt`。
- 检查 20 个 `.png.import` 文件，确认 `compress/mode=2`、`compress/high_quality=true`、`mipmaps/generate=true` 已生效。
- 打开百科大全 / Card Library 后，检查日志中的 `[LocalizationRuntimePatch]` 聚合统计，确认大多数 `cards` 查询在语言解析前被快速拒绝，且语言解析次数不再随查询数线性增长。
- 在 `zh_CN` 下检查至少一张模组卡，确认标题和描述能够命中中文文本。
- 在 `zh_CN` 下确认模组卡标题/描述即使运行时 key 形态为 `CARD.DISCARDMOD-*` 也能命中中文，而不是只在裸 `DISCARDMOD-*` key 下生效。
- 运行 `scripts/generate_card_art.py`，确认 10 张卡图都被重新生成，且脚本不会因 catalog/spec 漏配而静默跳过旧文件。
- 重新打开图鉴 / Card Library，确认页面不再卡死，且日志不再出现与 `MockCardPool` / `You monster!` 相关的调用链。

## 验证结果
- 已完成代码修复：停止在 `ModelDb.Init()` 后通过反射改写 `RegentCardPool` 内部缓存。
- 已完成代码修复：Debug 默认不再启用 `DebugCardPoolFilter`，仅保留起始牌组替换与商店替换两条安全的快速验证路径。
- 已完成代码修复：Debug 默认不再启用图鉴强制可见 override，避免继续干预 `NCardLibraryGrid.GetCardVisibility()`。
- 已完成代码修复：自定义卡图资源路径改为固定使用 Godot 正斜杠路径，不再通过 `Path.Join(...)` 生成平台相关路径。
- 已完成代码修复：全部 20 张 small/big 自定义卡图的 `.import` 配置统一切换为 `compress/mode=2`、`compress/high_quality=true`、`mipmaps/generate=true`。
- 已完成代码修复：`LocalizationRuntimePatch` 现在会先对非 `DISCARDMOD-` key 做常量时间快速拒绝，只在模组卡 key 上才进入运行时本地化查找。
- 已完成代码修复：`LocalizationRuntimePatch` 会缓存首次解析出的语言标记，并输出一次性聚合统计，便于确认卡牌总览是否仍把所有 `cards` 查询拖进热路径。
- 已完成代码修复：`CardCatalog.TryGetLocalizedText()` 改为单次 `TryGetValue` 查找，移除热路径上的双重字典探测。
- 已完成代码修复：`CardCatalog` 现在会归一化并兼容可选的 `CARD.` 前缀，为 `DISCARDMOD-...title/description`、`CARD.DISCARDMOD-...title/description` 以及对应的 `cards.` 形式都建立查找入口。
- 已完成代码修复：`scripts/generate_card_art.py` 现在会先校验 `cards.catalog.json` 的 `assetName` 是否与卡图规格完全一致，并已补齐缺失的 4 张卡图规格，使 10 张卡图都来自同一套可重现生成逻辑。
- 已完成代码修复：4 张历史来源不一致的卡图（`dark_flame_fragment`、`swift_cut`、`toxin_record`、`shattered_echo`）已通过统一脚本重新生成，避免继续混用两批来源不同的资源。
- 已验证构建：本轮最终 `Debug` 构建与部署成功，`src/bin/Debug/net9.0/BUILD_FLAVOR.txt` 与 live `BUILD_FLAVOR.txt` 显示 `BuiltUtc=2026-03-25T16:01:49Z`。
- 已验证构建：本轮最终 `Release` 构建成功，`src/bin/Release/net9.0/BUILD_FLAVOR.txt` 显示 `BuiltUtc=2026-03-25T16:02:02Z`。
- 已验证构建：Release 构建成功，`src/bin/Release/net9.0/BUILD_FLAVOR.txt` 显示 `BuiltUtc=2026-03-25T13:12:06Z`。
- 已验证构建：Debug 构建成功，`src/bin/Debug/net9.0/BUILD_FLAVOR.txt` 显示 `BuiltUtc=2026-03-25T13:13:34Z`。
- 已验证部署：Debug 运行时已重新部署到 live 模组目录，`BUILD_FLAVOR.txt` 显示 `DebugCardPoolFilter=false`。
- 已验证源码状态：20 个 `.png.import` 文件均显示新的压缩纹理与 mipmaps 参数。
- 已验证当前构建：`src/bin/Release/net9.0/BUILD_FLAVOR.txt` 显示 `BuiltUtc=2026-03-25T13:39:39Z`，`src/bin/Debug/net9.0/BUILD_FLAVOR.txt` 与 live `BUILD_FLAVOR.txt` 显示 `BuiltUtc=2026-03-25T13:40:05Z`。
- 已确认 `godot2026-03-25T20.08.50.log` 不能作为本轮修复后的验证证据：该文件内容内部时间戳仍为 `2026/3/25 19:39:57`，并继续打印旧行为日志 `DEBUG ONLY: RegentCardPool restricted...`，与当前源码和 live `BUILD_FLAVOR.txt` 的 `DebugCardPoolFilter=false` 不一致。
- 已确认 `godot2026-03-25T20.52.37.log` / `godot.log` 仅覆盖主菜单启动与正常退出，不包含进入百科大全卡牌总览后的失败证据。
- 已确认 `godot2026-03-25T21.06.07.log` / `godot.log` 仍在 `Common` 资源加载后结束，且 `Debug` / `Release` 均可复现卡死，因此当前根因判断已从 Debug 专用补丁收敛到通用卡牌资源/图鉴访问链路。
- 待补运行时证据：重新打开图鉴 / Card Library 后的新日志，或在卡死后强制结束进程留下的新尾部日志。

## 风险
- 在获取新的运行时日志前，仍不能宣称图鉴回归已被最终关闭。
- 若继续误用旧日志文件或内容被覆盖的日志文件作为验证依据，会导致对修复状态的判断失真。
- 当前“卡牌总览卡死”更像无异常栈落盘的 UI 卡住；若关闭图鉴强制可见后仍复现，需要进一步抓取进入页面前后的新增日志或性能采样。
- 若卡图路径修复后仍复现，则需要继续检查卡牌总览访问的其他同步属性，例如卡牌描述、本地化或 BaseLib 对自定义卡的预览构造流程。
- 若当前热路径收紧后卡牌总览仍然明显卡顿，则下一轮应直接补“卡牌总览是否枚举全部卡并触发 PortraitPath”的诊断，而不是继续凭直觉扩大资源调参范围。
- 最新日志仍出现 `Progress parse: Unknown card ID: CARD.DISCARDMOD-*`，说明“把模组卡持久标记为已发现”这条能力目前没有发现安全 API；在找到安全进度/奖励挂点前，不应为了恢复更强 Debug 验证而重新启用旧的全局卡池裁剪或图鉴强制可见 override。
- 即使导入参数已调整，若游戏仍反复使用旧 `.ctex` 缓存或旧进程未完全退出，首次验证结果仍可能受旧资源影响。
- 当前回退方案不再提供“全局只从模组卡池拿牌”的 Debug 加速路径；若后续仍需要该能力，必须在更安全的奖励/提供链路上重新实现。

## 决策日志
- 2026-03-25：确认图鉴回归由 DebugCardPoolFilter 通过反射改写 `RegentCardPool` 缓存引起。
- 2026-03-25：由于仓库内未发现安全的奖励生成挂点，本轮先禁用该开关并保留更安全的起始牌组 / 商店 Debug 能力。
- 2026-03-25：在 live 构建已确认 `DebugCardPoolFilter=false` 后，仍出现百科大全卡牌总览卡死，因此先禁用 `CardLibraryVisibilityPatch` 的默认 override，停止继续干预图鉴可见性判定。
- 2026-03-25：在 `Debug` / `Release` 都复现且日志无异常栈后，优先修复自定义卡图 Godot 资源路径的跨平台格式问题，因为卡牌总览会集中访问这些路径。
- 2026-03-25：在用户补充“局内抽牌也会因卡图逐步加载而卡顿”后，先实施资源导入参数优化，把纹理加载成本降下来，再继续处理卡图错配和本地化。
- 2026-03-25：在用户确认非 Mod 卡也会触发明显长时间卡顿后，本轮优先级改为先收紧 `LocalizationRuntimePatch` 的全局 `cards` 文本热路径，再决定是否回到卡图枚举/映射问题。
- 2026-03-25：在用户确认“加载速度正常”后，剩余工作改为处理卡图错配，并只在找到安全的奖励/提供边界时再增强 Debug 快速验证能力。

## 进展日志
- 2026-03-25：对照最新日志确认图鉴回归链路。
- 2026-03-25：移除 `ModelDbDiagnosticsPatch` 中对 `RegentCardPool` 的全局缓存篡改。
- 2026-03-25：将 Debug 默认行为收敛为起始牌组替换与商店替换，并重新构建、部署。
- 2026-03-25：核对 `godot2026-03-25T20.08.50.log`、当前源码和 live `BUILD_FLAVOR.txt` 后，确认该日志反映的是旧运行行为，不是本轮部署后的有效验证结果。
- 2026-03-25：核对 `godot2026-03-25T20.52.37.log` / `godot.log` 与 live `BUILD_FLAVOR.txt` 后，确认当前部署已关闭 DebugCardPoolFilter，但日志还未覆盖卡牌总览卡死现场。
- 2026-03-25：进一步将 Debug 默认行为收敛为“只保留起始牌组替换与商店替换”，同时关闭图鉴强制可见 override。
- 2026-03-25：结合 `godot2026-03-25T21.06.07.log` 与 `godot.log`，确认 `Debug` / `Release` 都复现卡死，开始从通用卡图资源路径问题收敛根因，并修正 `DiscardModCard` 的 Godot 资源路径格式。
- 2026-03-25：重新验证 `Release` 与 `Debug` 构建；其中 `Debug` 一度被 Godot `ExportGodotPack` 的临时文件问题干扰，但随后已成功完成构建和部署。
- 2026-03-25：统一调整 20 张自定义卡图的导入参数为压缩纹理 + mipmaps，并重新构建 `Release` / `Debug`、部署新的 Debug 运行时到 live 模组目录。
- 2026-03-25：基于“非 Mod 卡也会卡顿”的新证据，收紧 `LocalizationRuntimePatch` 到 `DISCARDMOD-` key，并缓存语言解析结果；同时加入一次性聚合统计用于验证热路径是否真正被切窄。
- 2026-03-25：审计卡图来源后确认 `generate_card_art.py` 只覆盖了 6 张牌，而源目录实际混有 10 张图片；现已补齐缺失的 4 张规格，并为全部 10 张牌重新生成 small/big 卡图后重新构建部署。
- 2026-03-25：复核当前代码库与依赖文档后，仍未找到安全的进度/奖励 API 用来持久标记模组卡“已发现”；因此本轮继续保留“起始牌组替换 + 商店替换”作为唯一默认开启的 Debug 快速验证路径。
- 2026-03-25：结合最新日志里的 `CARD.DISCARDMOD-*` 形态，补齐运行时本地化 key 归一化逻辑，使模组卡标题/描述不再因为完整模型 ID 前缀而回退英文。

## 后续事项
- 启动游戏并重新打开百科大全中的卡牌总览页面，收集一份与当前 live `BUILD_FLAVOR.txt` 时间一致的新日志证据。
- 在同一轮复测里，再验证局内抽到单张模组卡时是否仍出现明显长时间卡顿。
- 若再次卡死且无异常栈，结束游戏进程后保留新的 `godot.log` 尾部，作为下一轮定位的现场证据。
- 若后续仍需要“只出模组卡”的 Debug 奖励路径，优先寻找奖励生成或卡牌提供边界做定向过滤，不再回到 `ModelDb.Init()` 后修改 `CardPoolModel` 内部缓存的方案。