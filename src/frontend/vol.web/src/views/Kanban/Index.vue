<template>
  <div class="kanban-board">
    <div class="board-header">
      <h2>E-Kanban 执行看板</h2>
      <div class="header-actions">
        <el-button type="primary" @click="onSyncAzure">
          <el-icon><Refresh /></el-icon>
          同步 Azure Boards
        </el-button>
      </div>
    </div>

    <div class="board-columns" v-loading="loading">
      <div
        v-for="column in columns"
        :key="column.status"
        class="board-column"
      >
        <div class="column-header">
          <span class="column-title">{{ column.title }}</span>
          <el-tag size="small">{{ cardsByStatus[column.status]?.length || 0 }}</el-tag>
        </div>
        <div class="column-cards">
          <KanbanCard
            v-for="card in cardsByStatus[column.status]"
            :key="card.Id"
            :card="card"
            @trigger-execute="onTriggerExecute"
            @trigger-re-execute="onTriggerReExecute"
            @view-detail="onViewDetail"
          />
        </div>
      </div>
    </div>


  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import { ElMessage } from 'element-plus';
import { Refresh } from '@element-plus/icons-vue';
import KanbanCard from '@/components/Kanban/KanbanCard.vue';
import type {
  ExecutionCard,
  KanbanData,
  ExecutionCardStatus,
  ExecutionCardDetail,
  TaskPhaseProgress,
  TaskFileChange,
  DevelopmentPhase,
  PhaseStatus,
  ChangeType,
} from '@/types/kanban';
import * as api from '@/api/kanban';

const router = useRouter();
const loading = ref(false);
const cardsData = ref<KanbanData>({});

const columns = [
  { status: 0, title: '新建' },
  { status: 1, title: '就绪' },
  { status: 2, title: '执行中' },
  { status: 4, title: '已完成' },
  { status: 5, title: '失败' },
];

const cardsByStatus = computed(() => {
  const result: Record<number, ExecutionCard[]> = {};
  columns.forEach(col => {
    result[col.status] = cardsData.value[col.status] || [];
  });
  return result;
});

const statusMap: Record<number, string> = {
  0: '新建',
  1: '就绪',
  2: '执行中',
  3: '已提交',
  4: '已完成',
  5: '失败',
};

const executorMap: Record<number, string> = {
  0: '人工',
  1: 'AI',
  2: '系统',
};

// 开发阶段名称映射
const developmentPhaseMap: Record<number, string> = {
  1: '需求分析',
  2: '代码盘点',
  3: '需求-代码映射',
  4: '增量开发',
  5: '验证测试',
  6: '知识沉淀',
};

// 阶段状态映射
const phaseStatusMap: Record<number, string> = {
  0: '未开始',
  1: '进行中',
  2: '已完成',
};

const phaseStatusColorMap: Record<number, string> = {
  0: '#c0c4cc',
  1: '#409eff',
  2: '#67c23a',
};

const phaseStatusTypeMap: Record<number, string> = {
  0: 'info',
  1: 'primary',
  2: 'success',
};

// 文件变更类型映射
const changeTypeMap: Record<number, string> = {
  1: '新增',
  2: '修改',
  3: '删除',
};

const changeTypeTagMap: Record<number, string> = {
  1: 'success',
  2: 'warning',
  3: 'info',
};

const getStatusText = (status: number): string => statusMap[status] || '未知';
const getExecutorText = (type: number): string => executorMap[type] || '未知';
const getPhaseName = (phase: number): string => developmentPhaseMap[phase] || '未知';
const getPhaseStatusText = (status: number): string => phaseStatusMap[status] || '未知';
const getPhaseStatusColor = (status: number): string => phaseStatusColorMap[status] || '#c0c4cc';
const getPhaseStatusType = (status: number): string => phaseStatusTypeMap[status] || 'info';
const getChangeTypeName = (type: number): string => changeTypeMap[type] || '未知';
const getChangeTypeTag = (type: number): string => changeTypeTagMap[type] || '';

const formatDate = (dateStr: string | Date): string => {
  if (!dateStr) return '';
  return new Date(dateStr).toLocaleString('zh-CN');
};

// 格式化阶段时间显示
const formatPhaseTime = (phase: TaskPhaseProgress): string => {
  if (phase.CompletedAt) {
    return formatDate(phase.CompletedAt);
  }
  if (phase.StartedAt) {
    return `开始于 ${formatDate(phase.StartedAt)}`;
  }
  return '';
};



const loadKanbanData = async () => {
  loading.value = true;
  try {
    const res = await api.getKanbanData();
    // 转换接口返回的数据结构
    const transformedData: Record<number, any[]> = {};
    
    // 处理不同状态的卡片
    if (res && res.new) {
      // 转换 new 状态的卡片
      transformedData[0] = res.new.map((card: any) => ({
        Id: card.id,
        Title: card.title,
        Description: card.description,
        Status: card.status,
        ExecutorType: card.executorType,
        BoardWorkItemId: card.boardWorkItemId,
        BoardId: card.boardId,
        ProjectRepositoryId: card.projectRepositoryId,
        SpecId: card.specId,
        FailureCount: card.failureCount,
        NeedsManualIntervention: card.needsManualIntervention,
        InProgressStartedAt: card.inProgressStartTime,
        CreatedAt: card.createdAt,
        UpdatedAt: card.lastUpdated,
        IsManualCreated: card.isManualCreated
      }));
    }
    
    // 处理其他可能的状态
    if (res && res.ready) {
      transformedData[1] = res.ready.map((card: any) => ({
        Id: card.id,
        Title: card.title,
        Description: card.description,
        Status: card.status,
        ExecutorType: card.executorType,
        BoardWorkItemId: card.boardWorkItemId,
        BoardId: card.boardId,
        ProjectRepositoryId: card.projectRepositoryId,
        SpecId: card.specId,
        FailureCount: card.failureCount,
        NeedsManualIntervention: card.needsManualIntervention,
        InProgressStartedAt: card.inProgressStartTime,
        CreatedAt: card.createdAt,
        UpdatedAt: card.lastUpdated,
        IsManualCreated: card.isManualCreated
      }));
    }
    
    if (res && res.inProgress) {
      transformedData[2] = res.inProgress.map((card: any) => ({
        Id: card.id,
        Title: card.title,
        Description: card.description,
        Status: card.status,
        ExecutorType: card.executorType,
        BoardWorkItemId: card.boardWorkItemId,
        BoardId: card.boardId,
        ProjectRepositoryId: card.projectRepositoryId,
        SpecId: card.specId,
        FailureCount: card.failureCount,
        NeedsManualIntervention: card.needsManualIntervention,
        InProgressStartedAt: card.inProgressStartTime,
        CreatedAt: card.createdAt,
        UpdatedAt: card.lastUpdated,
        IsManualCreated: card.isManualCreated
      }));
    }
    
    if (res && res.completed) {
      transformedData[4] = res.completed.map((card: any) => ({
        Id: card.id,
        Title: card.title,
        Description: card.description,
        Status: card.status,
        ExecutorType: card.executorType,
        BoardWorkItemId: card.boardWorkItemId,
        BoardId: card.boardId,
        ProjectRepositoryId: card.projectRepositoryId,
        SpecId: card.specId,
        FailureCount: card.failureCount,
        NeedsManualIntervention: card.needsManualIntervention,
        InProgressStartedAt: card.inProgressStartTime,
        CreatedAt: card.createdAt,
        UpdatedAt: card.lastUpdated,
        IsManualCreated: card.isManualCreated
      }));
    }
    
    if (res && res.failed) {
      transformedData[5] = res.failed.map((card: any) => ({
        Id: card.id,
        Title: card.title,
        Description: card.description,
        Status: card.status,
        ExecutorType: card.executorType,
        BoardWorkItemId: card.boardWorkItemId,
        BoardId: card.boardId,
        ProjectRepositoryId: card.projectRepositoryId,
        SpecId: card.specId,
        FailureCount: card.failureCount,
        NeedsManualIntervention: card.needsManualIntervention,
        InProgressStartedAt: card.inProgressStartTime,
        CreatedAt: card.createdAt,
        UpdatedAt: card.lastUpdated,
        IsManualCreated: card.isManualCreated
      }));
    }
    
    cardsData.value = transformedData;
  } catch (e) {
    ElMessage.error('加载看板数据失败');
    console.error(e);
  } finally {
    loading.value = false;
  }
};

const onSyncAzure = async () => {
  try {
    await api.triggerSyncFromAzureBoards();
    ElMessage.success('同步已触发');
    await loadKanbanData();
  } catch (e) {
    ElMessage.error('同步失败');
    console.error(e);
  }
};

const onTriggerExecute = async (id: number) => {
  try {
    await api.triggerAiExecution(id);
    ElMessage.success('执行已触发');
    await loadKanbanData();
  } catch (e) {
    ElMessage.error('触发执行失败');
    console.error(e);
  }
};

const onTriggerReExecute = async (id: number) => {
  try {
    await api.triggerReExecute(id);
    ElMessage.success('重新执行已触发');
    await loadKanbanData();
  } catch (e) {
    ElMessage.error('触发重新执行失败');
    console.error(e);
  }
};

const onViewDetail = (card: ExecutionCard) => {
  // 跳转到详情页面
  router.push(`/kanban/detail/${card.Id}`);
};

onMounted(() => {
  loadKanbanData();
});
</script>

<style scoped>
.kanban-board {
  padding: 16px;
  height: 100%;
  display: flex;
  flex-direction: column;
}

.board-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

.board-header h2 {
  margin: 0;
  font-size: 20px;
  font-weight: 600;
}

.board-columns {
  display: flex;
  flex: 1;
  gap: 16px;
  overflow-x: auto;
  min-height: 0;
}

.board-column {
  flex: 1;
  min-width: 280px;
  background-color: #f5f5f5;
  border-radius: 8px;
  padding: 8px;
  display: flex;
  flex-direction: column;
}

.column-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 12px;
  margin-bottom: 8px;
  font-weight: 600;
  background-color: #fff;
  border-radius: 4px;
}

.column-title {
  font-size: 14px;
}

.column-cards {
  flex: 1;
  overflow-y: auto;
  padding: 4px;
}

.detail-content {
  padding: 8px 0;
}

.detail-section {
  margin-top: 16px;
}

.detail-section h4 {
  margin-bottom: 8px;
  font-weight: 600;
}

.detail-section p {
  color: #666;
  line-height: 1.6;
  white-space: pre-wrap;
}

.phase-list {
  padding: 8px 0;
}

.phase-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.phase-name {
  font-weight: 600;
  font-size: 15px;
}

.phase-log {
  margin-top: 8px;
}

.phase-log pre {
  background-color: #f5f5f5;
  padding: 8px;
  border-radius: 4px;
  white-space: pre-wrap;
  word-break: break-all;
  margin: 8px 0;
}
</style>
