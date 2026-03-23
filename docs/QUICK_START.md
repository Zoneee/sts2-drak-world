# 🚀 VS Code 一键部署 - 快速开始

## ⚡ 最快 5 秒启动

### 第一次设置（一次性）

1. **VS Code 中打开项目**
   ```bash
   code /home/alphonse/projects/STS2-Dark-World
   ```

2. **重新加载 VS Code** (F1 → "Developer: Reload Window")
   - 这样会加载 `.vscode/` 配置

3. **完成！** ✅

### 每次编译 + 部署

**按 Ctrl+Shift+B**

就这样！自动完成：
- ✅ 编译代码
- ✅ 复制 DLL 到游戏目录  
- ✅ 复制 modInfo.json

输出示例：
```
Build succeeded.
   0 Warning(s)
   0 Error(s)
✅ Release DLL deployed!
✅ modInfo.json deployed!
```

---

## 🎮 完整工作流（1 分钟）

### 步骤 1: 编辑代码

编辑 `src/Cards/DarkFlameFragment.cs` 或其他卡牌文件

### 步骤 2: 一键部署

```
Ctrl+Shift+B
↓
等待完成（通常 5-10 秒）
```

### 步骤 3: 测试

```
Alt+Tab → 切换到 STS2
Ctrl+Shift+Esc → 关闭游戏（或直接关闭）
重新启动 STS2
创建新游戏 → 选 Mystic → 检查卡牌
```

### 步骤 4: 调试（可选）

如果需要设置断点：

```
Ctrl+Shift+D → 选 "Build + Deploy: Debug"
↓
启动游戏
↓
F5 → Attach to Process → Godot.exe
↓
在代码中点击行号左边设置红色断点
↓
游戏中触发卡牌效果 → 代码自动暂停
```

---

## 📋 所有快捷键

| 按键 | 结果 |
|------|------|
| **Ctrl+Shift+B** | 编译 + 部署（默认 Release）⭐ 最常用 |
| **Ctrl+Shift+D** | 打开任务菜单（选其他选项） |
| **F5** | 附加调试器 |
| **Ctrl+`** | 打开/关闭终端 |
| **Ctrl+J** | 切换 VS Code 面板 |

---

## 🐛 如果部署失败

### 错误: "PowerShell 脚本禁用"

执行一次：
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### 错误: "文件无法访问"

检查游戏是否在运行。如果在运行，需要先：
```
关闭游戏 → 再部署 DLL → 重启游戏
```

### 错误: "找不到路径"

编辑 `.vscode/tasks.json`，修改：
```json
"D:/G_games/steam/steamapps/common/Slay the Spire 2/mods/"
```

替换为你的实际游戏路径。

---

## 💾 每次编译后的文件

```
D:\G_games\steam\steamapps\common\Slay the Spire 2\mods\
├── STS2_Discard_Mod.dll   ← 自动复制
├── modInfo.json           ← 自动复制
└── baselib/               ← 已有（无需处理）
```

---

## 🔄 开发循环速度对比

### ❌ 手动方式（之前）
1. 编译 → 5 秒
2. 打开文件管理器 → 5 秒
3. 找到 mods 文件夹 → 5 秒
4. 复制 DLL → 10 秒
5. 复制 modInfo.json → 10 秒
6. **总耗时: ~35 秒 ⏱️**

### ✅ VS Code 一键方式（现在）
1. Ctrl+Shift+B → 完全自动 → 10 秒
2. **总耗时: ~10 秒 ⏱️**

**节省 70% 时间！** 🚀

---

## 📚 更多文档

- 详细工作流: [docs/VSCODE_WORKFLOW.md](../VSCODE_WORKFLOW.md)
- 调试指南: [docs/GODOT_DEBUG_GUIDE.md](../GODOT_DEBUG_GUIDE.md)
- 部署指南: [docs/WINDOWS_DEPLOYMENT.md](../WINDOWS_DEPLOYMENT.md)

---

**现在就试试: Ctrl+Shift+B** ✨
