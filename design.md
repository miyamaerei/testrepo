# 执行型 Kanban 协调系统设计文档

## 1. 背景与目标

本系统旨在构建一个**以可视化 Kanban 为核心的人机协同执行平台**，用于承接 Azure Boards 中的工作项，并将其转化为可执行、可验证、可回溯的执行过程。

核心设计目标：
- Azure Boards 作为**业务事实源**
- Kanban 作为**执行中枢与唯一操作入口**
- 统一 Submit API 作为**执行触发点**
- 支持人 / AI / 系统执行一致化
- 使用 Spec 判定任务是否真正完成

---

## 2. 总体执行闭环

Azure Boards → 执行型 Kanban → Submit API → 执行 / 证据 → Spec 校验 → 状态与结果同步回 Azure Boards

---

## 3. 系统架构

### 3.1 架构分层

#### 外部层
- **Azure Boards**：业务任务管理与展示

#### 执行协调层（本系统）
- Integration API：Boards 拉取与回写
- Execution Core：任务协调与状态推进
- Submit API：统一执行入口
- Spec Engine：行为验证

#### 表现层
- **Visual Kanban UI**：执行可视化与操作界面

#### 执行层
- Human Executor
- AI Executor
- System Worker（Hangfire）

---

### 3.2 架构原则
- Kanban 是第一等公民
- 所有执行通过 Submit API
- 状态变化必须可追溯
- Board 与 Kanban 最终一致

---

## 4. 领域模型

### 4.1 BoardWorkItem（外部引用）
- BoardId
- Title
- ExternalState

职责：
- 提供业务上下文
- 显示同步结果

---

### 4.2 ExecutionCard（Kanban 卡片）
- ExecutionCardId
- BoardId
- Status
- LastUpdated

职责：
- 表示一个可执行对象
- 驱动 Kanban 展示
- 作为 Submit 的入口

---

### 4.3 ExecutionTask（执行定义）
- ExecutionTaskId
- CardId
- ExecutorType

职责：
- 定义执行方式
- 校验执行者类型

---

### 4.4 ExecutionRun（执行事实）
- ExecutionRunId
- TaskId
- SubmittedBy
- SubmittedAt
- Evidence

职责：
- 记录一次执行提交
- 执行审计与回溯

---

### 4.5 Spec
- SpecId
- Definition

职责：
- 定义完成标准
- 抽象行为契约

---

### 4.6 SpecEvaluation
- ExecutionRunId
- Result
- Message

职责：
- 判定是否完成
- 驱动状态流转

---

## 5. 执行状态机（ExecutionCard）

New → Ready → InProgress → Submitted → Completed / Failed

约束：
- Submit 不等于 Completed
- 仅 Spec Pass 可完成

---

## 6. Submit API（核心）

### 接口定义（简化）

POST /api/execution-cards/{id}/submit

请求包含：
- 执行者身份（Human / AI）
- Evidence / Result

效果：
- 创建 ExecutionRun
- 触发 Spec 校验
- 推进状态

---

## 7. Azure Boards 同步策略

- Task 创建 → 生成 Kanban Card
- 执行完成 → 回写 Comment + 状态
- Spec Fail → 同步执行失败信息

---

## 8. 成功标准

- 所有执行必须可视、可追溯
- 没有“假完成”
- 人和 AI 使用同一执行模型

---

## 9. 总结

这是一个以 Kanban 为中枢、以 Spec 为裁判、以 Submit API 为唯一执行入口的执行型协同系统。