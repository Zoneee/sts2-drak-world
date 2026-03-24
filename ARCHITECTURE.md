# 架构

本仓库实现的是《Slay the Spire 2》弃牌触发实验性模组。当前架构目标是让卡牌扩展、运行时诊断、资源导出和 live 部署形成可观察、可验证的闭环，而不是引入过度抽象的通用框架。

## 核心模块
- `src/Main.cs`：模组入口，负责日志与 `Harmony.PatchAll()`
- `src/Cards/DiscardModCard.cs`：统一弃牌体系卡牌基类
- `src/Cards/*.cs`：10 张具体卡牌实现
- `src/Patches/*.cs`：运行时文本、卡库可见性、`ModelDb` 诊断、弃牌链路诊断与 Debug 专用过滤
- `src/STS2DiscardMod/` 与 `src/.godot/`：Godot 资源及导入缓存

## 关键约定
- 通过 `[Pool(typeof(RegentCardPool))]` 声明式注册卡牌
- `ModId`、manifest `id`、`AssemblyName` 必须一致，当前都为 `STS2DiscardMod`
- `BaseLib` 作为独立依赖必须在游戏目录中存在
- `cards.json` 不能直接复制到 live 模组目录

## 构建与部署链路
- 构建前生成本地化文件
- 构建后写出 `BUILD_FLAVOR.txt`
- 若设置 `GODOT_CLI_COMMAND`，则导出 `STS2DiscardMod.pck`
- 若探测到 `ModsPath`，自动复制 DLL、manifest、`.pck`、Harmony 依赖和 Godot 资源到 live 模组目录

## 详细来源
- `docs/architecture/index.md`
- `docs/architecture/project-structure.md`
- `docs/runbooks/deployment.md`
