# 快速参考卡片

## 最常用命令

```text
Ctrl+Shift+B
```

默认行为：构建 Release。

## 当前正确的 live 目录结构

```text
{游戏目录}/mods/STS2_Discard_Mod/
├── STS2DiscardMod.dll
└── STS2_Discard_Mod.json
```

不要额外复制 `cards.json`。

## 常用任务

- `Build: Release`
- `Build: Debug`
- `Build + Deploy: Release`
- `Build + Deploy: Debug`
- `Clean Build`

## 调试最短流程

1. 运行 `Build + Deploy: Debug`
2. 启动游戏
3. 附加到 STS2 或 Godot 进程
4. 在 `src/Main.cs` 或卡牌类下断点

## 如果构建后游戏没更新

先检查三件事：

1. 游戏是否还在占用旧 DLL
2. live 模组目录里是否有多余 JSON
3. 当前目录里是否真的存在 `STS2DiscardMod.dll`

## 对应文档

- 快速上手：`docs/QUICK_START.md`
- 完整开发：`docs/DEV_GUIDE.md`
- 调试排障：`docs/DEBUGGING.md`
- VS Code 任务：`docs/VSCODE_WORKFLOW.md`
