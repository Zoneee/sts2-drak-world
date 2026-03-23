# 🚀 VS Code 一键部署 - 参考卡片

## 🎯 最常用

```
Ctrl+Shift+B  →  编译 + 部署 ✅
```

**自动执行:**
- ✅ 编译生成 DLL
- ✅ 复制到 mods 目录
- ✅ 复制 modInfo.json

## ⌨️ 所有快捷键

| 按键             | 功能                     | 用途         |
| ---------------- | ------------------------ | ------------ |
| **Ctrl+Shift+B** | Build + Deploy (Release) | 🟢 日常开发   |
| **Ctrl+Shift+D** | 打开任务菜单             | 选择其他任务 |
| **F5**           | 附加调试器               | 设置断点     |
| **Ctrl+`**       | 打开终端                 | 查看输出     |

## 📋 任务菜单 (Ctrl+Shift+D)

选择:
- `Build: Release` - 只编译 Release
- `Build: Debug` - 只编译 Debug (含调试符号)
- `Deploy: Release DLL` - 只部署 Release DLL
- `Deploy: Debug DLL + PDB` - 只部署 Debug + 符号
- `Deploy: modInfo.json` - 只部署配置
- `Build + Deploy: Debug` - 调试模式
- `Clean Build` - 清理编译

## 🔄 开发循环

```
编码          Ctrl+Shift+B     游戏测试
  ↓              ↓               ↓
编辑文件    自动编译+部署    重启STS2
  ↑                              ↓
←←←←←←←←← 重复 ←←←←←←←←←←←←←←
```

## 🐛 调试模式

```
1. Ctrl+Shift+D → "Build + Deploy: Debug"
   ↓ (等待编译完成)
2. 启动游戏
   ↓
3. F5 → Attach to Process → Godot.exe
   ↓
4. 代码左边点击行号设置红色断点
   ↓
5. 游戏中触发卡牌 → 代码暂停
   ↓
6. 悬停查看变量 / Ctrl+Shift+D 调用堆栈
```

## 📁 文件位置

```
.vscode/
├── tasks.json      编译 + 部署任务
├── launch.json     调试配置
├── settings.json   C# 优化
└── extensions.json 推荐扩展

deploy.ps1         PowerShell 脚本 (可选)
modInfo.json       Mod 配置文件
docs/QUICK_START.md 快速指南
```

## ✅ 验证设置

```bash
# 检查文件
ls -la .vscode/

# 测试编译
dotnet build src/ --configuration Release

# 查看 mods 目录
ls -la "D:/G_games/steam/steamapps/common/Slay the Spire 2/mods/"
```

## 💡 Pro 技巧

### 自动编译监听

```powershell
powershell -ExecutionPolicy Bypass -File deploy.ps1 -Watch
```
文件变化时自动编译部署

### 仅查看输出

编辑后按 `Ctrl+J` 打开/关闭终端面板

### 清理旧编译

```
Ctrl+Shift+D → "Clean Build"
```

## 🆘 常见问题

| 问题            | 解决方案                                              |
| --------------- | ----------------------------------------------------- |
| 部署失败        | 关闭游戏后重试                                        |
| PowerShell 阻止 | `Set-ExecutionPolicy RemoteSigned -Scope CurrentUser` |
| 找不到 Godot    | F5 附加时搜索 "Slay the Spire 2"                      |
| 没看到 DLL      | 确认路径在 .vscode/tasks.json 中正确                  |

## 📖 文档

- **快速指南**: [docs/QUICK_START.md](../docs/QUICK_START.md)
- **工作流**: [docs/VSCODE_WORKFLOW.md](../docs/VSCODE_WORKFLOW.md)
- **调试**: [docs/GODOT_DEBUG_GUIDE.md](../docs/GODOT_DEBUG_GUIDE.md)
- **部署**: [docs/WINDOWS_DEPLOYMENT.md](../docs/WINDOWS_DEPLOYMENT.md)

---

**🎉 现在按 Ctrl+Shift+B 开始！**
