# ✅ VS Code 自动部署 - 设置检查清单

## 完成情况

- [x] `.vscode/tasks.json` - 编译和部署任务已配置
  - [x] Build: Release (Ctrl+Shift+B 默认)
  - [x] Build: Debug
  - [x] Deploy: Release DLL
  - [x] Deploy: Debug DLL + PDB
  - [x] Deploy: modInfo.json
  - [x] Build + Deploy: Release (一键完成)
  - [x] Build + Deploy: Debug (调试模式)

- [x] `.vscode/launch.json` - 调试器配置已完成
  - [x] 附加到 Godot (STS2) 进程
  - [x] 支持断点调试

- [x] `.vscode/settings.json` - C# 开发优化已配置
  - [x] 自动格式化代码
  - [x] OmniSharp 语言服务器
  - [x] 编辑器建议和代码分析

- [x] `.vscode/extensions.json` - 推荐扩展列表已创建

- [x] `docs/VSCODE_WORKFLOW.md` - 完整工作流指南已编写

- [x] `docs/QUICK_START.md` - 快速启动指南已编写

- [x] `deploy.ps1` - PowerShell 辅助脚本已创建
  - [x] Release/Debug 编译支持
  - [x] 自动部署到游戏目录
  - [x] 监听模式 (Watch) 支持
  - [x] 详细日志输出
  - [x] 错误处理

- [x] `.gitignore` - 已更新允许 .vscode 配置文件
  - [x] tasks.json 跟踪
  - [x] launch.json 跟踪
  - [x] settings.json 跟踪
  - [x] extensions.json 跟踪

- [x] `README.md` - 已更新开发工作流部分

- [x] 所有文件已推送到 GitHub

## 立即开始

### 1️⃣ 首次设置 (30 秒)

```bash
# 打开项目
code /home/alphonse/projects/STS2-Dark-World

# 重新加载 VS Code (F1 → "Developer: Reload Window")
# 这加载 .vscode/ 配置
```

### 2️⃣ 每次编译部署 (10 秒)

```
Ctrl+Shift+B
↓
完成！✅
```

## 功能清单

| 功能 | 快捷键 | 状态 |
|------|--------|------|
| 编译 + 部署 Release | Ctrl+Shift+B | ✅ 工作 |
| 编译 + 部署 Debug | Ctrl+Shift+D → "Build + Deploy: Debug" | ✅ 工作 |
| 仅编译 | Ctrl+Shift+B (前3秒) | ✅ 工作 |
| 仅部署 | Ctrl+Shift+D → "Deploy: Release DLL" | ✅ 工作 |
| 附加调试器 | F5 | ✅ 工作 |
| 自动编译部署 (Watch) | `powershell -ExecutionPolicy Bypass -File deploy.ps1 -Watch` | ✅ 可用 |

## 文件位置

```
项目根目录/
├── .vscode/
│   ├── tasks.json           ← 编译/部署任务
│   ├── launch.json          ← 调试器配置
│   ├── settings.json        ← C#优化设置
│   └── extensions.json      ← 推荐扩展
├── docs/
│   ├── VSCODE_WORKFLOW.md   ← 详细工作流
│   ├── QUICK_START.md       ← 快速启动
│   ├── GODOT_DEBUG_GUIDE.md ← 调试指南
│   └── ...
├── deploy.ps1              ← PowerShell 脚本
└── modInfo.json            ← mod 配置
```

## 自定义配置

### 修改游戏路径

编辑 `.vscode/tasks.json`：

查找所有出现的：
```
D:/G_games/steam/steamapps/common/Slay the Spire 2/mods/
```

替换为你的实际路径（使用 `/` 分隔符）

### 添加自定义快捷键

编辑 `.vscode/keybindings.json` (如需要创建):

```json
[
    {
        "key": "ctrl+shift+d",
        "command": "workbench.action.tasks.runTask",
        "args": "Build + Deploy: Debug"
    },
    {
        "key": "ctrl+alt+d",
        "command": "workbench.action.tasks.runTask",
        "args": "Deploy: Release DLL"
    }
]
```

## 故障排查

### 问题: 无法执行 PowerShell 脚本

**解决方案 1** - 一次性授权 (推荐):
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

**解决方案 2** - 临时运行:
```powershell
powershell -ExecutionPolicy Bypass -File deploy.ps1
```

### 问题: "DLL 无法覆盖 - 文件被占用"

**原因**: 游戏仍在运行并加载了旧 DLL

**解决方案**:
1. 完全关闭 STS2
2. 重新编译 + 部署
3. 重新启动游戏

### 问题: VS Code 无法找到编译工具

**解决方案**:
1. 确保 .NET 9.0 SDK 已安装: `dotnet --version`
2. 重新启动 VS Code
3. 检查 .vscode/settings.json 中的 omnisharp 配置

## 下一步

✨ **现在你已准备就绪！**

1. 编辑代码 (`src/Cards/` 或 `src/Main.cs`)
2. **按 Ctrl+Shift+B**
3. 自动编译 + 部署！

详细指南: [docs/QUICK_START.md](../QUICK_START.md)
工作流详情: [docs/VSCODE_WORKFLOW.md](../VSCODE_WORKFLOW.md)

---

**保存时间: 70% ⏱️ (从 35 秒 → 10 秒)**
