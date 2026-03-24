# 快速参考卡片

## 最常用命令

构建：

```bash
dotnet build src/STS2_Discard_Mod.csproj -p:Sts2DataDir=/absolute/path/to/lib
```

带 Godot 导出：

```bash
GODOT_CLI_COMMAND=/path/to/godot4 dotnet build src/STS2_Discard_Mod.csproj -p:Sts2DataDir=/absolute/path/to/lib
```

## 当前正确的 live 目录结构

```text
{游戏目录}/mods/STS2_Discard_Mod/
├── STS2DiscardMod.dll
├── STS2DiscardMod.pck
├── 0Harmony.dll
└── STS2_Discard_Mod.json
```

不要额外复制 `cards.json`。

## 日志关键前缀

- 模组入口：`STS2DiscardMod`
- 单卡打出：`[CardName] play`
- 单卡弃牌触发：`[CardName] discard-trigger:start`
- 弃牌命令：`[DiscardCmd]`
- ModelDb 检查：`ModelDb loaded discard-system card`

## 调试最短流程

1. `Build: Debug` 或命令行构建
2. 启动游戏
3. 观察 `BepInEx/LogOutput.log`
4. 搜索 `STS2DiscardMod`
5. 如果卡可见但没效果，优先看有没有 `play` / `discard-trigger` / `[DiscardCmd]`

## 出问题先查什么

1. `STS2DiscardMod.dll`、`STS2DiscardMod.pck`、`0Harmony.dll`、manifest 是否都在
2. `BaseLib` 是否已经单独安装到游戏目录
3. 是否忘了传 `Sts2DataDir` 或复制 `lib/sts2.dll`
4. 是否忘了设置 `GODOT_CLI_COMMAND` 导致卡图没打包
