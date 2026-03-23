# lib/ — 游戏 DLL 引用

此目录存放从 STS2 游戏安装目录复制的 DLL 文件，用于编译时引用。

**不提交到 git**（已在 `.gitignore` 中排除，因为是游戏专有文件）。

## 当前状态

- `sts2.dll` — 已就位 ✅

## 从 Steam 安装重新获取

如需在新机器上重建或更换 DLL：

### Windows（PowerShell）

```powershell
$gameData = "D:\G_games\steam\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64"
# 如果开发用 WSL：
Copy-Item "$gameData\sts2.dll" "\\wsl$\Ubuntu\home\alphonse\projects\STS2-Dark-World\lib\"
# 如果在 Windows 本地开发：
Copy-Item "$gameData\sts2.dll" "lib\"
```

### Linux

```bash
cp ~/.local/share/Steam/steamapps/common/Slay\ the\ Spire\ 2/data_sts2_linuxbsd_x86_64/sts2.dll \
   ~/projects/STS2-Dark-World/lib/
```

## csproj 中的引用逻辑

```xml
<!-- 优先使用 lib/ 目录（跨平台开发） -->
<Sts2DataDir Condition="Exists('$(LocalLibPath)/sts2.dll')">$(LocalLibPath)</Sts2DataDir>

<!-- 找不到 lib/sts2.dll 时，退回到 Steam 安装路径 -->
<Sts2DataDir>...Steam 路径...</Sts2DataDir>
```

只需将 `sts2.dll` 放入此目录，`dotnet build src/` 即可自动找到它。

## CI/CD

CI 环境通过 GitHub Actions secret `STS2_DLL_B64`（base64 编码的 DLL）获取：

```bash
# 生成 secret 的值（在本机运行）：
base64 -w0 lib/sts2.dll
# 将输出粘贴到 GitHub → Settings → Secrets → New secret → STS2_DLL_B64
```
