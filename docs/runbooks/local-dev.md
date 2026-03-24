# 本地开发环境运行手册

状态：有效
负责人：工程团队
最后评审：2026-03-24

## 前置条件
- `.NET SDK 9.0+`
- 已安装《Slay the Spire 2》
- 可用的 `sts2.dll`
- 如需导出卡图资源，已安装 Godot CLI 并设置 `GODOT_CLI_COMMAND`

## 依赖准备
### 获取 `sts2.dll`
推荐两种方式任选其一：
- 方式 A：复制到当前仓库的 `lib/sts2.dll`
- 方式 B：构建时直接传 `-p:Sts2DataDir=/absolute/path/to/lib`

对于 worktree 开发，方式 B 往往更方便，因为不必在多个目录间重复复制依赖。

## 常用构建方式
### Debug 构建

```bash
dotnet build src/STS2_Discard_Mod.csproj --configuration Debug
```

### Release 构建

```bash
dotnet build src/STS2_Discard_Mod.csproj --configuration Release
```

### 显式指定 `Sts2DataDir`

```bash
dotnet build src/STS2_Discard_Mod.csproj \
	--configuration Debug \
	-p:Sts2DataDir=/absolute/path/to/lib
```

### 导出 `.pck`

```bash
export GODOT_CLI_COMMAND='/path/to/godot4'
dotnet build src/STS2_Discard_Mod.csproj --configuration Release
```

## VS Code 入口
- `Ctrl+Shift+B`：默认运行 `Build: Release`
- `Build: Debug`：生成调试构建
- `Build + Deploy: Debug`：调试构建并部署
- `Build + Deploy: Release`：发布构建并部署

## 当前代码结构
### 入口
`src/Main.cs` 只做两件事：
- 输出初始化日志
- 通过 `Harmony.PatchAll()` 应用补丁

### 卡牌基类
`src/Cards/DiscardModCard.cs` 是当前卡组的核心基类，集中封装：
- 反射发现所有弃牌体系卡牌类型
- 统一卡图路径
- 统一的打出与弃牌触发日志
- 抽牌、弃牌、群攻、随机攻击、上状态、格挡等常用 helper

### 补丁目录
`src/Patches/` 负责：
- 运行时本地化兜底
- 卡库可见性
- `ModelDb` 诊断
- 弃牌命令链路诊断
- Debug 构建下的卡池、起始牌组和商店过滤

## 添加新卡牌的最小步骤
1. 在 `src/Cards/` 下新建继承 `DiscardModCard` 的类。
2. 添加 `[Pool(typeof(RegentCardPool))]`。
3. 实现 `OnPlay()`。
4. 若有弃牌收益，实现 `OnSelfDiscarded()`。
5. 补充本地化输入。
6. 如有需要，补充运行时文本兜底。
7. 构建并在游戏内验证。

## 本地化规则
当前仓库使用扁平键，例如：

```json
{
	"DISCARDMOD-YOUR_CARD.title": "Your Card",
	"DISCARDMOD-YOUR_CARD.description": "Description."
}
```

规则：类名转大写下划线，并带 `DISCARDMOD-` 前缀。

## 常见问题
### 构建成功但卡图缺失
通常是因为没有设置 `GODOT_CLI_COMMAND`，导致 `.pck` 没有导出。

### 模组目录里多了本地化 JSON
不要把 `src/localization/eng/cards.json` 直接复制到 live 模组目录。

### build 成功但游戏里看不到卡
优先检查：
- `BaseLib` 是否安装
- `ModelDb` 诊断日志是否显示进库与进池成功
- 当前角色是否已经解锁 `Regent`
