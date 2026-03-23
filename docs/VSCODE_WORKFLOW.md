# VS Code 快速工作流指南

## 🎯 快速启动

### 一键编译 + 部署

**使用 Ctrl+Shift+B 运行默认构建任务:**

```
Ctrl+Shift+B  →  自动运行 "Build: Release"
```

### 完整编译 + 部署流程

**Ctrl+Shift+D 打开任务菜单，选择:**

```
"Build + Deploy: Release"  (推荐用于快速测试)
或
"Build + Deploy: Debug"    (用于调试会话)
```

两者都会自动：
1. 编译 mod
2. 复制 DLL 到游戏目录
3. 复制 modInfo.json 到游戏目录

---

## 📋 所有可用任务

### 编译任务

| 任务名称 | 快捷键 | 功能 |
|--------|-------|------|
| **Build: Release** | `Ctrl+Shift+B` | 编译 Release 版本（优化，用于最终部署） |
| Build: Debug | 无 | 编译 Debug 版本（包含调试符号） |
| Clean Build | 无 | 清理所有编译输出 |

### 部署任务

| 任务名称 | 功能 |
|--------|------|
| Deploy: Release DLL | 复制 Release DLL 到游戏目录 |
| Deploy: Debug DLL + PDB | 复制 Debug DLL + 调试符号到游戏目录 |
| Deploy: modInfo.json | 复制 modInfo.json 配置文件 |

### 组合任务（推荐）

| 任务名称 | 功能 | 推荐场景 |
|--------|------|--------|
| **Build + Deploy: Release** | 编译 → 部署 Release | 快速测试，最终部署 |
| **Build + Deploy: Debug** | 编译 → 部署 Debug | 调试会话前 |

### 调试任务

| 任务名称 | 功能 |
|--------|------|
| Attach Debugger: Godot | 显示如何附加到 Godot.exe 进程 |

---

## 🚀 典型工作流

### 场景 1: 快速测试（最常用）

```
1. 编辑代码
   └─ 修改 src/Cards/*.cs or src/Main.cs

2. 按 Ctrl+Shift+B (或 Ctrl+Shift+D → Build + Deploy: Release)
   └─ 自动编译 + 部署

3. 切换到游戏
   └─ 重启 STS2
   └─ 创建新游戏测试
```

### 场景 2: 调试会话

```
1. 按 Ctrl+Shift+D → "Build + Deploy: Debug"
   └─ 编译 + 部署 Debug 版本（带符号）

2. 启动游戏
   └─ STS2 会加载最新的 DLL

3. 按 F5 或 Debug → Attach to Process
   └─ 选择 Godot.exe
   └─ 现在可以在代码中设置断点

4. 在游戏中触发效果
   └─ 代码会在断点处暂停
   └─ 可以检查变量、堆栈等
```

### 场景 3: 添加新卡牌

```
1. 创建新文件 src/Cards/NewCard.cs

2. 编写代码

3. Ctrl+Shift+B 编译
   └─ 错误会在 VS Code "最多" 窗口显示

4. 如果编译成功：Ctrl+Shift+D → "Build + Deploy: Release"
   └─ 自动部署

5. 重启游戏测试
```

---

## ⌨️ 键盘快捷键速查

| 快捷键 | 操作 |
|--------|------|
| `Ctrl+Shift+B` | 运行默认构建 (Release) |
| `Ctrl+Shift+D` | 打开任务菜单（选择其他构建选项） |
| `Ctrl+`+`` | 开启/关闭集成终端 |
| `Ctrl+J` | 切换面板（查看编译输出） |
| `F5` | 启动调试/附加调试器 |

### 自定义快捷键

编辑 `.vscode/keybindings.json` 添加快捷键：

```json
[
    {
        "key": "ctrl+shift+d",
        "command": "workbench.action.tasks.runTask",
        "args": "Build + Deploy: Debug"
    },
    {
        "key": "ctrl+shift+r",
        "command": "workbench.action.tasks.runTask",
        "args": "Build + Deploy: Release"
    }
]
```

---

## 🔧 自定义部署路径

如果你的游戏路径不同，编辑 `.vscode/tasks.json`：

找到：
```json
"args": ["-Command", "Copy-Item -Path ... -Destination 'D:/G_games/steam/steamapps/common/Slay the Spire 2/mods/' ..."]
```

替换路径：
```json
"args": ["-Command", "Copy-Item -Path ... -Destination 'C:/YOUR_PATH/Slay the Spire 2/mods/' ..."]
```

> ⚠️ 在 PowerShell 中，Windows 路径使用 `/` 或 `\\` 都可以，但 `/` 更简洁。

---

## 📊 工作流图

```
编辑代码
   ↓
Ctrl+Shift+B (编译)
   ↓
自动部署 ✅
   ↓
切换游戏
   ↓
重启 STS2
   ↓
创建新游戏测试
   ↓
F5 附加调试器（可选）
   ↓
检查卡牌/效果
```

---

## 🎮 游戏部署路径

```
D:\G_games\steam\steamapps\common\Slay the Spire 2\mods\
├── STS2_Discard_Mod.dll    ← Release 或 Debug 版本
├── STS2_Discard_Mod.pdb    ← 仅 Debug 时存在
├── modInfo.json            ← mod 元数据
└── baselib/                ← 依赖 mod
```

---

## ✅ 验证部署成功

任务执行后，终端应显示：

```
✅ Release DLL deployed!
✅ modInfo.json deployed!
```

如果看不到这些信息，检查：
1. 游戏路径是否正确（编辑 tasks.json）
2. PowerShell 执行策略：
   ```powershell
   Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
   ```
3. 文件权限（mods 目录是否可写）

---

## 💡 Pro 技巧

### 自动监听文件变化

VS Code 可以配置自动编译。在 `.vscode/settings.json` 添加：

```json
{
    "dotnet.autoGenerateAssets": true,
    "editor.formatOnSave": true
}
```

### Watch 模式（实验性）

如果需要完全自动化，创建 PowerShell 脚本 `watch-deploy.ps1`：

```powershell
param([string]$Config = "Release")

while ($true) {
    Clear-Host
    Write-Host "🔄 编译中..." -ForegroundColor Cyan
    dotnet build "src/" --configuration $Config
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ 编译成功，部署中..." -ForegroundColor Green
        Copy-Item "src/bin/$Config/net9.0/STS2_Discard_Mod.dll" -Destination "D:/G_games/steam/steamapps/common/Slay the Spire 2/mods/" -Force
        Write-Host "✅ 部署完成！", $((Get-Date).ToString("HH:mm:ss")) -ForegroundColor Green
    } else {
        Write-Host "❌ 编译失败!" -ForegroundColor Red
    }
    
    Write-Host "`n⏳ 等待文件变化... (按 Ctrl+C 停止)" -ForegroundColor Yellow
    Start-Sleep -Seconds 5
}
```

运行：
```bash
powershell -ExecutionPolicy Bypass -File watch-deploy.ps1
```

---

**🎉 现在你可以按 Ctrl+Shift+B 一键编译+部署！**
