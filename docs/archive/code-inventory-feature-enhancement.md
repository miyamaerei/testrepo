# 代码资产盘点：E-Kanban 功能扩展 - 多项目支持和细粒度进度跟踪

---

## 1. 相关模块确定

本次需求相关的模块都在 `src/modules/E-Kanban.Backend/` 目录下：

```
src/modules/E-Kanban.Backend/
├── Models/               ✅ 我们要在这里新增三个实体
├── IRepository/          ✅ 新增三个仓储接口
├── Repository/           ✅ 新增三个仓储实现
├── IServices/            ✅ 可能需要更新现有服务接口
├── Services/             ✅ 更新现有服务集成新功能
├── AiExecution/          ✅ 在 AI 执行流程中集成阶段推进
├── Specs/                ✅ 现有 Spec 引擎，不需要改动
├── Jobs/                 ✅ 定时任务，不需要改动
├── DI/                   ✅ 需要添加依赖注入注册
└── Data/                 ✅ 数据库配置
```

---

## 2. 现有文件职责分析

### 2.1 Models 目录现有实体

| 文件 | 职责 | 是否可复用 |
|------|------|------------|
| `BoardWorkItem.cs` | Azure Boards 工作项，存储从外部拉取的工作项 | 可复用，本次不修改 |
| `ExecutionCard.cs` | Kanban 执行卡片，核心聚合根 | ❗ 需要修改，新增 `ProjectRepositoryId` 字段 |
| `ExecutionTask.cs` | 执行定义任务 | 可复用，本次不修改 |
| `ExecutionRun.cs` | 执行事实记录（一次提交） | 可复用，本次不修改 |
| `Spec.cs` | 验收规范定义 | 可复用，本次不修改 |
| `SpecEvaluation.cs` | Spec 评估结果记录 | 可复用，本次不修改 |

**现有实体结构分析：**
- 所有实体都遵循 SqlSugar 规范，使用 `[SugarTable]` 特性
- 主键都是 `int Id` 自增
- 都有 `CreatedAt` 字段，默认 `DateTime.UtcNow`
- 大文本使用 `[SugarColumn(ColumnDataType = "NVARCHAR(MAX)")]`

### 2.2 IRepository / Repository

现有模式：
- 每个实体对应一个接口，继承 `IBaseRepository<T>`
- 每个实体对应一个实现，继承 `BaseRepository<T>`，实现接口
- **可以完全复用这套模式**，新增的三个实体照着做就行

示例参考：`IBoardWorkItemRepository` + `BoardWorkItemRepository`

### 2.3 IServices / Services

现有相关服务：

| 服务 | 职责 | 是否需要修改 |
|------|------|--------------|
| `AiExecutionService.cs` | AI 任务执行入口 | ❗ 需要修改，在执行流程中推进阶段状态 |
| `SubmitService.cs` | 提交执行结果 | ❗ 需要修改，提交时记录文件变更 |
| `StateMachineService.cs` | 状态机流转 | ❗ 需要扩展，支持阶段状态流转 |
| `AzureBoardsClient.cs` | Azure Boards API 集成 | 不需要修改 |
| `AiTaskCheckService.cs` | AI 任务超时检查 | 不需要修改 |
| `SyncService.cs` | 同步工作项 | 不需要修改 |

### 2.4 DI 目录

现有：
- `RepositoryInject.cs` - 仓储依赖注入注册
- `ServiceInject.cs` - 服务依赖注入注册

**需要修改：** 在这两个文件中添加新实体的注册，照着现有写法即可。

### 2.5 AiExecution 目录

| 文件 | 职责 | 是否需要修改 |
|------|------|--------------|
| `AiExecutionService.cs` | AI 执行核心逻辑 | ❗ 需要修改，集成阶段跟踪 |
| `CopilotCliClient.cs` | Copilot CLI 调用封装 | 不需要修改 |
| `IAiExecutionService.cs` | 接口定义 | 不需要修改 |

### 2.6 数据库脚本

现有：
- `database/E-Kanban-NewTables.sql` - 新增表建表 SQL

**需要修改：** 在文件末尾追加三个新表的建表语句。

---

## 3. 可复用的组件/模式

| 组件/模式 | 复用方式 |
|-----------|----------|
| Vol.Core `IBaseRepository<T>` / `BaseRepository<T>` | 新增仓储继承这个基类 |
| SqlSugar 实体映射规范 | 完全复用现有特性标注方式 |
| Vol.Core Autofac 依赖注入 | 完全复用现有注册方式 |
| Github Copilot CLI 集成方式 | 现有集成不需要改动 |
| Quartz 定时任务调度 | 现有调度方式可复用 |
| Spec Engine 验收机制 | 现有机制可复用 |
| 命名规范 | 完全复用 |

---

## 4. 代码风格和命名规范

### 4.1 命名规范

- 类名：PascalCase → `ProjectRepository`
- 方法名：PascalCase → `GetByIdAsync`
- 私有字段：camelCase 下划线前缀 → `private readonly ILogger<ProjectRepositoryService> _logger;`
- 接口名：I 前缀 → `IProjectRepositoryRepository`（虽然有点绕，保持一致）
- 常量：PascalCase

### 4.2 代码结构规范

- 新建实体放在 `Models/` 文件夹
- 新建仓储接口放在 `IRepository/`
- 新建仓储实现放在 `Repository/`
- 服务接口放在 `IServices/`
- 服务实现放在 `Services/`
- DI 注册放在 `DI/` 对应文件

### 4.3 数据库规范

- 表名：复数形式 → `ProjectRepositories` 不是 `ProjectRepository`
- 主键：`Id` → int 自增
- 时间字段：`DateTime` / `DateTime?`，默认 `DateTime.UtcNow`
- 大文本：`[SugarColumn(ColumnDataType = "NVARCHAR(MAX)")]`
- 所有表都要有 `CreatedAt`

### 4.4 Git 提交规范

- 每次提交只改一个功能点
- 格式：`type: 描述`，例如 `feat: 新增 ProjectRepository 实体`

---

## 检查清单（阶段2完成）

- [x] 相关模块已梳理，每个文件职责清晰
- [x] 可复用组件已识别出来
- [x] 项目代码风格和架构模式已理解
- [x] 代码资产盘点已输出
