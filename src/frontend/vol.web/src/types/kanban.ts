// E-Kanban 类型定义

// 执行卡片状态
export enum ExecutionCardStatus {
  New = 0,
  Ready = 1,
  InProgress = 2,
  Submitted = 3,
  Completed = 4,
  Failed = 5,
}

// 执行者类型
export enum ExecutorType {
  Human = 0,
  AI = 1,
  System = 2,
}

// 开发阶段（六阶段方法论）
export enum DevelopmentPhase {
  RequirementAnalysis = 1,
  CodeInventory = 2,
  RequirementCodeMapping = 3,
  IncrementalDevelopment = 4,
  VerificationTesting = 5,
  KnowledgeDeposition = 6,
}

// 阶段状态
export enum PhaseStatus {
  NotStarted = 0,
  InProgress = 1,
  Completed = 2,
}

// 文件变更类型
export enum ChangeType {
  Added = 1,
  Modified = 2,
  Deleted = 3,
}

// 项目仓库
export interface ProjectRepository {
  Id: number;
  Name: string;
  LocalWorkingDir: string;
  GitRemoteUrl?: string;
  DefaultBranch: string;
  Description?: string;
  IsActive: boolean;
  CreatedAt: Date;
  UpdatedAt: Date;
}

// 任务阶段进度
export interface TaskPhaseProgress {
  Id: number;
  ExecutionCardId: number;
  Phase: DevelopmentPhase;
  Status: PhaseStatus;
  StartedAt?: Date;
  CompletedAt?: Date;
  PhaseLog?: string;
  CreatedAt: Date;
  UpdatedAt: Date;
}

// 文件变更记录
export interface TaskFileChange {
  Id: number;
  ExecutionCardId: number;
  FilePath: string;
  ChangeType: ChangeType;
  CommitHash?: string;
  CommitMessage?: string;
  ChangedAt: Date;
  CreatedAt: Date;
}

// 执行卡片
export interface ExecutionCard {
  Id: number;
  Title: string;
  Description: string;
  Status: ExecutionCardStatus;
  ExecutorType: ExecutorType;
  BoardWorkItemId: number;
  BoardId: string;
  ProjectRepositoryId?: number;
  SpecId?: number;
  FailureCount: number;
  NeedsManualIntervention: boolean;
  InProgressStartedAt?: Date;
  CreatedAt: Date;
  UpdatedAt: Date;
  IsManualCreated: boolean;
}

// 手动看板创建请求类型
export interface ManualCardCreateRequest {
  title: string;
  description?: string;
  boardId?: string;
  projectRepositoryId?: number;
  specId?: number;
}

// 手动看板更新请求类型
export interface ManualCardUpdateRequest {
  id: number;
  title: string;
  description?: string;
  boardId?: string;
  projectRepositoryId?: number;
  specId?: number;
}

// 看板数据按状态分组
export type KanbanData = Record<ExecutionCardStatus, ExecutionCard[]>;

// 卡片详情（包含阶段进度和文件变更）
export interface ExecutionCardDetail extends ExecutionCard {
  projectRepository?: ProjectRepository;
  phaseProgressList: TaskPhaseProgress[];
  fileChangeList: TaskFileChange[];
}

// 执行卡片状态文本映射
export const ExecutionCardStatusText = {
  [ExecutionCardStatus.New]: '新建',
  [ExecutionCardStatus.Ready]: '就绪',
  [ExecutionCardStatus.InProgress]: '进行中',
  [ExecutionCardStatus.Submitted]: '已提交',
  [ExecutionCardStatus.Completed]: '已完成',
  [ExecutionCardStatus.Failed]: '失败'
};

// 执行卡片状态标签类型映射
export const ExecutionCardStatusTagType = {
  [ExecutionCardStatus.New]: 'info',
  [ExecutionCardStatus.Ready]: 'primary',
  [ExecutionCardStatus.InProgress]: 'warning',
  [ExecutionCardStatus.Submitted]: 'success',
  [ExecutionCardStatus.Completed]: 'success',
  [ExecutionCardStatus.Failed]: 'danger'
};
