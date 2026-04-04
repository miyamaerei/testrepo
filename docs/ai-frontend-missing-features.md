# AI 任务：补充 E-Kanban 前端缺失功能

## 🎯 任务背景

E-Kanban 目前前端已经实现了核心功能（看板展示、项目管理、详情查看），但经过检查，还缺失几个重要业务模块需要补充开发。

## 📋 需求清单

### 1. 在卡片详情弹窗新增 **Spec 标签页**
**需求说明：**
- 在现有详情对话框的标签页中增加 "Spec" 标签
- 展示当前卡片的 Spec 内容（验收标准）
- 展示历史 Spec 评估记录（每次评估结果、是否通过）
- 支持编辑 Spec 内容（AI 生成可能不准确，需要人工调整）

**API 接口需要新增封装：**
- `getSpecByCardId(cardId)` - 获取卡片的 Spec
- `updateSpec(cardId, content)` - 更新 Spec
- `getSpecEvaluations(cardId)` - 获取评估历史

### 2. 在卡片详情弹窗新增 **执行历史标签页**
**需求说明：**
- 增加 "执行历史" 标签页
- 列表展示该卡片所有的 ExecutionRun（每次提交记录）
- 展示：执行轮次、执行者、提交时间、评估结果、证据内容
- 支持查看每次提交的详细证据

**API 接口需要新增封装：**
- `getExecutionRuns(cardId)` - 获取执行历史列表

### 3. 修复 API HTTP 方法不匹配问题
**当前问题：**
- 后端 `ProjectRepository/Update` 是 `PUT` 方法，前端用的 `POST`
- 后端 `ProjectRepository/Delete` 是 `DELETE` 方法，前端用的 `POST`

**修复要求：**
- 修改 `project.ts` 中 `updateProject` 使用 `PUT` 方法
- 修改 `project.ts` 中 `deleteProject` 使用 `DELETE` 方法

### 4. 新增**人工提交结果**入口（可选但推荐）
**需求说明：**
- 在卡片上增加"提交结果"按钮
- 弹出对话框，让用户输入提交证据内容
- 调用 Submit API 提交

**API 接口：**
- 需要封装 `submitResult(cardId, evidence)` 接口

### 5. 新增定时任务数据库初始化 SQL
**需求说明：**
- 代码层面已经有三个定时任务类：`SyncFromAzureBoardsJob`、`AiExecutionJob`、`AiTaskCheckJob`
- 需要在 `database/E-Kanban-NewTables.sql` 末尾追加 `Sys_QuartzOptions` 表的插入 SQL 语句
- 自动初始化这三个定时任务，用户部署时直接执行脚本即可

**插入数据内容：**

三个任务配置如下（假设 ID 自增长从 1001 开始）：

| 任务名称 | Cron 表达式 | 类全名 | 描述 |
|----------|-------------|--------|------|
| SyncFromAzureBoards | `0 0 */6 * * ?` | `EKanban.Jobs.SyncFromAzureBoardsJob` | 定时从 Azure Boards 同步工作项 |
| AiExecutionJob | `0 */15 * * * ?` | `EKanban.Jobs.AiExecutionJob` | 定时执行 Ready 状态的 AI 任务 |
| AiTaskCheckJob | `0 */15 * * * ?` | `EKanban.Jobs.AiTaskCheckJob` | 检查 AI 任务超时，自动重试 |

**SQL 要求：**
- 使用 `IF NOT EXISTS` 检查避免重复插入
- 遵循现有 `Sys_QuartzOptions` 表结构（使用框架已有字段）
- `Status = 0` 表示启用

## 🔍 现有代码结构参考

- 现有页面：`/src/frontend/vol.web/src/views/Kanban/Index.vue` - 详情对话框已有多标签页框架，直接加新标签页即可
- 现有组件：`/src/frontend/vol.web/src/components/Kanban/KanbanCard.vue` - 可新增按钮
- API 封装：`/src/frontend/vol.web/src/api/kanban.ts` - 在这里新增 API 封装
- 类型定义：`/src/frontend/vol.web/src/types/kanban.ts` - 在这里新增类型定义

## 📝 开发要求

1. **分支策略**：从最新 `master` 新开分支 `feature/frontend-add-missing-features`，在此分支上开发
2. **代码风格**：严格遵循现有代码风格（TypeScript + Composition API + Element Plus）
3. **兼容要求**：保持对现有代码的兼容，不破坏已有功能
4. **样式风格**：遵循现有样式风格，使用 scoped CSS

## ✅ 验收标准

- [ ] 卡片详情新增 Spec 标签页，能正常展示和编辑 Spec
- [ ] 卡片详情新增执行历史标签页，能正常展示所有执行记录
- [ ] HTTP 方法不匹配问题已修复
- [ ] 如果实现人工提交功能，需要能正常提交
- [ ] 已在数据库脚本中添加三个定时任务的初始化 INSERT SQL
- [ ] 编译通过，无 TypeScript 类型错误
- [ ] 所有新增功能测试可用

## ⚙️ 执行步骤（供 GitHub Copilot CLI / OpenClaw AI 执行参考）

1. Checkout 到新分支 `feature/frontend-add-missing-features`
2. 修复 HTTP 方法问题（最简单，先做）
   - 修改 `project.ts` 中 `updateProject` 和 `deleteProject` 的方法
3. 在 `kanban.ts` 新增缺失的 API 封装
4. 在 `types/kanban.ts` 新增 `Spec`、`SpecEvaluation`、`ExecutionRun` 类型定义
5. 修改 `Index.vue`，在详情对话框增加 Spec 标签页
6. 修改 `Index.vue`，在详情对话框增加执行历史标签页
7. （可选）如果时间允许，在 `KanbanCard.vue` 增加人工提交按钮和对话框
8. 编译验证前端：`cd src/frontend/vol.web && npm run build`
9. 提交代码，推送到远程

---

*任务创建时间：2026-04-04*
*需求提出：整理前端缺失功能补充开发*
