# 平台部署补充（Windows）

这份文档只保留 Windows 平台特有的信息。构建、项目结构与通用部署约定请以 [DEV_GUIDE.md](DEV_GUIDE.md) 为准。

## 典型游戏路径

```text
D:\G_games\steam\steamapps\common\Slay the Spire 2
```

## 推荐方式：自动部署

直接执行：

```powershell
dotnet build src/ --configuration Release
```

如果 `mods/` 目录存在，项目会自动部署到：

```text
{game}\mods\STS2_Discard_Mod\
├── STS2DiscardMod.dll
└── STS2_Discard_Mod.json
```

如果你的 Steam 库不在默认位置，可以在构建时覆盖：

```powershell
dotnet build src/ --configuration Release `
  -p:SteamLibraryPath="D:\G_games\steam\steamapps"
```

## 备用方式：手动复制

只有自动部署失效时再使用。

```cmd
mkdir "D:\G_games\steam\steamapps\common\Slay the Spire 2\mods\STS2_Discard_Mod"

copy "src\bin\Release\net9.0\STS2DiscardMod.dll" ^
  "D:\G_games\steam\steamapps\common\Slay the Spire 2\mods\STS2_Discard_Mod\"

copy "STS2_Discard_Mod.json" ^
  "D:\G_games\steam\steamapps\common\Slay the Spire 2\mods\STS2_Discard_Mod\"
```

不要复制 `src\localization\eng\cards.json` 到 live 模组目录。

## 验证部署成功

### 检查文件

```powershell
Get-ChildItem "D:\G_games\steam\steamapps\common\Slay the Spire 2\mods\STS2_Discard_Mod"
```

### 检查日志

```powershell
Select-String "STS2DiscardMod" `
  "D:\G_games\steam\steamapps\common\Slay the Spire 2\BepInEx\LogOutput.log"
```

## 常见问题

### DLL 无法覆盖

原因通常是游戏仍在运行。关闭游戏后重新构建。

### 模组目录里有多余 JSON

如果目录下出现 `localization/eng/cards.json`，先删除再启动游戏，否则可能触发 manifest 读取错误。

### 没有加载日志

先确认以下两个文件都在：

```text
mods/STS2_Discard_Mod/STS2DiscardMod.dll
mods/STS2_Discard_Mod/STS2_Discard_Mod.json
```
