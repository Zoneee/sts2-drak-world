# STS2 弃触发 Mod (Discard-Trigger)

**杀戮尖塔2 — "弃触发"卡牌系统 mod**

向 Slay the Spire 2 添加"弃触发"卡牌机制的 mod：将牌弃置时触发强力效果。4 张卡牌注册到 `RegentCardPool`（储君角色池）。

## 快速开始

### 环境要求

- **.NET SDK 9.0+** — [下载](https://dotnet.microsoft.com/download)，验证：`dotnet --version`
- **Slay the Spire 2**（Steam 安装）
- `lib/sts2.dll` — 从游戏目录手动复制（见下方）

### 首次设置

```bash
git clone git@github.com:Zoneee/sts2-drak-world.git
cd sts2-drak-world

# 从游戏目录复制 sts2.dll 到 lib/
# Windows (PowerShell):
# Copy-Item "D:\G_games\steam\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.dll" lib\

# Linux:
# cp ~/.local/share/Steam/steamapps/common/Slay\ the\ Spire\ 2/data_sts2_linuxbsd_x86_64/sts2.dll lib/

dotnet build src/
# → Build succeeded. 0 Error(s)
```

### 编译与部署

```bash
# 编译 Release 版本
dotnet build src/ --configuration Release

# 输出: src/bin/Release/net9.0/STS2_Discard_Mod.dll
```

**VS Code 一键编译 + 自动部署到游戏目录（需要游戏安装）：**

```
Ctrl+Shift+B
```

### 安装到游戏

将以下文件复制到 STS2 mod 目录下的 `STS2_Discard_Mod/` 子文件夹：

| 文件 | 说明 |
|------|------|
| `STS2_Discard_Mod.dll` | 编译好的 mod DLL |
| `STS2_Discard_Mod.json` | mod 清单 |
| `localization/` | 本地化 JSON 文件 |

Mod 目录路径：
- **Windows**: `D:\G_games\steam\steamapps\common\Slay the Spire 2\mods\STS2_Discard_Mod\`
- **Linux**: `~/.local/share/Steam/steamapps/common/Slay the Spire 2/mods/STS2_Discard_Mod/`
- **macOS**: `~/Library/Application Support/Steam/steamapps/common/Slay the Spire 2/mods/STS2_Discard_Mod/`

> 编译后 csproj 内置的 MSBuild target 会自动部署（条件：游戏目录存在）。

---

## 核心机制："弃触发"

这 4 张卡牌注册到 `RegentCardPool`，并在 `OnPlay()` 中实现效果（弃触发逻辑通过 Harmony 补丁监听弃牌事件实现，待完善）：

| 卡牌 | 类型 | 稀有度 | 费用 | 目标 |
|------|------|--------|------|------|
| **暗焰残页** DarkFlameFragment | Skill | Common | 1 | AnyEnemy |
| **迅影斩** SwiftCut | Attack | Common | 0 | AnyEnemy |
| **毒素记录** ToxinRecord | Skill | Uncommon | 1 | Self |
| **破碎回响** ShatteredEcho | Skill | Rare | 2 | Self |

---

## 项目结构

```
STS2-Dark-World/
├── src/
│   ├── Main.cs                        # 入口点 [ModInitializer]，注册卡牌
│   ├── Cards/                         # 每张卡牌一个文件，继承 CardModel
│   ├── Utils/Logger.cs                # MegaCrit Logger 封装
│   ├── localization/eng/cards.json    # 卡牌标题/描述本地化
│   └── STS2_Discard_Mod.csproj        # 项目文件
├── lib/
│   └── sts2.dll                       # 游戏 DLL（不提交到 git）
├── STS2_Discard_Mod.json              # mod 清单
├── .github/workflows/build.yml        # CI/CD
└── docs/                              # 详细文档
```

---

## 开发工作流

```
编辑代码 → Ctrl+Shift+B（编译+部署）→ 启动游戏测试
```

详见 [docs/DEV_GUIDE.md](docs/DEV_GUIDE.md)。

### 日志

所有 mod 日志通过 `DiscardModMain.Logger`（MegaCrit Logger），游戏内控制台可见：

```
[STS2DiscardMod][INFO] STS2 Discard-Trigger Mod loading...
[STS2DiscardMod][INFO] Registered 4 discard-trigger cards to RegentCardPool
[STS2DiscardMod][INFO] STS2 Discard-Trigger Mod loaded!
```

---

## CI/CD

GitHub Actions 工作流（`.github/workflows/build.yml`）：

- **触发**：push 到 `master`/`main`，或推送 `v*` tag
- **构建**：需要将 `sts2.dll` 以 base64 存入 GitHub secret `STS2_DLL_B64`（见 `build.yml` 注释）
- **发布**：推送 `v*` tag 时自动创建 GitHub Release，上传 `STS2_Discard_Mod_vX.X.X.zip`

---

## 文档

| 文档 | 内容 |
|------|------|
| [docs/DEV_GUIDE.md](docs/DEV_GUIDE.md) | 添加卡牌、构建、部署完整指南 |
| [docs/QUICK_START.md](docs/QUICK_START.md) | VS Code 一键部署快速指南 |
| [docs/DEBUGGING.md](docs/DEBUGGING.md) | 调试与日志分析 |
| [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) | 项目架构 |
| [docs/DESIGN.md](docs/DESIGN.md) | 卡牌设计思路 |
| [design/draft.md](design/draft.md) | 原始设计草稿 |

---

## 项目状态

**v0.1.0-alpha（开发中）**

- [x] 正确的项目结构（参考 StS2-Quick-Restart）
- [x] 使用真实 STS2 API（`CardModel`、`ModHelper`）
- [x] 4 张卡牌注册到 `RegentCardPool`
- [x] 本地化文件（满足 STS001 分析器）
- [x] CI/CD（需配置 `STS2_DLL_B64` secret）
- [ ] 弃触发逻辑（Harmony 补丁监听弃牌事件）
- [ ] 实际战斗效果（伤害/毒素/摸牌）
- [ ] 游戏内测试

---

**Made with ❤️ by alphonse-bot**
