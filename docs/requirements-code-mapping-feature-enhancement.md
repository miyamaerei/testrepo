# 需求-代码映射表：E-Kanban 功能扩展 - 多项目支持和细粒度进度跟踪

---

## 映射总表

| 功能点 | 当前状态 | 操作类型 | 需要修改/新增的文件 | 依赖模块 |
|--------|----------|----------|-------------------|----------|
| 1. 新增 ProjectRepository 实体 | 无 | 新增 | `Models/ProjectRepository.cs` | IBaseRepository |
| 1. 新增 ProjectRepository 仓储接口 | 无 | 新增 | `IRepository/IProjectRepositoryRepository.cs` | IBaseRepository |
| 1. 新增 ProjectRepository 仓储实现 | 无 | 新增 | `Repository/ProjectRepositoryRepository.cs` | BaseRepository |
| 2. 新增 TaskPhaseProgress 实体 | 无 | 新增 | `Models/TaskPhaseProgress.cs` | IBaseRepository |
| 2. 新增 TaskPhaseProgress 仓储接口 | 无 | 新增 | `IRepository/ITaskPhaseProgressRepository.cs` | IBaseRepository |
| 2. 新增 TaskPhaseProgress 仓储实现 | 无 | 新增 | `Repository/TaskPhaseProgressRepository.cs` | BaseRepository |
| 3. 新增 TaskFileChange 实体 | 无 | 新增 | `Models/TaskFileChange.cs` | IBaseRepository |
| 3. 新增 TaskFileChange 仓储接口 | 无 | 新增 | `IRepository/ITaskFileChangeRepository.cs` | IBaseRepository |
| 3. 新增 TaskFileChange 仓储实现 | 无 | 新增 | `Repository/TaskFileChangeRepository.cs` | BaseRepository |
| 4. 修改 ExecutionCard 实体添加 ProjectRepositoryId | 部分已有 | 修改 | `Models/ExecutionCard.cs` | 无 |
| 5. 更新 DI 注册 - 仓储 | 已有 | 修改 | `DI/RepositoryInject.cs` | 现有 Autofac 注册模式 |
| 6. 更新 DI 注册 - 服务 | 已有 | 修改 | `DI/ServiceInject.cs` | 现有 Autofac 注册模式 |
| 7. 更新 AiExecutionService 集成阶段跟踪 | 已有 | 修改 | `AiExecution/AiExecutionService.cs` | IExecutionCardRepository, IStateMachineService |
| 8. 更新 SubmitService 记录文件变更 | 已有 | 修改 | `Services/SubmitService.cs` | IExecutionRunRepository |
| 9. 添加新建表数据库脚本 | 无 | 修改 | `database/E-Kanban-NewTables.sql` | 无 |
| 10. 更新 README.md 说明新功能 | 已有 | 修改 | `../README.md` | 无 |

---

## 开发顺序（按依赖排序）

1. **第一批次**（无依赖，先做）：
   - 新增三个实体：ProjectRepository、TaskPhaseProgress、TaskFileChange
   - 新增三个仓储接口和实现
   - 修改 ExecutionCard 实体
   - 更新 DI 注册

2. **第二批次**（依赖第一批次）：
   - 更新 AiExecutionService 集成阶段跟踪
   - 更新 SubmitService 记录文件变更

3. **第三批次**：
   - 添加数据库建表脚本
   - 更新 README.md
   - 编译验证

---

## 依赖关系说明

- 新增实体依赖 Vol.Core 的 IBaseRepository / BaseRepository 基类（已存在）
- 服务修改依赖新增的仓储（所以必须先做第一批次）
- 不影响现有 Azure Boards 同步、Spec Engine、定时任务等核心功能

---

## 检查清单（阶段3完成）

- [x] 每个功能点都分析了现有实现情况
- [x] 每个功能点都确定了操作类型
- [x] 依赖关系已清晰识别
- [x] 修改范围已确定
- [x] 需求代码映射表已输出
