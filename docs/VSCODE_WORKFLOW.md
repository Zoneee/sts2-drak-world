# 编辑器工作流速查（VS Code）

这份文档只说明 VS Code 里的任务、快捷键和推荐开发流程。

## 1. 最常用操作

### 日常构建

```text
Ctrl+Shift+B
```

会运行默认任务：`Build: Release`。

### 调试构建

使用任务面板选择：

- `Build: Debug`
- `Build + Deploy: Debug`

## 2. 当前任务列表

### 构建任务

| 任务名           | 用途     |
| ---------------- | -------- |
| `Build: Release` | 日常构建 |
| `Build: Debug`   | 调试构建 |
| `Clean Build`    | 清理输出 |

### 部署相关任务

| 任务名                          | 用途                 |
| ------------------------------- | -------------------- |
| `Deploy: Release DLL`           | 手动复制 Release DLL |
| `Deploy: Debug DLL + PDB`       | 手动复制 Debug 产物  |
| `Deploy: STS2_Discard_Mod.json` | 手动复制 manifest    |

### 组合任务

| 任务名                    | 用途                   |
| ------------------------- | ---------------------- |
| `Build + Deploy: Release` | 构建并手动部署 Release |
| `Build + Deploy: Debug`   | 构建并手动部署 Debug   |

## 3. 推荐工作流

### 场景 A：改代码后快速验证

1. 修改 `src/Main.cs` 或 `src/Cards/*.cs`
2. 按 `Ctrl+Shift+B`
3. 重启游戏
4. 看日志是否出现 `STS2DiscardMod`

### 场景 B：准备断点调试

1. 运行 `Build + Deploy: Debug`
2. 启动游戏
3. 打开“运行和调试”并附加到进程
4. 在源码里下断点

### 场景 C：路径有问题时临时手动部署

1. 正常执行构建
2. 使用单独的部署任务补复制
3. 确认 live 模组目录结构正确

## 4. 部署目录约定

当前仓库约定的正确 live 结构是：

```text
{游戏目录}/mods/STS2_Discard_Mod/
├── STS2DiscardMod.dll
├── STS2DiscardMod.pdb    # 仅 Debug 时存在
└── STS2_Discard_Mod.json
```

不要往这个目录里额外复制 `cards.json`。

## 5. 常用快捷键

| 快捷键         | 作用             |
| -------------- | ---------------- |
| `Ctrl+Shift+B` | 运行默认构建任务 |
| `Ctrl+Shift+P` | 打开命令面板     |
| `Ctrl+J`       | 打开或关闭面板   |
| `F5`           | 启动或附加调试器 |

## 6. 路径自定义

如果你的游戏安装路径和默认值不同，有两种方式：

1. 优先通过 `dotnet build ... -p:SteamLibraryPath=...` 覆盖
2. 只在必要时修改 `.vscode/tasks.json` 里的手动部署路径

## 7. 配合文档

- 想看完整开发说明：见 [DEV_GUIDE.md](DEV_GUIDE.md)
- 想看调试流程：见 [DEBUGGING.md](DEBUGGING.md)
- 想看 Windows 专项路径：见 [WINDOWS_DEPLOYMENT.md](WINDOWS_DEPLOYMENT.md)
