# 需求-代码映射表

基于 Vue.NetCore (Vol.Core) SqlSugar + Vue3 骨架，增量开发 E-Kanban 业务功能。

---

## 1. 项目整体结构

项目遵循 Vue.NetCore 分层架构：

```
E-Kanban/
├── VOL.Entity/                    # 实体层（框架已有）
│   ├── DomainModels/             # ← 我们在这里新增 E-Kanban 领域实体
│   ├── BaseCore/                 # 框架基类（已有）
│   └── SystemModels/             # 系统实体（用户、角色，已有）
├── VOL.Core/                     # 核心层（框架已有）
│   ├── BaseRepository/           # 基类仓储（已有）
│   ├── Quartz/                   # 定时任务（已有，替代 Hangfire）
│   └── Jwt/                      # JWT 认证（已有）
├── VOL.WebApi/                   # Web API 入口（框架已有）
│   ├── Controllers/              # ← 在这里新增 E-Kanban 控制器
│   ├── Program.cs                # 主程序（已有）
│   └── appsettings.json          # 配置文件（需要添加配置）
├── VOL.Sys/                      # 系统模块（用户权限，已有）
├── EKanban/                      # ← 我们新增的 E-Kanban 业务模块（遵循框架分层）
│   ├── IRepositories/            # 仓储接口
│   ├── Repositories/             # 仓储实现
│   ├── IServices/                # 服务接口
│   ├── Services/                 # 服务实现
│   ├── AiExecution/              # GitHub Copilot CLI 集成（独立目录）
│   ├── Specs/                    # Spec Engine（生成 + 评估）
│   └── Jobs/                     # Quartz 定时任务（包含防偷懒检查）
├── vol.web/                      # Vue3 前端（框架已有）
│   ├── src/
│   │   ├── views/
│   │   │   └── Kanban/           # ← 新增 Kanban 页面
│   │   ├── components/
│   │   │   └── Kanban/           # ← 新增 Kanban 组件
│   │   ├── api/
│   │   │   └── kanban.ts         # ← 新增 API 封装
│   │   └── router/
│   │       └── index.ts          # 需要添加路由
│   └── package.json              # 已有（Element Plus 已引入）
├── docs/                          # 项目文档
│   ├── requirements-structured.md
│   ├── code-inventory.md
│   └── requirements-code-mapping.md
└── VOL.sln                        # 解决方案文件
```

---

## 2. 需求-代码映射表

| 功能点 | 当前状态 | 操作类型 | 需要新增/修改的文件 | 依赖框架模块 | 优先级 |
|--------|----------|----------|---------------------|--------------|--------|
| **领域模型** | | | | | |
| BoardWorkItem | 无 | 新增 | `VOL.Entity/DomainModels/BoardWorkItem.cs` | 框架基类 | P0 |
| ExecutionCard | 无 | 新增 | `VOL.Entity/DomainModels/ExecutionCard.cs` | 框架基类 | P0 |
| ExecutionTask | 无 | 新增 | `VOL.Entity/DomainModels/ExecutionTask.cs` | 框架基类 | P0 |
| ExecutionRun | 无 | 新增 | `VOL.Entity/DomainModels/ExecutionRun.cs` | 框架基类 | P0 |
| Spec | 无 | 新增 | `VOL.Entity/DomainModels/Spec.cs` | 框架基类 | P0 |
| SpecEvaluation | 无 | 新增 | `VOL.Entity/DomainModels/SpecEvaluation.cs` | 框架基类 | P0 |
| **仓储层** | | | | | |
| IExecutionCardRepository | 无 | 新增 | `EKanban/IRepositories/IExecutionCardRepository.cs` | SqlSugar 基类 | P0 |
| ExecutionCardRepository | 无 | 新增 | `EKanban/Repositories/ExecutionCardRepository.cs` | IRepository | P0 |
| ISpecRepository | 无 | 新增 | `EKanban/IRepositories/ISpecRepository.cs` | SqlSugar 基类 | P0 |
| SpecRepository | 无 | 新增 | `EKanban/Repositories/SpecRepository.cs` | IRepository | P0 |
| ISpecEvaluationRepository | 无 | 新增 | `EKanban/IRepositories/ISpecEvaluationRepository.cs` | SqlSugar 基类 | P0 |
| SpecEvaluationRepository | 无 | 新增 | `EKanban/Repositories/SpecEvaluationRepository.cs` | IRepository | P0 |
| IExecutionRunRepository | 无 | 新增 | `EKanban/IRepositories/IExecutionRunRepository.cs` | SqlSugar 基类 | P0 |
| ExecutionRunRepository | 无 | 新增 | `EKanban/Repositories/ExecutionRunRepository.cs` | IRepository | P0 |
| **服务层** | | | | | |
| IAzureBoardsClient | 无 | 新增 | `EKanban/Services/IAzureBoardsClient.cs` | - | P0 |
| AzureBoardsClient | 无 | 新增 | `EKanban/Services/AzureBoardsClient.cs` | IAzureBoardsClient | P0 |
| ISyncService | 无 | 新增 | `EKanban/IServices/ISyncService.cs` | - | P0 |
| SyncService | 无 | 新增 | `EKanban/Services/SyncService.cs` | IAzureBoardsClient | P0 |
| IExecutionCardService | 无 | 新增 | `EKanban/IServices/IExecutionCardService.cs` | - | P0 |
| ExecutionCardService | 无 | 新增 | `EKanban/Services/ExecutionCardService.cs` | IExecutionCardRepository | P0 |
| IStateMachineService | 无 | 新增 | `EKanban/IServices/IStateMachineService.cs` | - | P0 |
| StateMachineService | 无 | 新增 | `EKanban/Services/StateMachineService.cs` | ExecutionCard | P0 |
| ISubmitService | 无 | 新增 | `EKanban/IServices/ISubmitService.cs` | - | P0 |
| SubmitService | 无 | 新增 | `EKanban/Services/SubmitService.cs` | SpecEngine, AiExecution | P0 |
| IAiTaskCheckService | 无 | 新增 | `EKanban/IServices/IAiTaskCheckService.cs` | - | P0 |
| AiTaskCheckService | 无 | 新增 | `EKanban/Services/AiTaskCheckService.cs` | IExecutionCardRepository | P0 |
| **Spec Engine** | | | | | |
| ISpecGenerator | 无 | 新增 | `EKanban/Specs/ISpecGenerator.cs` | - | P0 |
| SpecGenerator | 无 | 新增 | `EKanban/Specs/SpecGenerator.cs` | CopilotCliClient | P0 |
| ISpecEvaluator | 无 | 新增 | `EKanban/Specs/ISpecEvaluator.cs` | - | P0 |
| SpecEvaluator | 无 | 新增 | `EKanban/Specs/SpecEvaluator.cs` | CopilotCliClient | P0 |
| **GitHub Copilot CLI AI 执行** | | | | | |
| ICopilotCliClient | 无 | 新增 | `EKanban/AiExecution/ICopilotCliClient.cs` | - | P0 |
| CopilotCliClient | 无 | 新增 | `EKanban/AiExecution/CopilotCliClient.cs` | - | P0 |
| IAiExecutionService | 无 | 新增 | `EKanban/AiExecution/IAiExecutionService.cs` | - | P0 |
| AiExecutionService | 无 | 新增 | `EKanban/AiExecution/AiExecutionService.cs` | ICopilotCliClient | P0 |
| **定时任务** | | | | | |
| SyncFromAzureBoardsJob | 无 | 新增 | `EKanban/Jobs/SyncFromAzureBoardsJob.cs` | Quartz | P0 |
| AiExecutionJob | 无 | 新增 | `EKanban/Jobs/AiExecutionJob.cs` | Quartz, IAiExecutionService | P0 |
| AiTaskCheckJob | 无 | 新增 | `EKanban/Jobs/AiTaskCheckJob.cs` | Quartz, IAiTaskCheckService | P0 |
| **API 控制器** | | | | | |
| ExecutionCardController | 无 | 新增 | `VOL.WebApi/Controllers/EKanban/ExecutionCardController.cs` | Services | P0 |
| **重新执行 API** | 无 | 新增 | `VOL.WebApi/Controllers/EKanban/ExecutionCardController` 增加 `TriggerReExecute` 方法 | Services | P0 |
| **配置文件** | | | | | |
| appsettings.json | 已有 | 修改 | `VOL.WebApi/appsettings.json` | - | P0 |
| **配置** | 无 | 新增 | 添加 `AiExecution` 配置（超时时间、最大重试次数） | - | P0 |
| **前端 UI** | | | | | |
| KanbanCard 组件 | 无 | 新增 | `vol.web/src/components/Kanban/KanbanCard.vue` | Element Plus | P0 |
| Kanban 看板页面 | 无 | 新增 | `vol.web/src/views/Kanban/KanbanBoard.vue` | Element Plus | P0 |
| API 封装 | 无 | 新增 | `vol.web/src/api/kanban.ts` | axios | P0 |
| 路由配置 | 已有 | 修改 | `vol.web/src/router/index.ts` | - | P0 |
| 重新执行按钮 | 无 | 新增 | `vol.web/src/components/Kanban/KanbanCard.vue` 增加手动触发 | Element Plus | P0 |

---

## 3. 技术选型（基于 Vue.NetCore 确认）

### 3.1 后端（.NET 8 + SqlSugar）

| 组件 | 选型 | 来源 | 说明 |
|-----|------|------|------|
| **框架** | ASP.NET Core Web API | .NET 8 | 框架已有 |
| **ORM** | SqlSugar | 用户指定 + 框架已有 | 所有业务表都用 SqlSugar |
| **定时任务** | Quartz.NET | 框架已有，替代 Hangfire | 用户说明框架自带 |
| **认证** | JWT + ASP.NET Core Identity | 框架已有 | 用户权限已实现 |
| **依赖注入** | Autofac | 框架已有 | 自动扫描注册 |
| **HTTP 客户端** | IHttpClientFactory + HttpClient | .NET 内置 | |
| **进程调用** | System.Diagnostics.Process | .NET 内置 | 调用 copilot CLI |

### 3.2 前端（Vue 3 + Vite + Element Plus）

| 组件 | 选型 | 来源 | 说明 |
|-----|------|------|------|
| **框架版本** | Vue 3 + Composition API | 框架已有 | vol.web 就是 Vue3 |
| **构建工具** | Vite | 框架已有 | |
| **UI 组件库** | Element Plus | 框架已有 | 用户指定，框架已经引入 |
| **HTTP 客户端** | Axios | 框架已有 | |
| **路由** | Vue Router | 框架已有 | |
| **状态管理** | Pinia | 框架已有 | 需要时使用 |

### 3.3 数据库（SQL Server）

| 组件 | 配置 |
|-----|------|
| **版本** | SQL Server 2019+ |
| **表创建** | SqlSugar CodeFirst |
| **连接字符串** | appsettings.json 配置 |

### 3.4 防偷懒机制配置

在 `appsettings.json` 中新增配置：

```json
"AiExecution": {
  "InProgressTimeoutMinutes": 120,      // InProgress 超时时间（默认 2 小时）
  "MaxAutoRetries": 3,                  // 最大自动重试次数
  "CheckIntervalMinutes": 15            // 检查间隔（默认 15 分钟）
}
```

### 3.5 关键变更总结

| 原计划 | 变更为 | 原因 |
|--------|--------|------|
| Hangfire 定时任务 | **Quartz.NET** | Vue.NetCore 框架已经集成 Quartz，用户说明框架自带了 |
| 自己创建项目结构 | **基于 Vue.NetCore 骨架增量开发** | 用户要求使用这个开源项目作为基础 |
| EF Core 用于 Identity | **框架已经处理好了** | 用户说"用户部分已经写好了，不要用efcore"，框架已经搞定 |
| AI 一次性执行 | **支持分批迭代 + 防偷懒检查** | 用户指出 AI 需要分批执行，需要防止偷懒 |

---

## 4. GitHub Copilot CLI 集成 + 防偷懒机制说明

### 4.1 角色定位

E-Kanban 作为**任务协调中枢**：
- 当卡片进入 InProgress 状态且执行者类型为 AI 时
- 通过 **Quartz** 调度后台任务（替代 Hangfire）
- 后台任务调用 `copilot -p "prompt" --allow-all-tools` 编程模式执行
- 执行完成后，收集输出作为证据，调用 Submit API 自提交
- Spec Engine 评估结果，驱动状态流转
- **定时扫描检查 InProgress 任务，超时自动重试，防止 AI 偷懒**

### 4.2 分批迭代 + 防偷懒工作流程

```
1. 从 Azure Boards 同步得到新任务
   ↓
2. 创建 ExecutionCard，ExecutorType = AI，状态 = New
   ↓
3. 生成 Spec → 状态转为 Ready
   ↓
4. 触发 AI 执行 → 状态 = InProgress，记录开始时间，重置失败次数 = 0
   ↓
5. Quartz 调度 AI 执行任务
   ↓
6. AiExecutionService 调用 copilot CLI → 获取输出 → 创建 ExecutionRun → 自动 Submit
   ↓
7. SpecEvaluator 评估 → Passed/Failed
   ↓
8. 如果 Passed → 状态 = Completed → 回写 Azure Boards → 结束
   ↓
9. 如果 Failed → 状态 = Ready（等待下一轮）→ 失败次数 + 1
   ↓
10. 防偷懒检查（AiTaskCheckJob 每 15 分钟运行一次）
     - 扫描所有 InProgress 状态的 AI 任务
     - 如果 当前时间 - 开始时间 > 超时时间（默认 2 小时）→ 超时
     - 如果 失败次数 < 最大重试次数（默认 3 次）→ 自动重新调度执行 → 失败次数 + 1
     - 如果 失败次数 >= 最大重试次数 → 保持 InProgress，但标记为需要人工干预 → 通知管理员
```

### 4.3 防偷懒机制新增模块

| 模块 | 文件 | 说明 |
|-----|------|------|
| `IAiTaskCheckService` | `EKanban/IServices/IAiTaskCheckService.cs` | 接口：检查超时 AI 任务 |
| `AiTaskCheckService` | `EKanban/Services/AiTaskCheckService.cs` | 实现：扫描超时任务，触发重试 |
| `AiTaskCheckJob` | `EKanban/Jobs/AiTaskCheckJob.cs` | Quartz 定时任务，定期检查 |
| 手动重新执行 API | `ExecutionCardController.TriggerReExecute` | 支持管理员手动触发重新执行 |
| 前端重新执行按钮 | `KanbanCard.vue` | 在卡片上显示重新执行按钮 |

### 4.4 AI 执行模块位置

放在 `EKanban/AiExecution/` 目录：
- `ICopilotCliClient.cs` - 接口：封装调用 copilot CLI 的底层能力
- `CopilotCliClient.cs` - 实现：使用 `Process` 启动 `copilot` 命令，捕获输出
- `IAiExecutionService.cs` - 接口：AI 执行高层逻辑
- `AiExecutionService.cs` - 实现：构造 prompt，调用 CLI，处理结果

---

## 5. 开发顺序建议

按照依赖关系，建议开发顺序：

1. **确认 Vue.NetCore 骨架结构** → 已经复制完成
2. **新增领域实体** → 所有实体到 `VOL.Entity/DomainModels`
3. **新增仓储接口和实现** → 遵循框架分层
4. **新增服务接口和实现** → Azure Boards 同步、Kanban 核心、Spec Engine、AI 执行、防偷懒检查
5 **新增 Quartz 定时任务** → Azure Boards 同步任务、AI 执行任务、AI 任务检查任务
6. **新增 API 控制器** → 提供前端接口，包含手动重新执行
7. **更新配置文件** → 添加 Azure Boards、Copilot CLI、AI 执行配置
8. **前端新增页面和组件** → Kanban 看板可视化，增加重新执行按钮
9. **添加前端路由** → 配置路由
10. **测试和验证** → 端到端走通流程，测试超时重试机制

---

## 6. 结论

基于 Vue.NetCore (Vol.Core) SqlSugar + Vue3 骨架，基础能力（用户认证、权限管理、定时任务、SqlSugar、Vue3、Element Plus）都已经有了。我们只需要增量开发 E-Kanban 业务功能，包含：
- 完整的领域模型
- Azure Boards 集成
- Spec Engine 生成和评估
- GitHub Copilot CLI 调用
- **AI 分批迭代支持**
- **超时检测和自动重试防偷懒机制**
- **手动重新执行支持**

映射表已经清晰列出每个功能点需要创建的文件。
