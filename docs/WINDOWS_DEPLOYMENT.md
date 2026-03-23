# Windows 部署指南 - STS2 弃触发 Mod

## 🎯 你的游戏路径
```
D:\G_games\steam\steamapps\common\Slay the Spire 2
```

## 📦 部署步骤

### 第 1 步: 编译 Mod

```bash
cd C:\path\to\your\project
dotnet build src/ --configuration Release
```

生成的文件:
```
src\bin\Release\net9.0\STS2_Discard_Mod.dll
```

### 第 2 步: 创建 Mods 目录 (如果不存在)

```cmd
mkdir "D:\G_games\steam\steamapps\common\Slay the Spire 2\mods"
```

### 第 3 步: 复制文件到游戏目录

**选项 A: 使用 Windows 文件管理器**

1. 打开文件管理器
2. 导航到: `D:\G_games\steam\steamapps\common\Slay the Spire 2\mods\`
3. 从 `src\bin\Release\net9.0\` 复制以下文件到 mods 目录:
   - `STS2_Discard_Mod.dll`
   - `modInfo.json` (从项目根目录)

**选项 B: 使用 PowerShell 脚本**

```powershell
# 保存为 deploy.ps1
$sourceDll = "C:\path\to\project\src\bin\Release\net9.0\STS2_Discard_Mod.dll"
$sourceJson = "C:\path\to\project\modInfo.json"
$destDir = "D:\G_games\steam\steamapps\common\Slay the Spire 2\mods"

# 创建目录 (如果不存在)
New-Item -ItemType Directory -Force -Path $destDir | Out-Null

# 复制文件
Copy-Item $sourceDll -Destination $destDir -Force
Copy-Item $sourceJson -Destination $destDir -Force

Write-Host "✅ Deployment complete!"
Write-Host "Files copied to: $destDir"
```

运行脚本:
```cmd
powershell -ExecutionPolicy Bypass -File deploy.ps1
```

**选项 C: 使用命令行 (最快)**

```cmd
REM 假设项目在 C:\Projects\STS2-Dark-World
copy "C:\Projects\STS2-Dark-World\src\bin\Release\net9.0\STS2_Discard_Mod.dll" "D:\G_games\steam\steamapps\common\Slay the Spire 2\mods\"
copy "C:\Projects\STS2-Dark-World\modInfo.json" "D:\G_games\steam\steamapps\common\Slay the Spire 2\mods\"
```

### 第 4 步: 验证部署

检查 `D:\G_games\steam\steamapps\common\Slay the Spire 2\mods\` 目录:

```
mods/
├── STS2_Discard_Mod.dll    ✅ 必须存在
├── modInfo.json            ✅ 必须存在
└── baselib/                ✅ 应该已存在
```

### 第 5 步: 启动游戏测试

1. 启动 Steam
2. 运行 **Slay the Spire 2**
3. 点击 **创建新游戏**
4. 选择 **储君 (Mystic)** 角色
5. 检查卡牌池是否包含:
   - 暗焰残页 (Dark Flame Fragment)
   - 迅切 (Swift Cut)
   - 毒记 (Toxin Record)
   - 碎念回响 (Shattered Echo)

### 第 6 步: 检查日志

打开 Steam 日志查看 mod 是否加载:

```
路径: Documents\Slay the Spire 2\logs\
文件: 查找最新的日志文件 (按修改时间排序)

搜索: [DiscardMod]
```

如果看到 `[DiscardMod]` 前缀的日志，说明 mod 已成功加载！

---

## 🔧 快速更新工作流

每次修改代码后:

```bash
# 1. 编译
dotnet build src/ --configuration Release

# 2. 部署 (使用 PowerShell 脚本 或 命令行)
copy "src\bin\Release\net9.0\STS2_Discard_Mod.dll" "D:\G_games\steam\steamapps\common\Slay the Spire 2\mods\"

# 3. 重启游戏
# (完全退出 STS2，然后重新启动)

# 4. 测试
# 创建新游戏，选择储君，检查卡牌
```

---

## 🐛 故障排除

| 症状                    | 可能原因           | 解决方案                        |
| ----------------------- | ------------------ | ------------------------------- |
| 卡牌未出现              | DLL 未部署或已卸载 | 重新复制 DLL，重启游戏          |
| 游戏崩溃                | DLL 版本不兼容     | 确认 .net 9.0 版本正确          |
| 日志中没有 [DiscardMod] | modInfo.json 缺失  | 复制 modInfo.json 到 mods/ 目录 |
| 卡牌只显示符号          | 字符编码问题       | 重新编译，确保 UTF-8 编码       |
| "mod 已禁用" 提示       | BaseLib 缺失       | 确认 baselib mod 已安装         |

---

## 📍 常用路径速查

| 用途       | 路径                                                      |
| ---------- | --------------------------------------------------------- |
| 游戏目录   | `D:\G_games\steam\steamapps\common\Slay the Spire 2`      |
| Mods 目录  | `D:\G_games\steam\steamapps\common\Slay the Spire 2\mods` |
| 项目源代码 | `C:\Projects\STS2-Dark-World` (或你的路径)                |
| 编译输出   | `src\bin\Release\net9.0\STS2_Discard_Mod.dll`             |
| 项目配置   | `modInfo.json` (项目根目录)                               |
| 游戏日志   | `Documents\Slay the Spire 2\logs\`                        |

---

## ⚡ 自动部署脚本 (高级)

创建 `auto-deploy.ps1`:

```powershell
param(
    [string]$Config = "Release"
)

$projectRoot = $PSScriptRoot
$gameModsDir = "D:\G_games\steam\steamapps\common\Slay the Spire 2\mods"

# 1. 编译
Write-Host "🔨 编译中..." -ForegroundColor Cyan
dotnet build "$projectRoot\src" --configuration $Config
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ 编译失败!" -ForegroundColor Red
    exit 1
}

# 2. 复制 DLL
$dllSource = "$projectRoot\src\bin\$Config\net9.0\STS2_Discard_Mod.dll"
Copy-Item $dllSource -Destination $gameModsDir -Force -Verbose

# 3. 复制 modInfo.json
$jsonSource = "$projectRoot\modInfo.json"
Copy-Item $jsonSource -Destination $gameModsDir -Force -Verbose

Write-Host "✅ 部署完成!" -ForegroundColor Green
Write-Host "📁 目标目录: $gameModsDir" -ForegroundColor Green
```

使用:
```powershell
powershell -ExecutionPolicy Bypass -File auto-deploy.ps1
```

---

**✨ 现在你可以开始测试你的 mod 了！**
