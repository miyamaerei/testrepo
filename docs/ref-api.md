# E-Kanban 前端对接文档

本文档整理了 E-Kanban 后端提供的所有 API 接口和数据模型，供前端开发对接使用。

## 目录

- [整体介绍](#整体介绍)
- [API 基础信息](#api-基础信息)
- [接口清单](#接口清单)
  - [执行卡片相关](#执行卡片相关)
  - [AI 执行相关](#ai-执行相关)
  - [项目仓库管理](#项目仓库管理)
  - [Azure Boards 同步](#azure-boards-同步)
- [数据模型](#数据模型)
  - [枚举定义](#枚举定义)
  - [实体结构](#实体结构)
- [对接准备清单](#对接准备清单)
- [前后端协作建议](#前后端协作建议)

---

## 整体介绍

E-Kanban 是一个**执行型 Kanban 协调系统**，核心功能：

1. 从 Azure Boards 同步工作项
2. 将工作项转化为 Kanban 卡片
3. AI 自动执行开发任务（基于 GitHub Copilot CLI）
4. Spec 验收标准自动评估
5. 防偷懒机制（超时检测、自动重试）
6. 细粒度进度跟踪和文件变更记录

---

## API 基础信息

- **基础路径**: `/api/ekanban/{controller}/{action}`
- **请求方式**: GET/POST 依接口而定
- **响应格式**: JSON
- **编码**: UTF-8

---

## 接口清单

### 执行卡片相关

#### `GET /api/ekanban/ExecutionCard/GetById`
获取单个执行卡片详情

**参数**:
- `id` (int): 卡片 ID

**响应**:
- 成功: 返回 `ExecutionCard` 对象
- 失败: 404 `{ "message": "Card not found" }`

---

#### `GET /api/ekanban/ExecutionCard/GetKanbanData`
获取看板所有数据，按状态分组

**参数**: 无

**响应**:
```json
{
  "0": [/* New 状态卡片数组 */],
  "1": [/* Ready 状态卡片数组 */],
  "2": [/* InProgress 状态卡片数组 */],
  "3": [/* Submitted 状态卡片数组 */],
  "4": [/* Completed 状态卡片数组 */],
  "5": [/* Failed 状态卡片数组 */]
}
```

---

#### `POST /api/ekanban/ExecutionCard/TriggerReExecute`
重新触发失败的卡片执行

**参数**:
- `id` (int): 卡片 ID

**响应**:
- 成功: `{ "message": "Re-trigger scheduled" }`
- 失败: 404 `{ "message": "Card not found" }`

> 说明：该接口会重置卡片状态为 Ready，等待调度器 picked up 执行

---

### AI 执行相关

#### `POST /api/ekanban/AiExecution/TriggerExecution`
手动触发 AI 执行

**参数**:
- `id` (int): 卡片 ID

**响应**:
- 成功: `{ "message": "Execution started" }`
- 失败: 400 `{ "message": "Card not found" }` 或 `{ "message": "Card is not in Ready state" }`

---

### 项目仓库管理

> 多项目支持功能，管理不同的代码仓库配置

#### `GET /api/ekanban/ProjectRepository/GetAll`
获取所有项目仓库列表

**响应**: 返回 `ProjectRepositories[]` 数组

---

#### `GET /api/ekanban/ProjectRepository/GetById`
获取单个项目仓库详情

**参数**:
- `id` (int): 仓库 ID

**响应**:
- 成功: 返回 `ProjectRepositories` 对象
- 失败: 404 `{ "message": "Project repository not found" }`

---

#### `POST /api/ekanban/ProjectRepository/Create`
创建新项目仓库

**参数**: `ProjectRepositories` JSON 对象 (body)

**响应**: 返回创建后的对象

---

#### `PUT /api/ekanban/ProjectRepository/Update`
更新项目仓库

**参数**: `ProjectRepositories` JSON 对象 (body)

**响应**:
- 成功: `{ "success": true }`
- 失败: 404 `{ "message": "Project repository not found" }`

---

#### `DELETE /api/ekanban/ProjectRepository/Delete`
删除项目仓库

**参数**:
- `id` (int): 仓库 ID

**响应**:
- 成功: `{ "success": true }`
- 失败: 404 `{ "message": "Project repository not found" }`

---

### Azure Boards 同步

#### `POST /api/ekanban/AzureBoardsSync/TriggerSync`
手动触发从 Azure Boards 同步工作项

**响应**: `{ "message": "Sync completed" }`

---

## 数据模型

### 枚举定义

#### ExecutionCardStatus - 卡片状态

| 值 | 名称 | 说明 |
|----|------|------|
| 0 | New | 新建，待就绪 |
| 1 | Ready | 就绪，可以开始执行 |
| 2 | InProgress | 执行中 |
| 3 | Submitted | 已提交，等待 Spec 校验 |
| 4 | Completed | 已完成 |
| 5 | Failed | 失败 |

---

#### ExecutorType - 执行者类型

| 值 | 名称 | 说明 |
|----|------|------|
| 0 | Human | 人工执行 |
| 1 | AI | AI 执行 (GitHub Copilot CLI) |
| 2 | System | 系统自动执行 |

---

#### ChangeType - 文件变更类型

| 值 | 名称 | 说明 |
|----|------|------|
| 0 | Added | 新增文件 |
| 1 | Modified | 修改文件 |
| 2 | Deleted | 删除文件 |

---

#### DevelopmentPhase - 开发阶段（六阶段）

| 值 | 名称 | 说明 |
|----|------|------|
| 1 | RequirementsAnalysis | 需求结构化分析 |
| 2 | CodeInventory | 本地代码资产盘点 |
| 3 | RequirementsCodeMapping | 需求-代码映射 |
| 4 | IncrementalDevelopment | 增量开发实施 |
| 5 | VerificationTesting | 验证测试 |
| 6 | KnowledgeSettle | 知识沉淀 |

---

#### PhaseStatus - 阶段状态

| 值 | 名称 | 说明 |
|----|------|------|
| 0 | NotStarted | 未开始 |
| 1 | InProgress | 进行中 |
| 2 | Completed | 已完成 |

---

### 实体结构

#### ExecutionCard - 执行卡片

```csharp
public class ExecutionCard
{
    public int Id { get; set; }                          // 主键 ID
    public int BoardWorkItemId { get; set; }            // 关联 Azure Boards 工作项 ID
    public string BoardId { get; set; }                 // 所属 Board ID
    public string Title { get; set; }                   // 卡片标题
    public string? Description { get; set; }            // 卡片描述
    public ExecutionCardStatus Status { get; set; }     // 当前状态
    public ExecutorType ExecutorType { get; set; }      // 执行者类型
    public DateTime LastUpdated { get; set; }           // 最后更新时间
    public DateTime CreatedAt { get; set; }             // 创建时间
    public int? SpecId { get; set; }                    // 关联的 Spec ID
    public int FailureCount { get; set; }               // 失败重试次数
    public bool NeedsManualIntervention { get; set; }   // 是否需要人工干预
    public DateTime? InProgressStartTime { get; set; }  // 进入执行中开始时间（超时检测用）
    public int? ProjectRepositoriesId { get; set; }     // 关联的项目仓库 ID
}
```

---

#### ProjectRepositories - 项目仓库

```csharp
public class ProjectRepositories
{
    public int Id { get; set; }                          // 主键 ID
    public string Name { get; set; }                    // 项目名称
    public string LocalWorkingDir { get; set; }         // 本地工作目录
    public string GitRemoteUrl { get; set; }            // Git 远程地址
    public string DefaultBranch { get; set; }           // 默认分支，默认 main
    public string? Description { get; set; }            // 项目描述
    public DateTime CreatedAt { get; set; }             // 创建时间
    public DateTime UpdatedAt { get; set; }             // 更新时间
}
```

---

#### Spec - 验收标准

```csharp
public class Spec
{
    public int Id { get; set; }                          // 主键 ID
    public int ExecutionCardId { get; set; }            // 关联的执行卡片 ID
    public string Definition { get; set; }               // Spec 定义（由 AI 生成，Markdown 格式）
    public DateTime CreatedAt { get; set; }             // 创建时间
    public DateTime UpdatedAt { get; set; }             // 最后更新时间
}
```

---

#### ExecutionRun - 执行记录（一次提交）

```csharp
public class ExecutionRun
{
    public int Id { get; set; }                          // 主键 ID
    public int ExecutionTaskId { get; set; }            // 关联的执行任务 ID
    public int ExecutionCardId { get; set; }            // 关联的执行卡片 ID
    public string SubmittedBy { get; set; }              // 提交者
    public DateTime SubmittedAt { get; set; }           // 提交时间
    public string? Evidence { get; set; }               // 执行证据/结果输出
    public int? ExitCode { get; set; }                  // 命令退出码（CLI 执行）
    public long? DurationMs { get; set; }               // 执行耗时（毫秒）
    public DateTime? StartTime { get; set; }            // 执行开始时间
    public DateTime? EndTime { get; set; }              // 执行结束时间
}
```

---

#### TaskFileChange - 任务文件变更记录

```csharp
public class TaskFileChange
{
    public int Id { get; set; }                          // 主键 ID
    public int ExecutionCardId { get; set; }            // 关联的执行卡片 ID
    public string FilePath { get; set; }                // 文件路径（相对项目目录）
    public ChangeType ChangeType { get; set; }          // 变更类型
    public string? CommitHash { get; set; }             // Git 提交哈希
    public DateTime ChangedAt { get; set; }             // 变更时间
}
```

---

#### TaskPhaseProgress - 任务阶段进度跟踪

```csharp
public class TaskPhaseProgress
{
    public int Id { get; set; }                          // 主键 ID
    public int ExecutionCardId { get; set; }            // 关联的执行卡片 ID
    public DevelopmentPhase Phase { get; set; }         // 开发阶段
    public PhaseStatus Status { get; set; }             // 阶段状态
    public string? OutputDocPath { get; set; }          // 阶段输出文档路径
    public string? Logs { get; set; }                   // 阶段日志
    public DateTime? StartedAt { get; set; }            // 开始时间
    public DateTime? CompletedAt { get; set; }          // 完成时间
    public DateTime CreatedAt { get; set; }             // 创建时间
}
```

---

#### BoardWorkItem - Azure Boards 工作项

```csharp
// 存储从 Azure Boards 同步过来的原始工作项数据
public class BoardWorkItem
{
    public int Id { get; set; }                          // 主键 ID
    public int RemoteWorkItemId { get; set; }            // Azure Boards 工作项 ID
    public string Title { get; set; }                   // 工作项标题
    public string? Description { get; set; }            // 工作项描述
    public string State { get; set; }                   // 远程状态
    public string WorkItemType { get; set; }            // 工作项类型
    public DateTime RemoteCreatedAt { get; set; }       // 远程创建时间
    public DateTime RemoteUpdatedAt { get; set; }       // 远程更新时间
    public DateTime SyncedAt { get; set; }               // 同步时间
}
```

---

#### SpecEvaluation - Spec 评估结果

```csharp
public class SpecEvaluation
{
    public int Id { get; set; }                          // 主键 ID
    public int SpecId { get; set; }                      // 关联 Spec ID
    public int ExecutionRunId { get; set; }              // 关联执行记录 ID
    public bool IsPassed { get; set; }                  // 是否通过
    public string EvaluationResult { get; set; }         // 评估结果说明
    public DateTime EvaluatedAt { get; set; }           // 评估时间
}
```

---

## 对接准备清单

给前端开发准备好的内容：

✅ **已完成**

| 项 | 状态 | 说明 |
|----|------|------|
| API 接口清单整理 | ✅ | 所有 E-Kanban 业务接口已整理 |
| 数据模型文档 | ✅ | 所有实体和枚举已说明 |
| URL 路由规则 | ✅ | `/api/ekanban/{controller}/{action}` |
| 项目结构重构 | ✅ | `src/backend/VOL.WebApi` 后端主项目，`src/frontend/vol.web` 前端项目 |

---

## 给前端的开发建议

### 1. 页面结构规划建议

```
/pages/Kanban/
├── KanbanBoard.vue          # 主看板页面（按状态分组展示卡片）
├── CardDetailModal.vue      # 卡片详情弹窗（显示 Spec、执行记录、文件变更）
├── ProjectRepository.vue    # 项目仓库管理页面
└── components/
    ├── KanbanColumn.vue     # 看板列组件
    ├── CardItem.vue         # 卡片项组件
    ├── SpecViewer.vue       # Spec 查看器
    └── FileChangeList.vue  # 文件变更列表
```

### 2. 核心交互流程

1. **进入看板** → 调用 `GetKanbanData` 获取分组数据 → 渲染看板列
2. **点击同步按钮** → 调用 `TriggerSync` 触发同步 → 刷新看板
3. **点击卡片** → 打开详情弹窗 → 显示详情、Spec、执行记录、文件变更
4. **就绪状态卡片** → 显示"触发执行"按钮 → 点击调用 `TriggerExecution`
5. **失败卡片** → 显示"重新执行"按钮 → 点击调用 `TriggerReExecute`
6. **项目仓库管理** → CRUD 四个接口对应增删改查

### 3. 状态色值参考

| 状态 | 建议颜色 |
|------|---------|
| New | #9E9E9E (灰色) |
| Ready | #2196F3 (蓝色) |
| InProgress | #FF9800 (橙色) |
| Submitted | #9C27B0 (紫色) |
| Completed | #4CAF50 (绿色) |
| Failed | #F44336 (红色) |

### 4. 需要注意的点

- `Description`、`Definition`、`Evidence` 等大字段可能是 Markdown 格式，建议使用 Markdown 渲染组件
- 文件变更记录可以按变更类型显示不同颜色（新增绿色、修改黄色、删除红色）
- 阶段进度可以展示六步法每个阶段的完成情况
- 卡片上建议显示：标题、执行者类型、失败次数、是否需要人工干预

---

## 文档更新记录

| 日期 | 更新内容 |
|------|---------|
| 2026-04-04 | 初始版本，整理了所有 API 接口和数据模型 |

---

## 联系

有问题请随时沟通 👍
