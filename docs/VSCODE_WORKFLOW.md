# 编辑器工作流速查（VS Code）

这份文档只说明 VS Code 里的任务、快捷键和推荐开发流程。

## 1. 最常用操作

### 日常构建

```text
Ctrl+Shift+B
```

会运行默认任务：`Build: Release`。

不会同时运行 Debug 和 Release。

### 调试构建

使用任务面板选择：

- `Build: Debug`
- `Build + Deploy: Debug`
- `Deploy: Debug Runtime`

## 2. 当前任务列表

### 构建任务

| 任务名           | 用途     |
| ---------------- | -------- |
| `Build: Release` | 日常构建 |
| `Build: Debug`   | 调试构建 |
| `Clean Build`    | 清理输出 |

### 部署相关任务

| 任务名                    | 用途                    |
| ------------------------- | ----------------------- |
| `Deploy: Release Runtime` | 手动复制 Release 运行时 |
| `Deploy: Debug Runtime`   | 手动复制 Debug 运行时   |

### 组合任务

| 任务名                                | 用途                        |
| ------------------------------------- | --------------------------- |
| `Build + Deploy: Release`             | 构建并手动部署 Release      |
| `Build + Deploy: Debug`               | 构建并手动部署 Debug        |
| `Build + Deploy + Attach Prep: Debug` | 为附加调试准备 Debug 运行时 |

## 3. 当前脚本列表

仓库现在提供这些脚本：

| 脚本                              | 用途                                    |
| --------------------------------- | --------------------------------------- |
| `scripts/build-debug.sh`          | 仅构建 Debug，不部署                    |
| `scripts/build-release.sh`        | 仅构建 Release，不部署                  |
| `scripts/deploy-debug.sh`         | 把现有 Debug 输出部署到 live mod 目录   |
| `scripts/deploy-release.sh`       | 把现有 Release 输出部署到 live mod 目录 |
| `scripts/build-deploy-debug.sh`   | 构建并部署 Debug                        |
| `scripts/build-deploy-release.sh` | 构建并部署 Release                      |

这些脚本会复用 `scripts/common.sh`，统一处理：

- Godot CLI 路径读取
- 输出目录定位
- live mod 目录定位
- `BUILD_FLAVOR.txt` 摘要输出

## 4. 推荐工作流

### 场景 A：改代码后快速验证

1. 修改 `src/Main.cs` 或 `src/Cards/*.cs`
2. 按 `Ctrl+Shift+B`
3. 如果需要进游戏验证，再运行 `Deploy: Release Runtime`
4. 重启游戏
5. 看日志是否出现 `STS2DiscardMod`

### 场景 B：准备断点调试

1. 运行 `Build + Deploy + Attach to Godot (STS2, same host only)`
2. 启动游戏
3. 选择 `Godot.exe` 或对应 Godot 进程
4. 在源码里下断点

### 场景 C：路径有问题时临时手动部署

1. 正常执行构建
2. 使用单独的部署任务补复制
3. 确认 live 模组目录结构正确

## 5. 依赖 `tasks.json` 的行为

当前这些 VS Code 行为依赖 `.vscode/tasks.json`：

- `Ctrl+Shift+B` 默认执行哪个任务
- `launch.json` 里的 `preLaunchTask` 在调试前会执行什么
- 任务面板里能看到哪些构建 / 部署入口
- Problems 面板如何识别 `dotnet build` 编译错误

如果删掉或改坏 `tasks.json`，最直接受影响的是：

- 默认构建快捷键
- 调试前自动 build/deploy
- 任务面板的一键构建和部署体验

## 6. 部署目录约定

当前仓库约定的正确 live 结构是：

```text
{游戏目录}/mods/STS2_Discard_Mod/
├── STS2DiscardMod.dll
├── STS2DiscardMod.pdb    # 仅 Debug 时存在
├── BUILD_FLAVOR.txt
├── STS2DiscardMod.pck
└── STS2_Discard_Mod.json
```

不要往这个目录里额外复制 `cards.json`。

## 7. 常用快捷键

| 快捷键         | 作用             |
| -------------- | ---------------- |
| `Ctrl+Shift+B` | 运行默认构建任务 |
| `Ctrl+Shift+P` | 打开命令面板     |
| `Ctrl+J`       | 打开或关闭面板   |
| `F5`           | 启动或附加调试器 |

## 8. 路径自定义

如果你的游戏安装路径和默认值不同，有两种方式：

1. 优先设置环境变量 `STS2_MOD_DIR=/your/mod/dir/STS2_Discard_Mod`
2. 或在脚本里走默认自动探测
3. Godot CLI 可以通过环境变量 `GODOT_CLI_COMMAND` 覆盖

## 9. 配合文档

- 想看完整开发说明：见 [DEV_GUIDE.md](DEV_GUIDE.md)
- 想看调试流程：见 [DEBUGGING.md](DEBUGGING.md)
- 想看 Windows 专项路径：见 [WINDOWS_DEPLOYMENT.md](WINDOWS_DEPLOYMENT.md)
