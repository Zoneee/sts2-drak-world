# 版本历史

本文件只记录已经落地到仓库中的变更。

## [Unreleased]

### 卡牌

- 补全原有 4 张牌的 `OnPlay()` 与弃牌触发实现
- 新增 6 张弃牌体系卡牌：`AshenAegis`、`CripplingManuscript`、`EmberVolley`、`RecallSurge`、`FadingFormula`、`FinalDraft`
- 当前卡组总数提升到 10 张，并全部接入 `RegentCardPool`

### 日志与排障

- 在 `DiscardModCard` 中加入统一的打出/弃牌触发日志
- 新增 `DiscardDiagnosticsPatch`，记录 `CardCmd.Discard` 与 `CardCmd.DiscardAndDraw`
- 扩展 `ModelDbDiagnosticsPatch`，改为自动遍历全部弃牌体系卡牌
- `Main.cs` 初始化时输出自动发现到的卡牌列表

### 文档

- 刷新 `README.md`、`QUICK_REFERENCE.md`、`DEV_GUIDE.md`、`DEBUGGING.md`
- 更新 `MOD_REGISTRY.md` 为 10 张牌的真实状态
- 清理过期的“只有 4 张牌 / 效果未实现 / 手工注册”描述
- 补充本轮成功经验总结

### 已知限制

- 当前规则仍然是“任何弃牌都会触发这张牌的弃牌效果”
- 仍建议继续做完整游戏内验证和数值平衡
- 未设置 `GODOT_CLI_COMMAND` 时，卡图资源不会被导出到 `.pck`

## [0.1.0-alpha]

### 已完成

- 建立 `DiscardModMain` 模组入口
- 配置 `STS2DiscardMod` 日志前缀
- 建立 `STS2_Discard_Mod.json` manifest
- 配置 `CopyToModsFolderOnBuild` 自动部署
- 统一 DLL 名为 `STS2DiscardMod.dll`
- 停止把 `cards.json` 部署到 live 模组目录，避免被误判为 manifest
- 收敛主要中文文档
