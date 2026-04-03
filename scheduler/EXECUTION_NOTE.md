# 执行注意事项 - 给接手这个任务的新 AI

欢迎接手 E-Kanban 功能扩展开发任务！这里给你一些关键注意事项，帮助你顺利完成。

---

## 🔑 关键信息

### 项目背景

这是 **E-Kanban = 执行型 Kanban 协调系统** 项目，核心是基于 Azure Boards + GitHub Copilot CLI 实现人机协同开发，AI 自动执行开发任务。

当前你接手的是**功能扩展需求**，项目主体已经开发完成，编译通过，现在需要新增：多项目支持 + 细粒度进度跟踪。

### 你要做什么

按顺序完成以下开发任务：

1. **批次 1：新增实体和仓储**
   - `Models/ProjectRepository.cs` - 项目仓库实体，存储项目配置
   - `Models/TaskPhaseProgress.cs` - 开发阶段进度跟踪实体
   - `Models/TaskFileChange.cs` - 文件变更记录实体
   - 修改 `Models/ExecutionCard.cs` 新增 `ProjectRepositoryId` 字段
   - 新增三个仓储接口（`IRepository/`）和实现（`Repository/`）
   - 更新 `DI/RepositoryInject.cs` 添加依赖注入注册

2. **批次 2：更新服务集成**
   - 更新 `AiExecution/AiExecutionService.cs` - 开始执行时推进阶段状态
   - 更新 `Services/SubmitService.cs` - 完成后更新阶段状态，记录文件变更

3. **批次 3：收尾工作**
   - 更新 `database/E-Kanban-NewTables.sql` 添加三个新表的建表 SQL
   - 更新项目根目录 `README.md` 说明新增功能
   - 编译验证 `dotnet build E-Kanban.sln`，修复所有错误
   - 提交代码到 `feature/multi-project-progress-tracking` 分支

---

## ⚠️ 必须遵守的规则（非常重要！）

### 1. 严格遵循现有代码风格

**命名规范：**
- 类名：PascalCase → `public class ProjectRepository`
- 方法名：PascalCase → `public Task<ProjectRepository> GetByIdAsync()`
- 私有字段：camelCase 下划线前缀 → `private readonly ILogger<ProjectRepositoryService> _logger;`
- 接口名：I 前缀 → `IProjectRepositoryRepository`
- 常量：PascalCase

**代码结构：**
- 实体放 `Models/`
- 仓储接口放 `IRepository/`
- 仓储实现放 `Repository/`
- 依赖注入注册在 `DI/RepositoryInject.cs`
- 完全照着现有写法复制就行！

### 2. 数据库规范

- 表名：复数形式 → `ProjectRepositories`（不是 `ProjectRepository`）
- 主键：`Id` → int 自增，加 `[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]`
- 时间字段：`DateTime` / `DateTime?`，默认值 `DateTime.UtcNow`
- 大文本：加 `[SugarColumn(ColumnDataType = "NVARCHAR(MAX)")]`
- 所有表必须有 `CreatedAt` 字段

去看看现有的 `BoardWorkItem.cs` 或者 `ExecutionCard.cs`，**完全照着那个写法来！**

### 3. 小步快跑，每次一个提交

- **不要一次性修改很多文件！**
- 每写完一个实体 + 仓储就提交一次
- 提交信息格式：`feat: 新增 ProjectRepository 实体和仓储`
- 每次提交后编译验证，确保没问题再继续

### 4. 保持兼容

- 不要删除现有代码，除非确定不需要了
- 不要修改现有接口的签名，保持兼容
- 如果需要加字段，直接加就行，不要改原有字段

### 5. 编译必须零错误

每次写完一个批次都要运行：
```bash
dotnet build E-Kanban.sln
```
必须**零错误**才能继续，有错误马上修复。

---

## 📋 已经帮你做好了哪些前置工作

三个前置阶段已经完成，不需要你再做了：

- ✅ 阶段1：需求结构化分析 → `docs/requirements-structured-feature-enhancement.md`
- ✅ 阶段2：代码资产盘点 → `docs/code-inventory-feature-enhancement.md`
- ✅ 阶段3：需求-代码映射 → `docs/requirements-code-mapping-feature-enhancement.md`

你只需要从**阶段4：增量开发实施**开始做就行。

---

## 🔍 如果遇到问题怎么办

1. **编译错误** → 仔细读错误信息，定位文件行号，用 Copilot 分析：
   ```bash
   copilot -p "编译出现这个错误：
   【粘贴错误信息】
   
   相关代码：
   【粘贴代码】
   
   帮我分析原因，怎么修复？" --allow-all-tools
   ```

2. **不确定写法** → 找一个现有的类似文件照着写！比如看 `BoardWorkItem` 怎么写，你就怎么写。

3. **需求不清晰** → 回头读：
   - `docs/requirements-structured-feature-enhancement.md`
   - 这个文件你现在正在读的 `scheduler/tasks/feature-enhancement-multi-project-progress-tracking.md`

4. **还是不清楚** → 停止执行，反馈给用户澄清。不要猜！

---

## ✅ 完成验收标准

全部完成后需要满足：

- [ ] `dotnet build E-Kanban.sln` 编译零错误
- [ ] 三个新实体都已创建，遵循规范
- [ ] 三个新仓储都已创建，DI 已注册
- [ ] ExecutionCard 已添加 ProjectRepositoryId 字段
- [ ] AiExecutionService 已更新集成阶段跟踪
- [ ] SubmitService 已更新记录文件变更
- [ ] 数据库建表脚本已添加到 `database/E-Kanban-NewTables.sql`
- [ ] README.md 已更新说明新功能
- [ ] 所有修改都已提交到 `feature/multi-project-progress-tracking` 分支并推送到远程

---

## 💡 一句话总结

**照着现有代码的写法，按批次一个一个来，每次编译通过，小步提交。**

祝你开发顺利！🚀

---

文档版本：v1.0  
创建日期：2026-04-03  
创建人：Claude
