# 系统不变量

状态：有效
负责人：工程团队
最后评审：2026-03-28

## 用途
记录系统中必须始终成立的约束，任何改动不得违反这些不变量。

## 格式
每条不变量包含：
- 编号
- 描述
- 适用范围
- 验证方式

## 不变量列表

| 编号    | 描述                                                                                                                                                                  | 适用范围       | 验证方式                                                                             |
| ------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------------- | ------------------------------------------------------------------------------------ |
| INV-001 | `DiscardModMain.ModId`、`AssemblyName`（csproj）、manifest `id` 三者必须一致，否则模组无法加载                                                                        | 整体           | 构建脚本 + 手工比对                                                                  |
| INV-002 | 所有卡牌必须通过 `[Pool(typeof(RegentCardPool))]` 声明进池，不允许在入口手工注册                                                                                      | `src/Cards/`   | ModelDb 诊断补丁在运行时校验                                                         |
| INV-003 | 所有卡牌必须通过 `DiscardModCard` 基类实现，不允许绕过基类直接继承游戏原生类                                                                                          | `src/Cards/`   | 编译期继承检查                                                                       |
| INV-004 | 能力牌触发判断必须通过 `DiscardTriggerRuntime.ConsumePowerDiscardEvent(card)` 完成，不允许直接判断 `card` 来源                                                        | `src/Powers/`  | 代码评审                                                                             |
| INV-005 | Power 类必须同时重写 `CustomPackedIconPath`（64×64）和 `CustomBigIconPath`（256×256），且路径以 `res://STS2DiscardMod/images/powers/` 为前缀                          | `src/Powers/`  | 代码评审 + 运行时 atlas 日志                                                         |
| INV-006 | `LocalizationRuntimePatch` 必须以 `[HarmonyBefore("BaseLib")]` + `[Priority.First]` 运行，且同时覆盖 `cards` 与 `powers` 两张表，否则能力牌名称与描述会降级为 ID 原文 | `src/Patches/` | 游戏日志中不出现 `[WARN] [BaseLib] GetRawText: Key 'DISCARDMOD-*_POWER.*' not found` |
| INV-007 | Power 类的回合内累计计数（如 `DarkMomentumPower.discardsSinceLastTrigger`）必须在 `BeforeTurnEnd()` 中归零                                                            | `src/Powers/`  | 代码评审                                                                             |
| INV-008 | `cards.catalog.json` 中能力条目必须包含 `smartDescription` 字段，使生成器将其写入 `powers.json` 而非 `cards.json`                                                     | `src/Data/`    | 构建期生成器输出                                                                     |
| INV-009 | `cards.json` 不允许直接复制到 live 模组目录（本地化文件在游戏内通过 BaseLib Harmony 补丁动态注入）                                                                    | 部署脚本       | 脚本 exclude 规则                                                                    |
| INV-010 | 调试构建与发布构建的行为差异以 `BUILD_FLAVOR.txt` 为准，不允许通过其他机制区分构建类型                                                                                | 整体           | 构建脚本                                                                             |
