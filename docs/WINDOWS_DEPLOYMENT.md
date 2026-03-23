# Windows 部署指南 - STS2 弃触发 Mod

## 游戏路径

根据你的 Steam 安装位置，请修改以下路径（示例使用 `D:\G_games`）：

```
D:\G_games\steam\steamapps\common\Slay the Spire 2
```

## 部署方式一：自动部署（推荐）

`src/STS2_Discard_Mod.csproj` 包含 `CopyToModsFolderOnBuild` MSBuild target，
**编译成功后自动**将 DLL + JSON 复制到游戏目录。

**前提**：`mods/` 目录必须存在（游戏目录下）

如果自动部署路径不正确，在编译时覆盖 Steam 库路径：

```powershell
dotnet build src/ --configuration Release `
  -p:SteamLibraryPath="D:\G_games\steam\steamapps"
```

部署成功后，游戏目录结构：

```
{game}\mods\
└── STS2_Discard_Mod\
    ├── STS2_Discard_Mod.dll
    └── STS2_Discard_Mod.json
```

**注意**：mod 必须在 `STS2_Discard_Mod\` 子目录中，不能直接放在 `mods\` 根目录。

---

## 部署方式二：手动部署

### 第 1 步：编译

```powershell
cd C:\path\to\STS2-Dark-World
dotnet build src/ --configuration Release
```

输出文件：
```
src\bin\Release\net9.0\STS2_Discard_Mod.dll
```

### 第 2 步：创建 mod 子目录

```cmd
mkdir "D:\G_games\steam\steamapps\common\Slay the Spire 2\mods\STS2_Discard_Mod"
```

### 第 3 步：复制文件

```cmd
copy "src\bin\Release\net9.0\STS2_Discard_Mod.dll" ^
  "D:\G_games\steam\steamapps\common\Slay the Spire 2\mods\STS2_Discard_Mod\"

copy "STS2_Discard_Mod.json" ^
  "D:\G_games\steam\steamapps\common\Slay the Spire 2\mods\STS2_Discard_Mod\"
```

### 第 4 步：验证文件结构

```
mods\
└── STS2_Discard_Mod\
    ├── STS2_Discard_Mod.dll    ✅ 必须存在
    └── STS2_Discard_Mod.json   ✅ 必须存在
```

---

## 测试 Mod

1. 启动 Steam → 运行 **Slay the Spire 2**
2. 创建新游戏，选择**储君**角色
3. 检查卡牌池是否包含：
   - 迅影斩 (Swift Cut)
   - 暗焰残页 (Dark Flame Fragment)
   - 毒素记录 (Toxin Record)
   - 碎念回响 (Shattered Echo)

---

## 检查日志

BepInEx 日志：
```
D:\G_games\steam\steamapps\common\Slay the Spire 2\BepInEx\LogOutput.log
```

查找 mod 加载确认：
```powershell
Select-String '\[STS2DiscardMod\]' `
  "D:\G_games\steam\steamapps\common\Slay the Spire 2\BepInEx\LogOutput.log"
```

成功加载时应看到：
```
[Info   : STS2DiscardMod] loading...
[Info   : STS2DiscardMod] Registered 4 discard-trigger cards to RegentCardPool
[Info   : STS2DiscardMod] loaded!
```

---

## 快速更新脚本

保存为 `deploy.ps1` 在项目根目录：

```powershell
param([string]$Config = "Release")

$gameDir = "D:\G_games\steam\steamapps\common\Slay the Spire 2"
$modDir  = "$gameDir\mods\STS2_Discard_Mod"

Write-Host "Building $Config..." -ForegroundColor Cyan
dotnet build src/ --configuration $Config
if ($LASTEXITCODE -ne 0) { exit 1 }

New-Item -ItemType Directory -Force -Path $modDir | Out-Null
Copy-Item "src\bin\$Config\net9.0\STS2_Discard_Mod.dll" -Destination $modDir -Force
Copy-Item "STS2_Discard_Mod.json" -Destination $modDir -Force

Write-Host "Deployed to $modDir" -ForegroundColor Green
```

运行：
```powershell
powershell -ExecutionPolicy Bypass -File deploy.ps1
```

---

## 故障排除

| 症状 | 可能原因 | 解决方案 |
| --- | --- | --- |
| 卡牌未出现 | DLL 未部署 | 确认 `mods/STS2_Discard_Mod/` 子目录存在且有两个文件 |
| 无 `[STS2DiscardMod]` 日志 | JSON 缺失或格式错误 | 确认 `STS2_Discard_Mod.json` 在子目录中 |
| 游戏崩溃 | DLL 不兼容 | 确认编译目标 `net9.0`，用最新 `sts2.dll` 重新编译 |
| mod 未启用 | 子目录名称错误 | 子目录名必须与 JSON 中 `id` 字段完全一致：`STS2_Discard_Mod` |

---

## 常用路径速查

| 用途 | 路径 |
| --- | --- |
| 游戏目录 | `D:\G_games\steam\steamapps\common\Slay the Spire 2` |
| Mods 子目录 | `...\mods\STS2_Discard_Mod\` |
| BepInEx 日志 | `...\BepInEx\LogOutput.log` |
| 编译输出 | `src\bin\Release\net9.0\STS2_Discard_Mod.dll` |
| Mod 清单 | `STS2_Discard_Mod.json`（项目根目录） |
