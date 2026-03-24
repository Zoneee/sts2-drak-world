# AGENTS.md

## 定位
本仓库面向 Agent 优先的软件开发方式。  
人类负责舵，Agent 负责执行。  
目标是可靠、可验证、小范围的持续交付。

## 工作规则
1. 不以对话历史作为唯一信息来源。
2. 以仓库文档作为首要知识来源。
3. 优先做小步、可逆、可测试的改动。
4. 尽可能用机械手段强制执行约束。
5. 没有验证证据，不得标记工作已完成。
6. 若任务反复失败，应改善系统，而非只修改实现。
7. 改动代码时，同步更新受影响的文档、测试与计划。

## 阅读入口
- 文档总索引：`docs/index.md`
- 架构概览：`docs/architecture/index.md`
- 开发规范：`docs/standards/index.md`
- 运行手册：`docs/runbooks/index.md`
- Skills 索引：`skills/index.md`
- 当前执行计划：`docs/exec-plans/active/`
- 质量评分：`QUALITY_SCORE.md`

## Skills 使用与加载规则
1. Agent 在开始任务时，必须先读取根目录 `AGENTS.md`，并将其中列出的规则、阅读入口、必读 Skills、按场景选读 Skills 与完成定义作为默认执行上下文。
2. 若任务与某个场景明显相关，Agent 必须主动读取对应的 Skills 文件；不能把是否读取 Skills 的责任完全留给用户在对话中额外提醒。
3. `AGENTS.md` 中列出的“必读 Skills”默认应被视为任务启动前的基础上下文；“按场景选读 Skills”应根据任务类型自动补充读取。
4. 用户不必通过 `/skills` 命令手动指定 Skills，除非用户希望强制指定某个 Skills 文件、覆盖默认选择，或显式测试某个 Skills 是否被采用。
5. 若用户显式指定某个 Skills 文件，Agent 必须优先采用该 Skills，并在不冲突时继续遵守 `AGENTS.md` 中的其余通用约束。
6. 若用户请求与当前自动选择的 Skills 不一致，Agent 必须明确说明最终采用了哪些 Skills，以及哪些是因为用户显式指定而被优先采用。
7. 在输出计划、开始实现或给出结论前，Agent 应先自检本次任务所需读取的 Skills 是否已经覆盖；不得在未读取相关 Skills 的情况下直接开始高风险或高歧义工作。

## 核心工作流
1. 仔细阅读任务。
2. 使用 `templates/plan-template.md` 创建或更新计划。
3. 阅读相关产品规格、架构文档、开发规范。
4. 读取 `AGENTS.md` 规定的必读 Skills，并按任务场景补充读取相关 Skills。
5. 在输出计划前，先列出本次已采用的 Skills 清单及其适用原因。
6. 实现最小可用改动。
7. 执行必要的检查。
8. 收集验证证据。
9. 使用清晰的摘要、风险说明和证据开启或更新 PR。
10. 若被阻塞，先改善文档、工具或约束。

## 必读 Skills
- `skills/plan-before-code.md`
- `skills/repo-as-source-of-truth.md`
- `skills/evidence-driven-delivery.md`
- `skills/small-safe-prs.md`

## 按场景选读 Skills
- API / 解析 / 外部输入：`skills/boundary-validation.md`
- 任务反复失败：`skills/fix-the-system-not-just-the-ticket.md`
- 调试类生产问题：`skills/observability-first-debugging.md`
- 大范围重构：`skills/refactor-with-constraints.md`
- 质量漂移：`skills/entropy-cleanup.md`

## 完成定义
满足以下全部条件，工作才算完成：
- 任务范围已满足
- 必要检查通过
- 验证证据已记录
- 受影响文档已更新
- 风险与后续事项已备注

## 禁止行为
- 大范围混合目的的 PR
- 未在仓库文档中记录的隐性假设
- 未经验证的外部输入直接进入业务逻辑
- 沉默的架构漂移
- 无测试、无检查、无证据的"完成"