# 从看板到AI执行的完整流程文档

## 1. 流程概述

E-Kanban 实现了从 Azure Boards 工作项到 AI 执行的完整闭环流程，涵盖工作项同步、执行卡片创建、状态管理、AI 执行、结果评估等多个环节。

## 2. 详细流程步骤

### 2.1 工作项同步

**功能说明**：定时从 Azure Boards 拉取工作项并创建对应的执行卡片。

**实现细节**：
- 通过 `SyncService.SyncFromAzureBoardsAsync()` 方法执行同步
- 调用 `AzureBoardsClient.GetAllWorkItemsAsync()` 获取工作项
- 对新工作项创建 `BoardWorkItem` 记录
- 为每个工作项创建对应的 `ExecutionCard`，初始状态为 `New`
- 对现有工作项更新其属性

**涉及表**：
- `BoardWorkItems`：存储 Azure Boards 工作项
- `ExecutionCards`：创建对应的执行卡片

### 2.2 状态管理

**功能说明**：管理执行卡片的状态流转，确保流程的正确性。

**状态流转**：
- `New` → `Ready`：准备执行
- `Ready` → `InProgress`：开始执行
- `InProgress` → `Submitted`：执行完成，等待评估
- `Submitted` → `Completed`：评估通过，任务完成
- `Submitted` → `Ready`：评估失败，返回就绪状态
- `Failed` → `Ready`：失败后重试

**实现细节**：
- `StateMachineService` 管理状态转换
- 定义了严格的状态转换规则
- 记录状态变更时间和执行开始时间

**涉及表**：
- `ExecutionCards`：更新状态字段

### 2.3 AI 执行

**功能说明**：调用 GitHub Copilot CLI 执行开发任务。

**实现细节**：
- `AiExecutionService.ExecuteAiTaskAsync()` 处理 AI 执行
- 标记卡片为 `InProgress` 状态
- 初始化 S/E 六阶段进度跟踪
- 确定工作目录（支持多项目）
- 构建执行提示
- 调用 `CopilotCliClient.ExecutePromptAsync()` 执行任务
- 自动提交执行结果

**涉及表**：
- `ExecutionCards`：更新执行状态
- `TaskPhaseProgress`：初始化和更新阶段进度
- `ProjectRepositories`：获取项目工作目录

### 2.4 结果提交与评估

**功能说明**：处理执行结果，评估是否满足验收标准。

**实现细节**：
- `SubmitService.SubmitExecutionResultAsync()` 处理执行结果
- 创建 `ExecutionRun` 记录执行历史
- 完成当前阶段并自动开始下一阶段
- 生成或评估 Spec
- 根据评估结果更新卡片状态

**涉及表**：
- `ExecutionRuns`：记录执行历史
- `TaskPhaseProgress`：更新阶段进度
- `Specs`：生成或评估验收标准
- `SpecEvaluations`：记录评估结果

### 2.5 AI 防偷懒机制

**功能说明**：监控执行中的任务，对超时任务进行处理。

**实现细节**：
- `AiTaskCheckService` 定期检查执行中的任务
- 对超时任务进行自动重试
- 超过最大重试次数后标记为需要人工干预

**涉及表**：
- `ExecutionCards`：更新失败次数和干预标志

## 3. 核心表结构

### 3.1 主要表

| 表名 | 作用 | 关键字段 |
|------|------|---------|
| **BoardWorkItems** | 存储 Azure Boards 工作项 | ExternalWorkItemId, Title, Description, ExternalState |
| **ExecutionCards** | 核心执行卡片 | BoardWorkItemId, Status, ExecutorType, SpecId, ProjectRepositoryId |
| **Specs** | 验收标准 | ExecutionCardId, Definition |
| **SpecEvaluations** | Spec 评估记录 | ExecutionRunId, SpecId, Result, Message |
| **ExecutionTasks** | 执行定义任务 | ExecutionCardId, ExecutorType, ExecutionInstructions |
| **ExecutionRuns** | 执行事实记录 | ExecutionCardId, SubmittedBy, Evidence, DurationMs |
| **ProjectRepositories** | 项目仓库配置 | Name, LocalWorkingDir, GitRemoteUrl |
| **TaskPhaseProgress** | 任务阶段进度 | ExecutionCardId, Phase, Status, StartedAt, CompletedAt |
| **TaskFileChanges** | 任务文件变更 | ExecutionCardId, FilePath, ChangeType, CommitHash |

### 3.2 表关系

- `BoardWorkItems` → `ExecutionCards`：1:N
- `ExecutionCards` → `ExecutionTasks`：1:N
- `ExecutionCards` → `ExecutionRuns`：1:N
- `ExecutionCards` → `TaskPhaseProgress`：1:N
- `ExecutionCards` → `TaskFileChanges`：1:N
- `ExecutionCards` → `Specs`：1:1
- `ExecutionRuns` → `SpecEvaluations`：1:N
- `ProjectRepositories` → `ExecutionCards`：1:N

## 4. 技术实现亮点

### 4.1 多项目支持
- 通过 `ProjectRepositories` 表管理多个项目仓库
- AI执行时自动在关联项目的本地目录中工作
- 支持不同任务关联到不同项目

### 4.2 细粒度进度跟踪
- S/E六阶段进度跟踪（需求分析、代码盘点、需求映射、增量开发、验证测试、知识沉淀）
- 每个阶段有独立的状态和时间记录
- 自动阶段流转，提高执行效率

### 4.3 AI 防偷懒机制
- 超时检测：监控执行中的任务
- 自动重试：失败任务自动回到就绪状态
- 人工干预：超过重试次数后标记需要人工处理

### 4.4 Spec Engine
- AI生成验收标准，确保任务真正完成
- 自动评估执行结果是否满足验收标准
- 避免"假完成"现象

### 4.5 统一执行模型
- 支持人、AI、系统三种角色统一执行模型
- 统一的Submit API，简化执行流程
- 统一的状态管理和进度跟踪

## 5. 系统架构

### 5.1 架构层次
1. **表现层**：Vue 3 + Element Plus 前端
2. **API层**：.NET 8 Web API
3. **服务层**：核心业务逻辑
4. **数据访问层**：SqlSugar ORM
5. **存储层**：SQL Server 2019+

### 5.2 关键组件

| 组件 | 主要职责 | 交互关系 |
|------|---------|---------|
| **SyncService** | 同步 Azure Boards 工作项 | 调用 AzureBoardsClient，创建 BoardWorkItem 和 ExecutionCard |
| **StateMachineService** | 管理卡片状态流转 | 处理状态转换，确保合法状态变更 |
| **AiExecutionService** | AI 任务执行 | 调用 CopilotCliClient，初始化阶段进度，提交执行结果 |
| **SubmitService** | 处理执行结果 | 记录执行历史，完成阶段进度，评估 Spec |
| **SpecEvaluator** | 评估执行结果 | 验证执行结果是否满足验收标准 |
| **AiTaskCheckService** | 防偷懒检查 | 监控超时任务，自动重试 |

## 6. 流程示例

### 6.1 完整流程示例

1. **同步**：系统定时从 Azure Boards 拉取工作项
2. **创建**：为每个工作项创建执行卡片，状态为 `New`
3. **准备**：卡片状态从 `New` 变为 `Ready`
4. **执行**：AI 执行任务，状态变为 `InProgress`
5. **提交**：执行完成，状态变为 `Submitted`
6. **评估**：Spec Engine 评估是否通过
7. **完成**：评估通过，状态变为 `Completed`
8. **重试**：评估失败，状态回到 `Ready` 等待下一轮执行

### 6.2 多项目执行示例

1. **项目配置**：在 `ProjectRepositories` 中配置多个项目仓库
2. **关联项目**：为执行卡片关联特定项目
3. **AI执行**：AI 自动在关联项目的本地目录中工作
4. **结果提交**：执行结果与项目关联，便于跟踪

## 7. 配置说明

### 7.1 数据库连接
修改 `VOL.WebApi/appsettings.json` 中的连接字符串。

### 7.2 Azure Boards 配置
配置组织URL、项目、PAT和同步时间表达式。

### 7.3 AI 执行防偷懒配置
配置执行超时时间、最大重试次数和检查间隔。

### 7.4 Copilot CLI 配置
配置 Copilot CLI 命令路径、默认工作目录和执行超时时间。

## 8. 开发与部署

### 8.1 构建运行
- 后端：`dotnet restore`、`dotnet build`、`dotnet publish`
- 前端：`npm install`、`npm run build`

### 8.2 部署方式
- Docker 部署
- IIS 部署

## 9. 总结

E-Kanban 实现了一个完整的从看板到AI执行的闭环系统，通过整合 Azure Boards、GitHub Copilot CLI 和 Spec Engine，实现了智能、高效的任务执行流程。系统支持多项目管理、细粒度进度跟踪和AI防偷懒机制，为团队协作和项目管理提供了有力的支持。