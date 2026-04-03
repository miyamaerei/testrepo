-- =========================================
-- E-Kanban 新增表 SQL 脚本 (PostgreSQL)
-- 基于 Vol.Core 基础框架，新增 E-Kanban 业务表
-- 请注意：请根据实际情况修改 schema
-- =========================================

-- =========================================
-- 1. BoardWorkItems - Azure Boards 工作项表
-- 存储从 Azure Boards 同步过来的工作项
-- =========================================
CREATE TABLE IF NOT EXISTS board_work_items (
    id SERIAL PRIMARY KEY,
    external_work_item_id INTEGER NOT NULL,
    board_id VARCHAR(100) NOT NULL,
    title VARCHAR(500) NOT NULL,
    description TEXT NULL,
    external_state VARCHAR(100) NOT NULL,
    last_synced_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- 创建索引加快查询
CREATE INDEX IF NOT EXISTS idx_board_work_items_board_id ON board_work_items(board_id);
CREATE INDEX IF NOT EXISTS idx_board_work_items_external_work_item_id ON board_work_items(external_work_item_id);

-- =========================================
-- 2. ExecutionCards - Kanban 执行卡片表
-- 核心表，每个工作项对应一个执行卡片，跟踪执行状态
-- =========================================
CREATE TABLE IF NOT EXISTS execution_cards (
    id SERIAL PRIMARY KEY,
    board_work_item_id INTEGER NOT NULL,
    board_id VARCHAR(100) NOT NULL,
    title VARCHAR(500) NOT NULL,
    description TEXT NULL,
    status INTEGER NOT NULL, -- 0=New 1=Ready 2=InProgress 3=Submitted 4=Completed 5=Failed
    executor_type INTEGER NOT NULL, -- 0=Human 1=AI 2=System
    last_updated TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    spec_id INTEGER NULL,
    failure_count INTEGER NOT NULL DEFAULT 0,
    needs_manual_intervention BOOLEAN NOT NULL DEFAULT FALSE,
    in_progress_start_time TIMESTAMP WITH TIME ZONE NULL,
    project_repository_id INTEGER NULL
);

-- 索引
CREATE INDEX IF NOT EXISTS idx_execution_cards_board_id ON execution_cards(board_id);
CREATE INDEX IF NOT EXISTS idx_execution_cards_status ON execution_cards(status);
CREATE INDEX IF NOT EXISTS idx_execution_cards_board_work_item_id ON execution_cards(board_work_item_id);
CREATE INDEX IF NOT EXISTS idx_execution_cards_project_repository_id ON execution_cards(project_repository_id);

-- =========================================
-- 3. Specs - 验收标准规范表
-- 存储 AI 生成的验收标准
-- =========================================
CREATE TABLE IF NOT EXISTS specs (
    id SERIAL PRIMARY KEY,
    execution_card_id INTEGER NOT NULL,
    definition TEXT NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_specs_execution_card_id ON specs(execution_card_id);

-- =========================================
-- 4. SpecEvaluations - Spec 评估记录表
-- 存储每次 Spec 评估结果
-- =========================================
CREATE TABLE IF NOT EXISTS spec_evaluations (
    id SERIAL PRIMARY KEY,
    execution_run_id INTEGER NOT NULL,
    spec_id INTEGER NOT NULL,
    result INTEGER NOT NULL, -- 0=Passed 1=Failed
    message TEXT NULL,
    evaluated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_spec_evaluations_execution_run_id ON spec_evaluations(execution_run_id);

-- =========================================
-- 5. ExecutionTasks - 执行定义任务表
-- 一个 ExecutionCard 可能拆分成多个执行任务
-- =========================================
CREATE TABLE IF NOT EXISTS execution_tasks (
    id SERIAL PRIMARY KEY,
    execution_card_id INTEGER NOT NULL,
    executor_type INTEGER NOT NULL,
    execution_instructions TEXT NULL,
    is_enabled BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_execution_tasks_execution_card_id ON execution_tasks(execution_card_id);
CREATE INDEX IF NOT EXISTS idx_execution_tasks_is_enabled ON execution_tasks(is_enabled);

-- =========================================
-- 6. ExecutionRuns - 执行事实记录表
-- 存储每次执行/提交的历史记录
-- =========================================
CREATE TABLE IF NOT EXISTS execution_runs (
    id SERIAL PRIMARY KEY,
    execution_task_id INTEGER NOT NULL,
    execution_card_id INTEGER NOT NULL,
    submitted_by VARCHAR(100) NOT NULL,
    submitted_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    evidence TEXT NULL,
    exit_code INTEGER NULL,
    duration_ms BIGINT NULL,
    start_time TIMESTAMP WITH TIME ZONE NULL,
    end_time TIMESTAMP WITH TIME ZONE NULL
);

CREATE INDEX IF NOT EXISTS idx_execution_runs_execution_card_id ON execution_runs(execution_card_id);
CREATE INDEX IF NOT EXISTS idx_execution_runs_execution_task_id ON execution_runs(execution_task_id);

-- =========================================
-- 7. ProjectRepositories - 项目仓库表
-- 存储多项目配置信息，支持多项目管理
-- =========================================
CREATE TABLE IF NOT EXISTS project_repositories (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    local_working_dir VARCHAR(500) NOT NULL,
    git_remote_url VARCHAR(500) NOT NULL,
    default_branch VARCHAR(100) NOT NULL DEFAULT 'main',
    description TEXT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- 索引
CREATE INDEX IF NOT EXISTS idx_project_repositories_name ON project_repositories(name);

-- =========================================
-- 8. TaskPhaseProgress - 任务阶段进度跟踪表
-- 跟踪每个任务在 S/E 六阶段中的进度状态
-- =========================================
CREATE TABLE IF NOT EXISTS task_phase_progress (
    id SERIAL PRIMARY KEY,
    execution_card_id INTEGER NOT NULL,
    phase INTEGER NOT NULL, -- 1=需求分析 2=代码盘点 3=需求映射 4=增量开发 5=验证测试 6=知识沉淀
    status INTEGER NOT NULL, -- 0=未开始 1=进行中 2=已完成
    output_doc_path VARCHAR(500) NULL,
    logs TEXT NULL,
    started_at TIMESTAMP WITH TIME ZONE NULL,
    completed_at TIMESTAMP WITH TIME ZONE NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- 索引
CREATE INDEX IF NOT EXISTS idx_task_phase_progress_execution_card_id ON task_phase_progress(execution_card_id);
CREATE INDEX IF NOT EXISTS idx_task_phase_progress_phase ON task_phase_progress(phase);

-- =========================================
-- 9. TaskFileChanges - 任务文件变更记录表
-- 记录每个任务新增/修改/删除了哪些文件
-- =========================================
CREATE TABLE IF NOT EXISTS task_file_changes (
    id SERIAL PRIMARY KEY,
    execution_card_id INTEGER NOT NULL,
    file_path VARCHAR(1000) NOT NULL,
    change_type INTEGER NOT NULL, -- 0=新增 1=修改 2=删除
    commit_hash VARCHAR(100) NULL,
    changed_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- 索引
CREATE INDEX IF NOT EXISTS idx_task_file_changes_execution_card_id ON task_file_changes(execution_card_id);
CREATE INDEX IF NOT EXISTS idx_task_file_changes_change_type ON task_file_changes(change_type);

-- =========================================
-- 完成说明
-- =========================================
-- E-Kanban 新增表创建完成！
--
-- 新增表汇总：
--   1. board_work_items         - Azure Boards 工作项
--   2. execution_cards         - Kanban 执行卡片（核心表）
--   3. specs                  - 验收标准规范
--   4. spec_evaluations        - Spec 评估记录
--   5. execution_tasks         - 执行定义任务
--   6. execution_runs          - 执行事实记录
--   7. project_repositories    - 项目仓库配置（多项目支持）
--   8. task_phase_progress      - 任务阶段进度跟踪
--   9. task_file_changes        - 任务文件变更记录
--
-- 所有表都已添加必要的主键和索引，可直接使用。
