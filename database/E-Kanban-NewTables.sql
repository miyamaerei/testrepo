-- =========================================
-- E-Kanban 新增表 SQL 脚本 (SQL Server)
-- 基于 Vol.Core 基础框架，新增 E-Kanban 业务表
-- =========================================

USE [E-Kanban] -- 请修改为你的数据库名
GO

-- =========================================
-- 1. BoardWorkItems - Azure Boards 工作项表
-- 存储从 Azure Boards 同步过来的工作项
-- =========================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='BoardWorkItems' AND xtype='U')
BEGIN
CREATE TABLE [dbo].[BoardWorkItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExternalWorkItemId] [int] NOT NULL,
	[BoardId] [nvarchar](100) NOT NULL,
	[Title] [nvarchar](500) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[ExternalState] [nvarchar](100) NOT NULL,
	[LastSyncedAt] [datetime2](7) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_BoardWorkItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

-- 创建索引加快查询
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_BoardWorkItems_BoardId' AND object_id = OBJECT_ID('BoardWorkItems'))
CREATE NONCLUSTERED INDEX [IX_BoardWorkItems_BoardId] ON [dbo].[BoardWorkItems] ([BoardId])
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_BoardWorkItems_ExternalWorkItemId' AND object_id = OBJECT_ID('BoardWorkItems'))
CREATE NONCLUSTERED INDEX [IX_BoardWorkItems_ExternalWorkItemId] ON [dbo].[BoardWorkItems] ([ExternalWorkItemId])
GO

-- =========================================
-- 2. ExecutionCards - Kanban 执行卡片表
-- 核心表，每个工作项对应一个执行卡片，跟踪执行状态
-- =========================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ExecutionCards' AND xtype='U')
BEGIN
CREATE TABLE [dbo].[ExecutionCards](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BoardWorkItemId] [int] NOT NULL,
	[BoardId] [nvarchar](100) NOT NULL,
	[Title] [nvarchar](500) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Status] [int] NOT NULL, -- 0=New 1=Ready 2=InProgress 3=Submitted 4=Completed 5=Failed
	[ExecutorType] [int] NOT NULL, -- 0=Human 1=AI 2=System
	[LastUpdated] [datetime2](7) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[SpecId] [int] NULL,
	[FailureCount] [int] NOT NULL DEFAULT 0,
	[NeedsManualIntervention] [bit] NOT NULL DEFAULT 0,
	[InProgressStartTime] [datetime2](7) NULL,
 CONSTRAINT [PK_ExecutionCards] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

-- 索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_ExecutionCards_BoardId' AND object_id = OBJECT_ID('ExecutionCards'))
CREATE NONCLUSTERED INDEX [IX_ExecutionCards_BoardId] ON [dbo].[ExecutionCards] ([BoardId])
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_ExecutionCards_Status' AND object_id = OBJECT_ID('ExecutionCards'))
CREATE NONCLUSTERED INDEX [IX_ExecutionCards_Status] ON [dbo].[ExecutionCards] ([Status])
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_ExecutionCards_BoardWorkItemId' AND object_id = OBJECT_ID('ExecutionCards'))
CREATE NONCLUSTERED INDEX [IX_ExecutionCards_BoardWorkItemId] ON [dbo].[ExecutionCards] ([BoardWorkItemId])
GO

-- =========================================
-- 3. Specs - 验收标准规范表
-- 存储 AI 生成的验收标准
-- =========================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Specs' AND xtype='U')
BEGIN
CREATE TABLE [dbo].[Specs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExecutionCardId] [int] NOT NULL,
	[Definition] [nvarchar](max) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Specs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Specs_ExecutionCardId' AND object_id = OBJECT_ID('Specs'))
CREATE NONCLUSTERED INDEX [IX_Specs_ExecutionCardId] ON [dbo].[Specs] ([ExecutionCardId])
GO

-- =========================================
-- 4. SpecEvaluations - Spec 评估记录表
-- 存储每次 Spec 评估结果
-- =========================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SpecEvaluations' AND xtype='U')
BEGIN
CREATE TABLE [dbo].[SpecEvaluations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExecutionRunId] [int] NOT NULL,
	[SpecId] [int] NOT NULL,
	[Result] [int] NOT NULL, -- 0=Passed 1=Failed
	[Message] [nvarchar](max) NULL,
	[EvaluatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_SpecEvaluations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_SpecEvaluations_ExecutionRunId' AND object_id = OBJECT_ID('SpecEvaluations'))
CREATE NONCLUSTERED INDEX [IX_SpecEvaluations_ExecutionRunId] ON [dbo].[SpecEvaluations] ([ExecutionRunId])
GO

-- =========================================
-- 5. ExecutionTasks - 执行定义任务表
-- 一个 ExecutionCard 可能拆分成多个执行任务
-- =========================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ExecutionTasks' AND xtype='U')
BEGIN
CREATE TABLE [dbo].[ExecutionTasks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExecutionCardId] [int] NOT NULL,
	[ExecutorType] [int] NOT NULL,
	[ExecutionInstructions] [nvarchar](max) NULL,
	[IsEnabled] [bit] NOT NULL DEFAULT 1,
	[CreatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_ExecutionTasks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_ExecutionTasks_ExecutionCardId' AND object_id = OBJECT_ID('ExecutionTasks'))
CREATE NONCLUSTERED INDEX [IX_ExecutionTasks_ExecutionCardId] ON [dbo].[ExecutionTasks] ([ExecutionCardId])
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_ExecutionTasks_IsEnabled' AND object_id = OBJECT_ID('ExecutionTasks'))
CREATE NONCLUSTERED INDEX [IX_ExecutionTasks_IsEnabled] ON [dbo].[ExecutionTasks] ([IsEnabled])
GO

-- =========================================
-- 6. ExecutionRuns - 执行事实记录表
-- 存储每次执行/提交的历史记录
-- =========================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ExecutionRuns' AND xtype='U')
BEGIN
CREATE TABLE [dbo].[ExecutionRuns](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExecutionTaskId] [int] NOT NULL,
	[ExecutionCardId] [int] NOT NULL,
	[SubmittedBy] [nvarchar](100) NOT NULL,
	[SubmittedAt] [datetime2](7) NOT NULL,
	[Evidence] [nvarchar](max) NULL,
	[ExitCode] [int] NULL,
	[DurationMs] [bigint] NULL,
	[StartTime] [datetime2](7) NULL,
	[EndTime] [datetime2](7) NULL,
 CONSTRAINT [PK_ExecutionRuns] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_ExecutionRuns_ExecutionCardId' AND object_id = OBJECT_ID('ExecutionRuns'))
CREATE NONCLUSTERED INDEX [IX_ExecutionRuns_ExecutionCardId] ON [dbo].[ExecutionRuns] ([ExecutionCardId])
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_ExecutionRuns_ExecutionTaskId' AND object_id = OBJECT_ID('ExecutionRuns'))
CREATE NONCLUSTERED INDEX [IX_ExecutionRuns_ExecutionTaskId] ON [dbo].[ExecutionRuns] ([ExecutionTaskId])
GO

-- =========================================
-- 7. ProjectRepositories - 项目仓库表
-- 存储多项目配置信息，支持多项目管理
-- =========================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ProjectRepositories' AND xtype='U')
BEGIN
CREATE TABLE [dbo].[ProjectRepositories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[LocalWorkingDir] [nvarchar](500) NOT NULL,
	[GitRemoteUrl] [nvarchar](500) NOT NULL,
	[DefaultBranch] [nvarchar](100) NOT NULL DEFAULT 'main',
	[Description] [nvarchar](max) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_ProjectRepositories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

-- 索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_ProjectRepositories_Name' AND object_id = OBJECT_ID('ProjectRepositories'))
CREATE NONCLUSTERED INDEX [IX_ProjectRepositories_Name] ON [dbo].[ProjectRepositories] ([Name])
GO

-- =========================================
-- 8. TaskPhaseProgress - 任务阶段进度跟踪表
-- 跟踪每个任务在 S/E 六阶段中的进度状态
-- =========================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TaskPhaseProgress' AND xtype='U')
BEGIN
CREATE TABLE [dbo].[TaskPhaseProgress](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExecutionCardId] [int] NOT NULL,
	[Phase] [int] NOT NULL, -- 1=需求分析 2=代码盘点 3=需求映射 4=增量开发 5=验证测试 6=知识沉淀
	[Status] [int] NOT NULL, -- 0=未开始 1=进行中 2=已完成
	[OutputDocPath] [nvarchar](500) NULL,
	[Logs] [nvarchar](max) NULL,
	[StartedAt] [datetime2](7) NULL,
	[CompletedAt] [datetime2](7) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_TaskPhaseProgress] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

-- 索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_TaskPhaseProgress_ExecutionCardId' AND object_id = OBJECT_ID('TaskPhaseProgress'))
CREATE NONCLUSTERED INDEX [IX_TaskPhaseProgress_ExecutionCardId] ON [dbo].[TaskPhaseProgress] ([ExecutionCardId])
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_TaskPhaseProgress_Phase' AND object_id = OBJECT_ID('TaskPhaseProgress'))
CREATE NONCLUSTERED INDEX [IX_TaskPhaseProgress_Phase] ON [dbo].[TaskPhaseProgress] ([Phase])
GO

-- =========================================
-- 9. TaskFileChanges - 任务文件变更记录表
-- 记录每个任务新增/修改/删除了哪些文件
-- =========================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TaskFileChanges' AND xtype='U')
BEGIN
CREATE TABLE [dbo].[TaskFileChanges](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExecutionCardId] [int] NOT NULL,
	[FilePath] [nvarchar](1000) NOT NULL,
	[ChangeType] [int] NOT NULL, -- 0=新增 1=修改 2=删除
	[CommitHash] [nvarchar](100) NULL,
	[ChangedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_TaskFileChanges] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

-- 索引
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_TaskFileChanges_ExecutionCardId' AND object_id = OBJECT_ID('TaskFileChanges'))
CREATE NONCLUSTERED INDEX [IX_TaskFileChanges_ExecutionCardId] ON [dbo].[TaskFileChanges] ([ExecutionCardId])
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_TaskFileChanges_ChangeType' AND object_id = OBJECT_ID('TaskFileChanges'))
CREATE NONCLUSTERED INDEX [IX_TaskFileChanges_ChangeType] ON [dbo].[TaskFileChanges] ([ChangeType])
GO

-- =========================================
-- 增量修改：为 ExecutionCards 表添加 ProjectRepositoryId 字段
-- =========================================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ExecutionCards' AND COLUMN_NAME = 'ProjectRepositoryId')
BEGIN
	ALTER TABLE [dbo].[ExecutionCards] ADD [ProjectRepositoryId] [int] NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_ExecutionCards_ProjectRepositoryId' AND object_id = OBJECT_ID('ExecutionCards'))
CREATE NONCLUSTERED INDEX [IX_ExecutionCards_ProjectRepositoryId] ON [dbo].[ExecutionCards] ([ProjectRepositoryId])
GO

-- =========================================
-- 完成说明
-- =========================================
PRINT 'E-Kanban 新增表创建完成！'
PRINT ''
PRINT '新增表汇总：'
PRINT '  1. BoardWorkItems         - Azure Boards 工作项'
PRINT '  2. ExecutionCards         - Kanban 执行卡片（核心表）'
PRINT '  3. Specs                  - 验收标准规范'
PRINT '  4. SpecEvaluations        - Spec 评估记录'
PRINT '  5. ExecutionTasks         - 执行定义任务'
PRINT '  6. ExecutionRuns          - 执行事实记录'
PRINT '  7. ProjectRepositories    - 项目仓库配置（多项目支持）'
PRINT '  8. TaskPhaseProgress      - 任务阶段进度跟踪'
PRINT '  9. TaskFileChanges        - 任务文件变更记录'
PRINT ''
PRINT '所有表都已添加必要的主键和索引，可直接使用。'
GO
