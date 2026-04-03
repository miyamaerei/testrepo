# 开发案例总结：多项目支持 + 细粒度进度跟踪 需求实现全过程

> 这个案例展示了如何按照 E-Kanban 的 S/E 方法论，从用户一句话需求，到交付可运行代码的完整过程。

---

## 1. 原始需求

用户原始提问：

> 1，2点可以重构，然后3，我想的是，不只是状态，每一个任务，我都要看到现在进度怎么样了，他在分析步骤还是需求步骤，现在添加了哪些文件之类的这些，或者说有没有任务在跑的日志跟踪，目前有吗？请你再研究代码回答我
>
> 请你把我的需求写一份，然后新开一个分支，把这三个需求，设置一下定时任务，让任务自动执行。请你不要自己执行，而是告诉任务如何做？

拆解后三个具体需求：

1. **需求 1**：支持任务指派到不同项目（每个任务可以对应不同的外部项目）
2. **需求 2**：支持配置项目的本地文件位置和 Git 连接管理
3. **需求 3**：细粒度进度跟踪 - 不仅看到任务整体状态，还能看到：
   - 当前走到 S/E 六阶段的哪一步
   - 该任务当前已经添加/修改了哪些文件
   - 完整的执行日志跟踪

---

## 2. 阶段1：需求结构化分析

**输出文档**：`requirements-structured-feature-enhancement.md`

**产出内容：**
- 完整记录原始需求
- 分析核心目标：解决单项目限制，提供全链路可追溯能力
- 拆解出 5 个具体功能点
- 识别约束条件：保持现有技术栈、兼容、规范
- 确认无模糊点

**关键：结构化让需求从模糊变清晰**，接手的 AI 不需要再猜用户想要什么。

---

## 3. 阶段2：本地代码资产盘点

**输出文档**：`code-inventory-feature-enhancement.md`

**产出内容：**
- 确定哪些模块和本次需求相关
- 梳理每个现有文件的职责
- 找出哪些可以复用（基类、模式、规范）
- 明确哪些文件需要修改
- 总结现有代码风格和命名规范

**关键：让 AI 接手就知道现状**，不用自己重新摸索整个项目。

---

## 4. 阶段3：需求-代码映射

**输出文档**：`requirements-code-mapping-feature-enhancement.md`

**产出内容：**
- 每个功能点对应操作类型（复用/修改/新增）
- 列出每个功能点需要修改/新增哪些文件
- 确定依赖关系
- 给出开发顺序（按依赖排序）

**这一步做完，AI 只需要按顺序敲代码就行**，不用再想"接下来我该做什么"。

---

## 5. 给执行 AI 的注意事项

**输出文档**：`../scheduler/EXECUTION_NOTE.md`

**内容要点：**
- 告诉 AI 它要做什么（把开发顺序再列一遍）
- 强调必须遵守的规则（命名、数据库、Git 提交规范）
- 哪些前置工作已经做完了
- 遇到问题该怎么办（给出现成的 Copilot 提示词模板）
- 给出完成验收标准

**这是给新 AI 的"导航地图"**，就算它第一次接手这个项目也不会迷路。

---

## 6. 任务注册和自动执行

**操作：**
1. 从 `master` 新开分支：`feature/multi-project-progress-tracking`
2. 在 `~/.openclaw/cron/task-registry.json` 注册任务
3. 设置状态为 `PENDING`，优先级 `P0`
4. 通用任务调度器会每 10 分钟检查，自动触发执行

**执行 AI 做了什么：**
- 按批次开发：批次1（三个实体+三个仓储）→ 批次2（服务集成）→ 批次3（收尾）
- 每个功能点单独提交
- 每批次完成编译验证
- 最后更新数据库脚本和 README
- 推送到远程

---

## 7. 最终交付结果

### 新增文件

| 文件 | 说明 |
|------|------|
| `Models/ProjectRepository.cs` | 项目仓库实体，存储项目名称、本地目录、Git 地址、默认分支 |
| `Models/TaskPhaseProgress.cs` | 开发阶段进度跟踪，支持六个阶段每个阶段的状态跟踪 |
| `Models/TaskFileChange.cs` | 文件变更记录，跟踪每个任务新增/修改/删除了哪些文件 |
| `IRepository/IProjectRepositoryRepository.cs` | 仓储接口 |
| `IRepository/ITaskPhaseProgressRepository.cs` | 仓储接口 |
| `IRepository/ITaskFileChangeRepository.cs` | 仓储接口 |
| `Repository/ProjectRepositoryRepository.cs` | 仓储实现 |
| `Repository/TaskPhaseProgressRepository.cs` | 仓储实现 |
| `Repository/TaskFileChangeRepository.cs` | 仓储实现 |

### 修改文件

| 文件 | 修改内容 |
|------|----------|
| `Models/ExecutionCard.cs` | 新增 `ProjectRepositoryId` 字段，关联项目 |
| `AiExecution/AiExecutionService.cs` | 开始执行时初始化所有六个阶段，第一个阶段设为 InProgress |
| `Services/SubmitService.cs` | 提交完成后，当前阶段标记为 Completed，自动推进到下一阶段 |
| `database/E-Kanban-NewTables.sql` | 新增三个表的建表 SQL，新增字段 ALTER TABLE |
| `README.md` | 更新项目功能说明和目录结构 |

### 数据库新增表

```sql
-- 项目仓库表
CREATE TABLE ProjectRepositories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    LocalWorkingDir NVARCHAR(500) NOT NULL,
    GitRemoteUrl NVARCHAR(500) NOT NULL,
    DefaultBranch NVARCHAR(100) NOT NULL DEFAULT 'main',
    Description NVARCHAR(MAX) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
);

-- 任务阶段进度表
CREATE TABLE TaskPhaseProgress (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ExecutionCardId INT NOT NULL,
    Phase INT NOT NULL,
    Status INT NOT NULL,
    OutputDocPath NVARCHAR(500) NULL,
    Logs NVARCHAR(MAX) NULL,
    StartedAt DATETIME NULL,
    CompletedAt DATETIME NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
);

-- 文件变更记录表
CREATE TABLE TaskFileChanges (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ExecutionCardId INT NOT NULL,
    FilePath NVARCHAR(500) NOT NULL,
    ChangeType INT NOT NULL,
    CommitHash NVARCHAR(100) NULL,
    ChangedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
);

-- 给 ExecutionCards 新增字段
ALTER TABLE ExecutionCards ADD ProjectRepositoryId INT NULL;
```

---

## 8. 关键经验总结

### 为什么要分这六个阶段？

- **结构化先行**：避免 AI 上来就写代码，写到一半才发现理解错需求
- **充分盘点**：让 AI 知道现有哪些可以复用，避免重复造轮子
- **明确映射**：AI 只需要按清单干活，不用自己决定该改哪里
- **小步快跑**：每次一个功能点，编译通过再继续，问题容易定位
- **验证测试**：保证质量
- **知识沉淀**：更新文档，下次接手更快

### 给新 AI 哪些关键指引？

1. **告诉它规则**：命名、数据库、Git 规范，copy 就行
2. **给出现成示例**：照着现有代码写
3. **给出问题解决方法**：遇到错误该怎么找 Copilot 帮忙
4. **给出验收标准**：做到什么程度算完成

### 自动化价值

- 前置分析工作由人（或主 AI）完成
- 执行 AI 只需要专注编码
- 定时调度保证不会"偷懒"，超时会自动检查
- 全流程可追溯，每个阶段都有文档

---

## 9. 验收结果

✅ `dotnet build E-Kanban.sln` → **0 错误**  
✅ 所有功能点已完成  
✅ 遵循项目规范  
✅ 文档已更新  
✅ 推送到远程分支：`feature/multi-project-progress-tracking`

---

**案例创建日期**：2026-04-03  
**创建人**：Claude  
**基于 S/E 方法论**
