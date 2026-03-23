# STS2 Mod Godot Debugging Guide (Visual Studio)

简体中文 | [English](#english)

## 🎯 快速开始

### 前置条件
- Visual Studio 2022 (Community/Professional/Enterprise)
- .NET 9.0 SDK
- STS2 游戏已安装: `D:\G_games\steam\steamapps\common\Slay the Spire 2`
- BaseLib-StS2 mod 已安装

---

## 📋 方法 1: 使用 Debug 配置编译 (推荐)

### 步骤 1: 修改项目配置

编辑 `src/STS2_Discard_Mod.csproj`:

```xml
<PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Debug|AnyCPU'">
  <DebugType>full</DebugType>
  <DebugSymbols>true</DebugSymbols>
  <Optimize>false</Optimize>
  <DefineConstants>DEBUG;TRACE</DefineConstants>
</PropertyGroup>
```

### 步骤 2: 编译 Debug 版本

```bash
dotnet build src/ --configuration Debug
```

输出文件: `src/bin/Debug/net9.0/STS2_Discard_Mod.dll` (带 PDB 符号)

### 步骤 3: 复制文件到游戏目录

```
D:\G_games\steam\steamapps\common\Slay the Spire 2\mods\
├── STS2_Discard_Mod.dll        ← Debug 版本 (包含符号)
├── STS2_Discard_Mod.pdb        ← 调试符号文件 (自动复制)
└── modInfo.json                 ← 已有
```

### 步骤 4: 在 VS 中配置附加调试

1. 打开 Visual Studio
2. **调试 → 附加到进程** (Debug → Attach to Process)
3. 查找进程: `Godot.exe` (Slay the Spire 2)
4. 点击 **附加**

---

## 🔍 方法 2: 在 Main.cs 中添加控制台输出

### 编辑器中的快速调试

编辑 `src/Main.cs`:

```csharp
public static void OnModLoad()
{
    // 调试检查点
    System.Diagnostics.Debugger.Break(); // 如果附加了调试器，此处停止
    
    Logger.Log("=== MOD LOADING START ===");
    Logger.Log($"Game thread ID: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
    Logger.Log($"Assembly location: {typeof(DiscardModMain).Assembly.Location}");
    
    // 注册卡牌
    RegisterCards();
    
    Logger.Log("=== MOD LOADING COMPLETE ===");
}
```

### 编译并部署

```bash
dotnet build src/ --configuration Release
# 复制 DLL 到 mods/ 目录
```

### 在游戏中查看日志

启动游戏后，在 Windows 命令行查看输出:

```bash
# 查看 STS2 日志 (如果游戏启用了控制台)
C:\Program Files\Git\bash.exe
# 或使用 DebugView (微软工具) 追踪 System.Console 输出
```

---

## 🛠️ 方法 3: 使用 Attach to Process (最简单)

### 前置条件
- 游戏已启动
- Visual Studio 已打开项目

### 执行步骤

1. **启动游戏**: 运行 STS2，选择储君角色创建新游戏

2. **附加调试器**:
   ```
   VS菜单 → 调试 → 附加到进程 → 找 Godot.exe → 附加
   ```

3. **设置断点**:
   - 在 `src/Main.cs` OnModLoad() 第一行设置断点
   - 或在卡牌的 `OnDiscard()` 方法设置断点

4. **触发代码执行**:
   - 在游戏中创建新局
   - 抽取你的卡牌
   - 触发弃牌效果

5. **代码应自动暂停在断点处**

---

## 📊 方法 4: 使用 Console.Out 重定向追踪

### 创建追踪文件

编辑 `src/Utils/Logger.cs`:

```csharp
public static class Logger
{
    private static StreamWriter? _logFile;
    
    public static void Initialize()
    {
        try
        {
            string logPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "sts2_discard_mod.log"
            );
            _logFile = new StreamWriter(logPath, append: true);
            _logFile.AutoFlush = true;
            Log($"=== LOG STARTED AT {DateTime.Now} ===");
        }
        catch { /* 忽略错误 */ }
    }
    
    public static void Log(string message)
    {
        string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        string formatted = $"[{timestamp}] [DiscardMod] {message}";
        Console.WriteLine(formatted);
        _logFile?.WriteLine(formatted);
    }
}
```

在 `Main.cs` 初始化:

```csharp
public static void OnModLoad()
{
    Logger.Initialize();
    Logger.Log("Mod initialization started");
    // ... 其他代码
}
```

### 查看日志文件

日志会写入: `C:\Users\<你的用户名>\Desktop\sts2_discard_mod.log`

实时监视日志:
```bash
# Windows PowerShell
Get-Content C:\Users\<用户名>\Desktop\sts2_discard_mod.log -Wait
```

---

## 🎮 实时调试工作流

### 完整调试循环 (推荐)

1. **编写代码**
   ```bash
   编辑 src/Cards/YourCard.cs
   ```

2. **编译**
   ```bash
   dotnet build src/ --configuration Debug
   ```

3. **部署**
   ```bash
   # 复制到 D:\G_games\steam\steamapps\common\Slay the Spire 2\mods\
   cp src/bin/Debug/net9.0/STS2_Discard_Mod.dll "D:\G_games\steam\steamapps\common\Slay the Spire 2\mods\"
   cp src/bin/Debug/net9.0/STS2_Discard_Mod.pdb "D:\G_games\steam\steamapps\common\Slay the Spire 2\mods\"
   ```

4. **启动游戏**
   ```bash
   # 启动 STS2
   ```

5. **附加调试器**
   ```
   VS → 调试 → 附加到进程 → Godot.exe
   ```

6. **触发效果**
   ```
   在游戏中抽卡 → 弃卡 → 观察断点
   ```

7. **查看变量**
   ```
   - 悬停查看变量
   - 即时窗口执行表达式
   - 调试堆栈跟踪
   ```

---

## 🐛 常见问题排查

| 问题                       | 原因                 | 解决方案                                  |
| -------------------------- | -------------------- | ----------------------------------------- |
| 没有看到 [DiscardMod] 日志 | modInfo.json 缺失    | 确保 modInfo.json 在 mods/ 目录中         |
| 卡牌没有出现               | CharacterID 不匹配   | 验证所有卡牌都有 `CharacterID = "Mystic"` |
| 附加调试器失败             | Godot.exe 进程名错误 | 查找 "Slay the Spire 2" 相关进程          |
| 断点不被触发               | 无符号或代码不匹配   | 重新编译 Debug 版本并复制 PDB             |
| DLL 加载失败               | 依赖项缺失           | 确认 BaseLib-StS2 已安装                  |

---

## 💡 调试技巧

### 查看调用堆栈
```
调试 → 窗口 → 调用堆栈 (Ctrl+Alt+C)
```

### 即时窗口执行代码
```
调试 → 窗口 → 即时 (Ctrl+Alt+I)
输入: card?.GetCardId()
输入: player?.GetClass().Name
```

### 条件断点
```
右键断点 → 筛选器
添加: card.ID == "DiscardMod_DarkFlameFragment"
只在满足条件时中断
```

### 性能分析
```
调试 → Performance Profiler
记录 CPU 使用 + 内存分配
```

---

## 📁 文件布局验证

```
D:\G_games\steam\steamapps\common\Slay the Spire 2\
├── Slay the Spire 2.exe        ← 启动器
├── mods/
│   ├── STS2_Discard_Mod.dll    ✅ (Debug 或 Release)
│   ├── STS2_Discard_Mod.pdb    ✅ (Debug 时必需)
│   ├── modInfo.json            ✅ (必需)
│   └── baselib/                ✅ (依赖)
└── ...
```

---

<h2 id="english">🌐 English Version</h2>

# STS2 Mod Godot Debugging Guide (Visual Studio)

## Quick Start

### Prerequisites
- Visual Studio 2022 (Community/Professional/Enterprise)
- .NET 9.0 SDK
- STS2 game installed: `D:\G_games\steam\steamapps\common\Slay the Spire 2`
- BaseLib-StS2 mod installed

## Method 1: Using Debug Configuration (Recommended)

### Step 1: Build Debug Version
```bash
dotnet build src/ --configuration Debug
```

Output: `src/bin/Debug/net9.0/STS2_Discard_Mod.dll` (with PDB symbols)

### Step 2: Copy Files to Game Mods Directory
```
D:\G_games\steam\steamapps\common\Slay the Spire 2\mods\
├── STS2_Discard_Mod.dll        ← Debug version
├── STS2_Discard_Mod.pdb        ← Debug symbols
└── modInfo.json
```

### Step 3: Attach Debugger in Visual Studio
1. Open Visual Studio
2. Go to Debug → Attach to Process
3. Find `Godot.exe` process
4. Click Attach

### Step 4: Set Breakpoints and Debug
- Open `src/Main.cs` or any card file
- Click left margin to set breakpoint
- Launch game and trigger mod code
- VS will pause at breakpoints

## Method 2: Add Debug.Break() Calls

```csharp
public static void OnModLoad()
{
    System.Diagnostics.Debugger.Break();  // VS will pause here
    Logger.Log("Mod loading...");
}
```

## Method 3: File-Based Logging (Easiest)

Modify `src/Utils/Logger.cs` to write to file:

```csharp
public static void Log(string message)
{
    string logPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
        "sts2_discard_mod.log"
    );
    File.AppendAllText(logPath, $"[DiscardMod] {message}\n");
}
```

View real-time: `C:\Users\<YourUsername>\Desktop\sts2_discard_mod.log`

## Complete Debugging Workflow

1. **Edit code** → 2. **Build** → 3. **Deploy** → 4. **Launch game** → 5. **Attach debugger** → 6. **Trigger effect** → 7. **Inspect variables**

---

**推荐使用方法 1 (Debug 配置 + 附加调试器) 获得最佳体验！**
