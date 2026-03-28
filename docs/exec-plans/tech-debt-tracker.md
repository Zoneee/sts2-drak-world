# 技术债跟踪器

## 格式
每条记录包含：
- 编号
- 标题
- 症状
- 根本原因
- 影响
- 建议修复方案
- 优先级
- 状态
- 相关文档 / 计划

## 原则
反复出现的摩擦应被系统化地捕获和减少。

## 记录列表

| 编号   | 标题                                   | 优先级 | 状态     | 说明                                                                                                                        |
| ------ | -------------------------------------- | ------ | -------- | --------------------------------------------------------------------------------------------------------------------------- |
| TD-001 | VoidSurgePower 空引用 nullable warning | 低     | 已知接受 | `VoidSurgePower.cs` 中 `Owner` 参数存在 CS8604 nullable warning；Owner 在 `AfterCardDiscarded` 中运行时始终非空，不影响功能 |
| TD-002 | 能力牌图标为占位符                     | 中     | 待处理   | `images/powers/` 下 6 张图标（3×64px、3×256px）均为纯色圆形占位符，需美术出图替换                                           |
| TD-003 | PCK 缺少 Godot CLI 自动导出            | 中     | 待处理   | 本地构建在未设置 `GODOT_CLI_COMMAND` 时跳过 PCK 导出，新图标资源需手动导出 PCK 才能在游戏中加载                             |

## 详细记录

### TD-001 VoidSurgePower 空引用 nullable warning
- **症状**：`dotnet build` 输出 CS8604 warning：`Possible null reference argument for parameter 'dealer' in CreatureCmd.Damage`
- **根本原因**：`Owner` 属性声明为可空类型，但 `AfterCardDiscarded` 触发时 Owner 始终有效
- **影响**：仅编译警告，无运行时影响
- **建议修复**：用 `Owner!` null 强制断言，或等 BaseLib 将 Owner 改为非空类型后跟进
- **优先级**：低

### TD-002 能力牌图标为占位符
- **症状**：游戏内状态栏显示纯色圆形图标而非设计图
- **根本原因**：无美术资源，由脚本生成占位
- **影响**：视觉体验差，但功能正常
- **建议修复**：美术产出 3 张设计图后替换 `src/STS2DiscardMod/images/powers/` 下 6 个文件，重新导出 PCK
- **优先级**：中

### TD-003 PCK 缺少 Godot CLI 自动导出
- **症状**：本地构建输出 `Skipping Godot PCK export because GodotCliCommand is not set`
- **根本原因**：CI 和本地开发环境未配置 `GODOT_CLI_COMMAND` 环境变量
- **影响**：图标资源变更后，必须手动运行 Godot 导出 PCK 并部署，否则游戏加载旧缓存
- **建议修复**：在 `scripts/common.sh` 文档中补充 Godot CLI 配置说明，或搭建 CI 步骤自动导出
- **优先级**：中
