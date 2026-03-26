# 文档模板迁移计划

## 状态
已完成

## 背景
仓库已引入一套面向 Agent 优先开发的最佳实践模板，但 `docs/`、`README.md`、`ARCHITECTURE.md`、`DESIGN.md` 等入口文件仍保留大量模板占位内容。旧项目知识主要散落在 `docs-old/`、`README-old.md` 和若干根目录速查文件中，导致“新模板结构存在，但内容仍不可信”。

## 问题摘要
- `docs/` 目录结构已建立，但核心运行手册和索引仍是占位模板。
- 旧文档中的真实开发流程、调试路径、架构说明和卡牌设计尚未迁移到新结构。
- 顶层入口文档仍描述模板仓库，而不是当前 STS2 弃牌触发 Mod。
- Agent 按模板入口读取文档时，容易先读到空壳内容，降低仓库作为事实来源的可靠性。

## 范围
- 将旧文档内容迁移到新的 `docs/` 结构。
- 更新 `docs` 各层级索引，使入口链接指向真实项目文档。
- 更新顶层 `README.md`、`ARCHITECTURE.md`、`DESIGN.md`、`QUICK_REFERENCE.md`，使其与项目现状一致。

## 非范围
- 不改动代码实现与运行逻辑。
- 不重写 `standards/`、`references/` 中仍可继续沿用模板的通用规范。
- 不处理所有历史文档的逐行归档，仅迁移当前高频入口和核心知识。

## 受影响区域
- `docs/runbooks/`
- `docs/architecture/`
- `docs/design-docs/`
- `docs/product-specs/`
- `docs/exec-plans/active/`
- 根目录入口文档

## 约束
- 已采用 Skills：
  - `skills/plan-before-code.md`：本任务跨多个文档与目录，需要先给出迁移计划。
  - `skills/repo-as-source-of-truth.md`：必须以仓库文档为准，把持久知识收敛进新结构。
  - `skills/evidence-driven-delivery.md`：完成时需要留下可核对的改动与验证证据。
  - `skills/small-safe-prs.md`：只做文档迁移与入口修正，不混入代码重构。
- 文档内容必须与当前仓库真实脚本、路径、任务名和产物一致。
- 保留模板的分层结构，但替换为项目特定内容。

## 验收标准
- `docs/` 中存在可直接使用的 quick start、本地开发、调试、部署、VS Code 工作流文档。
- `architecture/`、`design-docs/`、`product-specs/` 至少各有一个项目特定核心文档，并被对应索引引用。
- `README.md` 与顶层速查文档不再描述模板仓库，而是当前项目。
- 迁移后的文档不引用已废弃的旧文档作为主入口。

## 验证计划
- 检查新增与更新文档是否覆盖旧文档中的核心信息。
- 检查索引文件中的链接是否指向仓库内现有文件。
- 使用 `git diff` 审核改动范围，确认仅包含文档迁移与入口更新。

## 验证结果
- 已通过编辑器诊断检查本轮新增与更新的文档文件，未发现错误。
- 已确认活跃入口文件不再引用 `docs/QUICK_START.md`、`docs/DEV_GUIDE.md`、`docs/DEBUGGING.md`、`docs/MOD_REGISTRY.md` 等旧路径；剩余旧路径仅保留在 `README-old.md` 归档文件中。
- 已检查本轮改动文件集合，范围集中在 `README.md`、顶层速查文档、`docs/runbooks/`、`docs/architecture/`、`docs/design-docs/`、`docs/product-specs/` 和当前执行计划。

## 风险
- 旧文档中少量信息可能已过期，需要以当前仓库脚本和配置为准做裁剪。
- 仍有部分模板文档未在本轮迁移中项目化，后续若被高频引用，仍需继续收口。

## 决策日志
- 2026-03-24：先迁移高频入口文档，再保留次级模板文档作为后续增量完善对象。

## 进展日志
- 2026-03-24：完成旧文档与新模板目录的映射分析，准备落第一轮核心文档。
- 2026-03-24：完成核心文档迁移、索引更新和顶层入口收口，并修正 `SETUP_COMPLETE.md` 的旧路径引用。

## 后续事项
- 如后续继续项目化 `standards/` 和 `references/`，应以当前代码和构建流程为依据逐步替换模板内容。