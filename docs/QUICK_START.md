# VS Code 一键部署 — 快速上手

## 最快上手（3 步）

### 第一步：确认 sts2.dll 已就位

```bash
ls lib/sts2.dll   # 必须存在，否则先从游戏目录复制
```

如果文件不存在，见 [lib/README.md](../lib/README.md)。

### 第二步：在 VS Code 中打开项目

```bash
code /home/alphonse/projects/STS2-Dark-World
```

### 第三步：编译 + 自动部署

```
Ctrl+Shift+B
```

完成！自动流程：
- 编译代码 → `src/bin/Release/net9.0/STS2_Discard_Mod.dll`
- 部署 DLL 到 `{游戏目录}/mods/STS2_Discard_Mod/`
- 部署 `STS2_Discard_Mod.json` 到同目录

---

## 每日开发循环

```
1. 编辑 src/Cards/*.cs 或 Main.cs
2. Ctrl+Shift+B  →  等待 5-10 秒
3. 启动 / 重启 STS2
4. 创建游戏 → 选择储君（Regent）→ 检查卡牌
5. 查看日志中是否有 "Registered 4 discard-trigger cards"
```

---

## 快捷键速查

| 按键 | 操作 |
|------|------|
| **Ctrl+Shift+B** | 编译 Release + 自动部署 ⭐ |
| **Ctrl+`** | 打开/关闭终端 |
| **Ctrl+Shift+P** | 命令面板 |
| **F5** | 附加调试器（需要游戏在运行） |

---

## 部署失败怎么办

### "找不到游戏目录"

csproj 中的 `ModsPath` 未能解析。临时手动部署：

```bash
# Linux
cp src/bin/Release/net9.0/STS2_Discard_Mod.dll \
  ~/.local/share/Steam/steamapps/common/Slay\ the\ Spire\ 2/mods/STS2_Discard_Mod/
```

或编辑 `.vscode/tasks.json`，将 `ModsPath` 改为你的实际路径。

### "文件被占用"

游戏正在运行时无法替换 DLL。先关闭游戏再部署：

```
关闭 STS2 → Ctrl+Shift+B → 重启 STS2
```

### "构建失败 — sts2.dll not found"

```bash
ls lib/sts2.dll   # 检查文件
# 如果不存在，从游戏目录复制，见 lib/README.md
```

---

## 部署后的文件结构

```
{游戏目录}/mods/STS2_Discard_Mod/
├── STS2_Discard_Mod.dll     ← 编译的 mod
└── STS2_Discard_Mod.json    ← mod 清单 (id, has_dll, version…)
```

> `localization/` 目前仅用于 CI/CD 打包，游戏运行时暂不需要（等实际本地化实现后再部署）。

---

## 验证 mod 已加载

启动游戏后，在 STS2 日志中搜索 `STS2DiscardMod`：

```
[STS2DiscardMod][INFO] STS2 Discard-Trigger Mod loading...
[STS2DiscardMod][INFO] Registered 4 discard-trigger cards to RegentCardPool
[STS2DiscardMod][INFO] STS2 Discard-Trigger Mod loaded!
```

日志文件位置：
- Windows: `%APPDATA%\Roaming\STS2\logs\`
- Linux: `~/.local/share/STS2/logs/`

---

更多内容见 [DEV_GUIDE.md](DEV_GUIDE.md)。
