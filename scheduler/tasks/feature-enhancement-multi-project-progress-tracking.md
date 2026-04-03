# 执行任务：E-Kanban 功能扩展 - 多项目支持和细粒度进度跟踪

## 任务说明

根据用户需求，为 E-Kanban 项目添加以下功能：

1. **多项目支持**：支持每个任务指派到不同的项目仓库，配置本地工作目录和 Git 地址
2. **细粒度进度跟踪**：跟踪任务在 S/E 六阶段中每个阶段的进度
3. **文件变更跟踪**：记录每个任务新增/修改/删除了哪些文件

## 开发流程

严格按照以下文档流程执行：
- `docs/NEW_AI_ONBOARDING_GUIDE.md` - 新 AI 上手引导
- `docs/requirements-structured-feature-enhancement.md` - 已完成 需求结构化分析
- `docs/code-inventory-feature-enhancement.md` - 已完成 代码资产盘点
- `docs/requirements-code-mapping-feature-enhancement.md` - 已完成 需求代码映射

**你只需要执行 阶段4：增量开发实施 → 阶段5：验证测试 → 阶段6：知识沉淀**

## 开发要求

1. 严格遵循现有代码风格和命名规范
2. 每次只提交一个功能点，小步快跑
3. 保持对现有代码的兼容
4. 必须编译通过（`dotnet build E-Kanban.sln` 零错误）
5. 更新数据库建表脚本
6. 更新 README.md

## 具体要做的修改列表

按顺序完成：

### 批次 1：新增实体和仓储

1. ✅ `src/modules/E-Kanban.Backend/Models/ProjectRepository.cs` - 新增实体
   ```csharp
   字段：
   - int Id (主键自增)
   - string Name - 项目名称
   - string LocalWorkingDir - 本地工作目录
   - string GitRemoteUrl - Git 远程地址
   - string DefaultBranch - 默认分支（默认 main）
   - string? Description - 描述
   - DateTime CreatedAt - 默认 DateTime.UtcNow
   - DateTime UpdatedAt - 默认 DateTime.UtcNow
   ```

2. ✅ `src/modules/E-Kanban.Backend/Models/TaskPhaseProgress.cs` - 新增实体
   ```csharp
   枚举：
   public enum DevelopmentPhase
   {
       RequirementsAnalysis = 1,
       CodeInventory = 2,
       RequirementsCodeMapping = 3,
       IncrementalDevelopment = 4,
       VerificationTesting = 5,
       KnowledgeSettle = 6
   }
   
   public enum PhaseStatus
   {
       NotStarted = 0,
       InProgress = 1,
       Completed = 2
   }

   字段：
   - int Id (主键自增)
   - int ExecutionCardId - 关联任务卡片
   - DevelopmentPhase Phase - 开发阶段
   - PhaseStatus Status - 阶段状态
   - string? OutputDocPath - 阶段输出文档路径
   - string? Logs - 阶段日志
   - DateTime? StartedAt - 开始时间
   - DateTime? CompletedAt - 完成时间
   - DateTime CreatedAt - 默认 DateTime.UtcNow
   ```

3. ✅ `src/modules/E-Kanban.Backend/Models/TaskFileChange.cs` - 新增实体
   ```csharp
   枚举：
   public enum ChangeType
   {
       Added = 0,
       Modified = 1,
       Deleted = 2
   }

   字段：
   - int Id (主键自增)
   - int ExecutionCardId - 关联任务卡片
   - string FilePath - 文件路径（相对项目目录）
   - ChangeType ChangeType - 变更类型
   - string? CommitHash - Git commit hash
   - DateTime ChangedAt - 默认 DateTime.UtcNow
   ```

4. ✅ `src/modules/E-Kanban.Backend/Models/ExecutionCard.cs` - 修改，新增：
   ```csharp
   public int? ProjectRepositoryId { get; set; }
   ```

5. ✅ 新增三个仓储接口到 `IRepository/`：
   - `IProjectRepositoryRepository.cs`
   - `ITaskPhaseProgressRepository.cs`
   - `ITaskFileChangeRepository.cs`

6. ✅ 新增三个仓储实现到 `Repository/`：
   - `ProjectRepositoryRepository.cs`
   - `TaskPhaseProgressRepository.cs`
   - `TaskFileChangeRepository.cs`

7. ✅ 更新 `DI/RepositoryInject.cs` 添加依赖注入注册

### 批次 2：更新服务集成

1. ✅ 在 `AiExecutionService.cs` 中，当开始执行时，自动推进到对应阶段，更新阶段状态为 InProgress
2. ✅ 在 `SubmitService.cs` 中，完成执行后，更新阶段状态为 Completed，如果还有下一阶段自动进入下一阶段
3. ✅ （可选）在 `SubmitService` 中，如果有文件变更，记录到 `TaskFileChange`

### 批次 3：收尾

1. ✅ 更新 `database/E-Kanban-NewTables.sql` 添加三个新表的建表 SQL
2. ✅ 更新项目根目录 `README.md` 说明新增功能
3. ✅ 编译验证 `dotnet build E-Kanban.sln`，修复所有错误
4. ✅ 提交代码到远程分支

## Git 操作

- 从 `master` 新开分支：`feature/multi-project-progress-tracking`
- 每个功能点单独提交
- 完成后推送到远程

## 验收标准

- `dotnet build E-Kanban.sln` 编译零错误
- 所有新增实体遵循现有规范
- 数据库脚本正确
- 文档已更新

## 完成后

本次任务完成，等待用户合并。
