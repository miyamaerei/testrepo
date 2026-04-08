# E-Kanban 功能清单与实施步骤（S/ 思维整理）

## 一、需求背景

用户需要在现有 **Vue.NetCore (Vol.Core)** 框架基础上，开发一个 **执行型 Kanban 协调系统**：

- 对接 **Azure Boards**，拉取工作项
- 作为**人机协同执行中枢**，支持人/AI/系统三种执行方式
- AI 执行使用 **GitHub Copilot CLI 编程模式**自动执行
- **Spec 校验机制**：只有 Spec 通过才能算完成，避免假完成
- **AI 防偷懒机制**：AI 分批执行，超时检测，自动重试，人工干预

---

## 二、核心功能清单

### 模块 1：Azure Boards 集成

| 功能点 | 实现说明 | 注意事项 |
|--------|----------|---------|
| 定时拉取工作项 | Quartz 定时任务调用 Azure Boards API，拉取所有工作项 | 做好幂等，重复拉不重复创建 |
| 同步标题/描述/状态 | 每次拉取对比更新，保持 Kanban 和 Azure Boards 一致 | 记录最后同步时间 |
| 执行完成回写 | Kanban 卡片完成后，回写评论和状态到 Azure Boards | 需要 PAT 权限，配置化 |

**研究点**：
- 研究 Azure Boards REST API 认证方式（PAT）
- 研究 WIQL 查询语法，如何获取所有工作项
- 研究如何添加评论和更新状态

---

### 模块 2：Kanban 核心领域

| 功能点 | 实现说明 | 注意事项 |
|--------|----------|---------|
| 卡片状态机 | New → Ready → InProgress → Submitted → Completed / Failed | Submit ≠ Completed，只有 Spec 通过才能到 Completed |
| 支持三种执行者类型 | Human / AI / System | 不同执行者执行流程不同 |
| 按状态分组展示 | 前端看板按状态列展示卡片 | 简单可视化，不需要拖拽（MVP 阶段）|
| 执行历史记录 | 每次提交都创建 ExecutionRun，记录完整过程 | 支持追溯多次迭代过程 |

**状态机流转规则**：

```
New (新建)
  ↓ 生成 Spec
Ready (就绪，等待执行)
  ↓ 开始执行
InProgress (执行中) ←← 重试失败任务 ←←
  ↓ 提交结果
Submitted (已提交，等待评估)
  ↓ Spec 评估
  ├─→ Pass → Completed (完成) → 回写 Azure Boards → 结束
  └─→ Fail → Ready (就绪，等待下一轮) → 失败次数+1
```

**研究点**：
- 状态机是否需要设计模式，还是简单判断即可
- 如何处理并发执行，同一张卡会不会同时多次执行

---

### 模块 3：Spec Engine

| 功能点 | 实现说明 | 注意事项 |
|--------|----------|---------|
| AI 自动生成 Spec | 从任务标题和描述，调用 GitHub Copilot CLI 生成清晰可验证的验收标准 | prompt 要写好，让 AI 生成每条可验证的标准 |
| Spec 评估执行结果 | 根据 Spec 检查执行证据，判定 Pass/Fail | 输出结果要求 AI 在最后明确给出 RESULT: PASS/FAIL，便于解析 |
| 根据评估结果驱动状态流转 | Pass → Completed，Fail → Ready | 需要更新卡片状态，记录评估结果 |

**提示词模板（给 Copilot CLI）**：

**生成 Spec 提示词**：
```
# 任务：生成完成验收标准（Spec）

请为以下任务生成清晰、可验证的完成验收标准。

## 任务信息
标题：{title}
描述：
{description}

## 要求
1. Spec 需要清晰、具体、可验证
2. 每条标准应该能够明确判定"满足"或"不满足"
3. 不要模糊的描述，要具体可检查
4. 使用 markdown 列表格式输出

请直接输出 Spec 内容，不要其他解释：
```

**评估提示词**：
```
# 任务：评估执行结果是否符合验收标准

## 验收标准（Spec）
{spec}

## 执行证据（Execution Output）
{evidence}

## 评估要求
请对照上面的验收标准，逐条检查执行证据是否满足所有要求。

最后，请给出总结判定：**RESULT: PASS** 或 **RESULT: FAIL**

如果有任何一条标准不满足，就判定为 FAIL。

格式要求：
- 首先逐条说明检查结果
- 最后一行必须是：RESULT: PASS 或 RESULT: FAIL

请开始评估：
```

**研究点**：
- 如何让 AI 生成的 Spec 更可验证
- 解析评估结果的可靠性，如果 AI 不按格式输出怎么处理

---

### 模块 4：GitHub Copilot CLI AI 执行集成

| 功能点 | 实现说明 | 注意事项 |
|--------|----------|---------|
| 调用 copilot CLI 编程模式 | `copilot -p "prompt" --allow-all-tools` | 使用 `Process` 启动，捕获标准输出和退出码 |
| 构造执行 prompt | 包含任务标题、描述、Spec，指导 AI 完成开发 | prompt 需要清晰，让 AI 知道要做什么 |
| 执行完成自动 Submit | 捕获输出后，作为 Evidence，调用 Submit API 自提交 | 需要处理执行超时、进程异常退出 |
| 支持分批迭代执行 | 每次执行只做一部分，多次迭代完成整个任务 | 每轮执行都是独立的 `ExecutionRun`，完整记录 |

**工作流程**：

```
卡片进入 InProgress → 记录开始时间
    ↓
构造 prompt（任务描述 + Spec）
    ↓
Process 启动 copilot CLI
    ↓
等待执行完成，捕获 stdout/stderr/exitCode/duration
    ↓
调用 Submit API 提交结果
    ↓
Spec 评估 → 状态流转
```

**配置项**（appsettings.json）：

```json
"CopilotCli": {
  "CommandPath": "copilot",         // copilot 命令路径
  "WorkingDirectory": "C:\\repo",   // 工作目录（代码仓库在哪里）
  "TimeoutSeconds": 300             // 超时时间（默认 5 分钟）
}
```

**研究点**：
- 如何处理 copilot 长时间执行不退出
- 工作目录权限问题，确保进程有权限读写代码
- 环境变量问题，copilot 需要登录认证，进程环境是否有凭据

---

### 模块 5：AI 防偷懒机制

| 功能点 | 实现说明 | 注意事项 |
|--------|----------|---------|
| 定时扫描检查 | Quartz 定时任务，每隔 N 分钟扫描一次 | 默认 15 分钟检查一次，可配置 |
| 超时检测 | InProgress 状态超过 X 小时没有提交 → 判定超时 | 默认 2 小时，可配置 |
| 自动重试 | 超时任务，如果失败次数 < 最大重试次数 → 自动重新调度执行 | 默认最多 3 次自动重试，可配置 |
| 失败次数上限 | 超过最大重试次数 → 标记为需要人工干预，通知管理员 | 不再自动重试，等待人工处理 |
| 手动重新执行 | 前端支持管理员手动触发任意 AI 任务重新执行 | 给卡住的任务一次人工推动的机会 |
| 失败次数记录 | 每张卡片记录当前失败次数 | 每次重试失败次数+1 |

**配置项**：

```json
"AiExecution": {
  "InProgressTimeoutMinutes": 120,  // InProgress 超时时间
  "MaxAutoRetries": 3,              // 最大自动重试次数
  "CheckIntervalMinutes": 15        // 检查间隔
}
```

**扫描逻辑**：

```csharp
foreach (var card in 所有 InProgress 状态且 ExecutorType = AI 的卡片)
{
    if (当前时间 - 卡片.LastUpdated > 超时时间)
    {
        if (卡片.失败次数 < 最大重试次数)
        {
            // 自动重试
            调度 AI 执行任务
            卡片.失败次数 += 1
            await _cardRepository.UpdateAsync(card);
            _logger.Info($"AI 任务 {card.Id} 超时，自动重试（第 {card.失败次数} 次）");
        }
        else
        {
            // 需要人工干预
            // TODO: 通知管理员
            _logger.Warn($"AI 任务 {card.Id} 超时且已达到最大重试次数，需要人工干预");
        }
    }
}
```

**研究点**：
- 通知管理员方式：邮件？消息？还是看板上标记即可？
- 重新执行是否需要重新生成 Spec？还是复用原有 Spec？

---

### 模块 6：Submit API（核心入口）

| 功能点 | 实现说明 | 注意事项 |
|--------|----------|---------|
| 统一提交入口 | `POST /api/ekanban/ExecutionCard/Submit` | 人/AI/系统都用这个入口提交 |
| 创建 ExecutionRun | 每次提交都创建新的执行记录 | 完整追溯谁在什么时候提交了什么证据 |
| 触发 Spec 评估 | 提交后自动调用 SpecEvaluator 评估 | 评估结果驱动状态流转 |
| 状态回写 | 完成后回写到 Azure Boards | 保持两端一致 |

**提交请求结构**：

```json
{
  "executionCardId": 123,
  "submittedBy": "AI",   // 提交者：用户名/AI/System
  "evidence": "执行输出结果..."
}
```

**研究点**：
- 认证：需要登录用户才能提交吗？AI 自动提交怎么认证？
- 幂等：重复提交会不会有问题？

---

### 模块 7：前端 Kanban 可视化

| 功能点 | 实现说明 | 注意事项 |
|--------|----------|---------|
| 按状态分列展示 | 四列：Ready / InProgress / Completed / Failed | 简单可视化，MVP 不需要拖拽 |
| 卡片展示基本信息 | 标题、执行者类型、失败次数、最后更新时间 | 一目了然看到哪些卡住了 |
| 重新执行按钮 | 对于卡住的 AI 任务，支持手动点击重新执行 | 管理员操作 |
| 同步 Azure Boards 按钮 | 手动触发立即同步 | 不用等定时任务 |
| 使用 Element Plus | 遵循 Vol.Core 前端规范，使用 Element Plus 组件 | 不用自己写样式 |

**布局草图**：

```
┌─────────┬─────────────┬─────────────┬─────────────┐
│ Ready  │ InProgress  │ Completed  │ Failed    │
├─────────┼─────────────┼─────────────┼─────────────┤
│ [卡片] │  [卡片]    │  [卡片]    │  [卡片]    │
│ [卡片] │  [卡片]    │  [卡片]    │           │
│       │  [卡片]    │            │           │
└─────────┴─────────────┴─────────────┴─────────────┘
```

每个卡片：
```
┌─────────────────────────┐
│ 标题                     │
│ 类型：AI  失败次数：1/3   │
│ 最后更新：2026-04-01     │
│                         │
│ [重新执行]              │  ← 按钮只有 InProgress/Failed 显示
└─────────────────────────┘
```

**研究点**：
- 是否需要拖拽改状态？MVP 可以先不做，手动操作在列表也能改
- 是否需要分页？卡片不多，全加载出来也没问题

---

### 模块 8：配置

需要在 `appsettings.json` 添加的配置：

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=...;Database=E-Kanban;..."
},
"AzureBoards": {
  "OrganizationUrl": "https://dev.azure.com/your-org",
  "Project": "your-project",
  "PersonalAccessToken": "your-pat"
},
"CopilotCli": {
  "CommandPath": "copilot",
  "WorkingDirectory": "C:\\path\\to\\your\\repo",
  "TimeoutSeconds": 300
},
"AiExecution": {
  "InProgressTimeoutMinutes": 120,
  "MaxAutoRetries": 3,
  "CheckIntervalMinutes": 15
}
```

---

## 三、实施步骤（按顺序）

### 第一步：创建所有实体
按照 Vol.Core 规范，在 `VOL.Entity/DomainModels` 创建：
1. `BoardWorkItem.cs`
2. `ExecutionCard.cs`（包含状态枚举、执行者类型枚举）
3. `ExecutionTask.cs`
4. `ExecutionRun.cs`
5. `Spec.cs`
6. `SpecEvaluation.cs`

### 第二步：创建所有仓储
按照 Vol.Core 规范，在 `EKanban/IRepositories` 和 `EKanban/Repositories` 创建：
1. `IExecutionCardRepository` + `ExecutionCardRepository`
2. `IExecutionRunRepository` + `ExecutionRunRepository`
3. `ISpecRepository` + `SpecRepository`
4. `ISpecEvaluationRepository` + `SpecEvaluationRepository`

### 第三步：创建核心服务
1. `IAzureBoardsClient` + `AzureBoardsClient` - Azure Boards API 调用
2. `ISyncService` + `SyncService` - 同步逻辑
3. `IExecutionCardService` + `ExecutionCardService` - 卡片管理
4. `IStateMachineService` + `StateMachineService` - 状态机推进
5. `ISubmitService` + `SubmitService` - 提交处理

### 第四步：创建 Spec Engine
1. `ISpecGenerator` + `SpecGenerator` - Spec 生成
2. `ISpecEvaluator` + `SpecEvaluator` - Spec 评估

### 第五步：创建 GitHub Copilot CLI 集成
1. `ICopilotCliClient` + `CopilotCliClient` - CLI 调用
2. `IAiExecutionService` + `AiExecutionService` - AI 执行逻辑

### 第六步：创建防偷懒机制
1. `IAiTaskCheckService` + `AiTaskCheckService` - 超时检查
2. `AiTaskCheckJob` - Quartz 定时任务

### 第七步：创建 API 控制器
在 `VOL.WebApi/Controllers/EKanban` 创建 `ExecutionCardController`，包含：
- 获取看板数据
- 触发重新执行
- 触发同步

### 第八步：更新配置
在 `appsettings.json` 添加配置项

### 第九步：前端开发
1. `vol.web/src/api/ekanban.ts` - API 封装
2. `vol.web/src/views/ekanban/KanbanBoard/index.vue` - 看板页面
3. `vol.web/src/views/ekanban/KanbanBoard/options.js` - 页面配置
4. `vol.web/src/components/ekanban/KanbanCard.vue` - 卡片组件
5. `vol.web/src/router/index.ts` - 添加路由

### 第十步：配置 Quartz 定时任务
在系统 UI 中添加两个定时任务：
1. **同步 Azure Boards**：Cron `0 */15 * * * ?` 每 15 分钟
2. **检查 AI 任务超时**：Cron `0 */15 * * * ?` 每 15 分钟

---

## 四、关键风险与应对

| 风险 | 影响 | 应对措施 |
|-----|------|---------|
| Copilot CLI 调用权限问题 | AI 执行失败 | 测试进程调用，确保工作目录正确、copilot 已登录 |
| AI 生成 Spec 质量差 | 评估不准确 | prompt 写清晰，要求每条可验证 |
| AI 一直失败达不到完成标准 | 任务卡住 | 最大重试次数限制，超过后通知人工干预 |
| 定时任务重复执行 | 重复调度 | Quartz 可靠调度，幂等处理 |
| Copilot CLI 执行超时 | 进程挂住 | 设置超时时间，超时强制杀死进程 |

---

## 五、S/ 总结

| 阶段 | 研究内容 | 输出 |
|-----|---------|------|
| **需求理解** | 理清用户要解决什么问题 | 结构化需求文档 |
| **框架研究** | 研究 Vol.Core 开发规范 | 开发指南 + 模板 |
| **API 研究** | 研究 Azure Boards REST API 用法 | 知道怎么拉取、怎么回写 |
| **CLI 研究** | 研究 GitHub Copilot CLI 编程模式用法 | 知道怎么传 prompt、怎么捕获输出 |
| **设计** | 设计状态机、防偷懒机制 | 功能清单 + 流程梳理 |
| **实施** | 按顺序增量开发 | 可运行的代码 |
| **验证** | 测试每一个功能点 | 修复问题 |
| **沉淀** | 更新文档，记录经验 | 知识沉淀 |

整个过程遵循**小步快跑，增量验证**，每完成一个模块就测试一个模块，不要一下子写很多代码再测试。
