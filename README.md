# 项目总览：STS2 弃牌触发 Mod

为《Slay the Spire 2》添加一组围绕“弃牌后触发效果”设计的实验性卡牌。

当前仓库处于框架完成、效果开发中的阶段：

- 已完成：模组入口、4 张卡注册、构建与自动部署、基础文档
- 未完成：弃牌事件补丁、实际战斗效果、完整游戏内验证

## 当前实现

目前已经注册到 `RegentCardPool` 的卡牌共有 4 张：

| 中文名   | 类名                | 类型   | 稀有度   | 费用 | 当前状态           |
| -------- | ------------------- | ------ | -------- | ---- | ------------------ |
| 迅影斩   | `SwiftCut`          | Attack | Common   | 0    | 已注册，效果待实现 |
| 暗焰残页 | `DarkFlameFragment` | Skill  | Common   | 1    | 已注册，效果待实现 |
| 毒素记录 | `ToxinRecord`       | Skill  | Uncommon | 1    | 已注册，效果待实现 |
| 碎念回响 | `ShatteredEcho`     | Skill  | Rare     | 2    | 已注册，效果待实现 |

## 快速开始

### 1. 准备依赖

- 安装 `.NET SDK 9.0+`
- 从游戏目录复制 `sts2.dll` 到 `lib/sts2.dll`

### 2. 构建项目

```bash
dotnet build src/ --configuration Release
```

输出文件：

```text
src/bin/Release/net9.0/STS2DiscardMod.dll
```

### 3. 部署到游戏目录

构建成功后，项目内置的 MSBuild 目标会自动部署到：

```text
{游戏目录}/mods/STS2_Discard_Mod/
├── STS2DiscardMod.dll
├── STS2DiscardMod.pck
├── 0Harmony.dll
└── STS2_Discard_Mod.json
```

如果你在 VS Code 中开发，直接按：

```text
Ctrl+Shift+B
```

## 关键注意事项

- 模组加载器按 `id` 默认查找 `STS2DiscardMod.dll`，因此 manifest、代码中的 `ModId` 和构建输出名称必须保持一致
- 运行时不要把 `src/localization/eng/cards.json` 直接复制到模组目录；当前加载器会把额外的 `.json` 误判为 manifest
- live 模组目录至少要包含 `STS2DiscardMod.dll`、`STS2DiscardMod.pck`、`0Harmony.dll` 和 `STS2_Discard_Mod.json`
- 卡图这类 `res://STS2DiscardMod/...` 资源必须进入 `.pck`；只把 png 原文件复制到 mods 目录，游戏不会挂载它们

## 文档索引

- [docs/QUICK_START.md](docs/QUICK_START.md)：3 分钟快速上手
- [docs/DEV_GUIDE.md](docs/DEV_GUIDE.md)：完整开发指南
- [docs/DEBUGGING.md](docs/DEBUGGING.md)：调试与故障排查
- [docs/VSCODE_WORKFLOW.md](docs/VSCODE_WORKFLOW.md)：VS Code 工作流速查
- [docs/WINDOWS_DEPLOYMENT.md](docs/WINDOWS_DEPLOYMENT.md)：Windows 路径与手动部署补充
- [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)：项目结构与实现说明
- [docs/DESIGN.md](docs/DESIGN.md)：设计目标与后续规划
- [docs/MOD_REGISTRY.md](docs/MOD_REGISTRY.md)：卡牌注册表
- [docs/CHANGELOG.md](docs/CHANGELOG.md)：版本历史

## 仓库结构

```text
STS2-Dark-World/
├── src/                       # 源码与本地化
├── docs/                      # 项目文档
├── lib/                       # 本地游戏依赖，不提交到 git
├── STS2_Discard_Mod.json      # 模组清单
└── deploy.ps1                 # Windows 辅助部署脚本
```

## 当前阶段

版本：`v0.1.0-alpha`

- [x] 模组入口与日志系统
- [x] 4 张卡注册到 `RegentCardPool`
- [x] 构建、自动部署、基础文档
- [ ] `OnPlay()` 实际效果
- [ ] 弃牌触发逻辑
- [ ] 游戏内验证与平衡调整
