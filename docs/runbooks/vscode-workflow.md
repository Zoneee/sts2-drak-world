# VS Code 工作流运行手册

状态：有效
负责人：工程团队
最后评审：2026-03-24

## 目标
统一仓库在 VS Code 中的构建、部署、调试和日志查看路径，降低“改了代码但不知道该用哪个任务验证”的成本。

## 当前任务列表
### 构建任务
- `Build: Release`：默认日常构建
- `Build: Debug`：调试构建
- `Clean Build`：清理输出

### 部署任务
- `Deploy: Release Runtime`
- `Deploy: Debug Runtime`

### 组合任务
- `Build + Deploy: Release`
- `Build + Deploy: Debug`
- `Build + Deploy + Attach Prep: Debug`

### 调试辅助
- `Attach Debugger: Godot`

## 推荐工作流
### 场景 A：改完代码后快速验证
1. 修改 `src/Main.cs`、`src/Cards/*.cs` 或 `src/Patches/*.cs`。
2. 执行 `Ctrl+Shift+B` 触发默认 `Build: Release`。
3. 如需显式部署，补跑 `Deploy: Release Runtime`。
4. 启动游戏并检查 `BepInEx/LogOutput.log`。

### 场景 B：准备断点调试
1. 运行 `Build + Deploy + Attach Prep: Debug`。
2. 启动游戏。
3. 执行 `Attach Debugger: Godot`，按提示附加到 Godot 进程。
4. 在 `DiscardModMain.Initialize()`、`DiscardModCard.AfterCardDiscarded()` 或目标卡牌 `OnPlay()` 上设断点。

### 场景 C：部署路径不确定
1. 先运行 `Build: Debug` 或 `Build: Release`。
2. 再使用 `Deploy: Debug Runtime` 或 `Deploy: Release Runtime` 验证脚本输出。
3. 如仍失败，转到 `deployment.md` 做手动部署与路径核对。

## 脚本对应关系
- `scripts/build-release.sh`：仅构建 Release
- `scripts/build-debug.sh`：仅构建 Debug
- `scripts/deploy-release.sh`：部署 Release 运行时
- `scripts/deploy-debug.sh`：部署 Debug 运行时
- `scripts/build-deploy-release.sh`：构建并部署 Release
- `scripts/build-deploy-debug.sh`：构建并部署 Debug
- `scripts/common.sh`：共享路径、环境变量与部署逻辑

## 常用快捷键
- `Ctrl+Shift+B`：运行默认构建任务
- `Ctrl+Shift+P`：打开命令面板
- `F5`：启动或附加调试器
- `Ctrl+J`：切换面板

## 常见问题
### 默认构建不是我想要的配置
默认绑定的是 `Build: Release`。需要调试特性时，明确从任务面板选择 `Build: Debug` 或 `Build + Deploy + Attach Prep: Debug`。

### 断点不生效
优先检查：
- 是否部署的是 `Debug` 产物
- live 模组目录中是否存在 `STS2DiscardMod.pdb`
- `BUILD_FLAVOR.txt` 是否显示 `Configuration=Debug`
- 当前附加的是否是正确的 Godot 进程

### WSL 开发，Windows 运行游戏
WSL 窗口通常只能附加 WSL 侧进程。若游戏运行在 Windows 上，建议在 Windows 本机 VS Code 窗口中执行附加，WSL 仍负责构建与部署。

## 延伸阅读
- `local-dev.md`
- `deployment.md`
- `debug-failures.md`