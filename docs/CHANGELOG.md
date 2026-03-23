# 版本历史

本文件记录已经落地到仓库中的变更，不记录纯设计想法。

## [Unreleased]

### 开发中

- 4 张卡牌的 `OnPlay()` 真实效果
- 弃牌触发事件监听
- 游戏内实际验证与平衡调整

## [0.1.0-alpha]

### 已完成

#### 框架

- 建立 `DiscardModMain` 模组入口
- 配置 `STS2DiscardMod` 日志前缀
- 建立 `STS2_Discard_Mod.json` manifest
- 项目可以在 `net9.0` 下成功构建

#### 卡牌

- 新增 4 张卡牌类
- 已将 4 张卡注册到 `RegentCardPool`
- 已补齐对应本地化键

#### 构建与部署

- 配置 `CopyToModsFolderOnBuild` 自动部署
- 统一 live 目录结构为 `mods/STS2_Discard_Mod/`
- 统一 DLL 名为 `STS2DiscardMod.dll`
- 修复 manifest 与 DLL 命名不一致导致的加载失败问题
- 停止把 `cards.json` 部署到 live 模组目录，避免被误判为 manifest

#### 文档

- 重写 `README.md`
- 收敛快速上手、开发、调试、Windows 部署、Godot 调试说明
- 将主要文档统一为中文

### 当前已知限制

- 卡牌目前主要是注册成功，效果未完成
- 弃牌触发机制尚未实现
- 游戏内完整验证尚未完成

## 后续版本方向

### [0.2.0] 计划

- 完成 4 张卡的实际效果
- 接入弃牌事件监听
- 完成至少一轮游戏内验证
- 根据结果调整数值与设计
