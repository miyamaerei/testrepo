<template>
  <el-card class="kanban-card" :class="{ 'needs-manual': card.NeedsManualIntervention, 'manual-created': card.IsManualCreated }">
    <template #header>
      <div class="card-header">
        <div class="title-container">
          <span class="card-title">{{ card.Title }}</span>
          <el-tag v-if="card.projectRepository" size="small" type="primary" style="margin-left: 8px">
            {{ card.projectRepository.Name }}
          </el-tag>
        </div>
        <div class="header-right">
          <el-tag v-if="card.IsManualCreated" size="small" type="info" style="margin-right: 8px">
            手动创建
          </el-tag>
          <el-tag :type="getTagType(card.Status)">{{ getStatusText(card.Status) }}</el-tag>
        </div>
      </div>
    </template>

    <div class="card-body">
      <div v-if="card.Description" class="card-description">
        {{ card.Description }}
      </div>

      <div v-if="card.projectRepository" class="project-info">
        <el-divider content-position="left" class="project-divider">项目信息</el-divider>
        <div class="project-details">
          <div class="project-item">
            <span class="project-label">项目名称:</span>
            <span class="project-value">{{ card.projectRepository.Name }}</span>
          </div>
          <div class="project-item">
            <span class="project-label">本地路径:</span>
            <span class="project-value">{{ card.projectRepository.LocalWorkingDir }}</span>
          </div>
        </div>
      </div>

      <div class="card-meta">
        <div class="meta-item">
          <span class="meta-label">执行者:</span>
          <el-tag size="small">{{ getExecutorText(card.ExecutorType) }}</el-tag>
        </div>
        <div class="meta-item" v-if="card.FailureCount > 0">
          <span class="meta-label">失败次数:</span>
          <span>{{ card.FailureCount }}</span>
        </div>
      </div>

      <div v-if="card.NeedsManualIntervention" class="manual-warning">
        <el-alert
          title="需要人工干预"
          type="warning"
          description="该任务多次执行失败，请检查后重试"
          show-icon
          :closable="false"
        />
      </div>
    </div>

    <template #footer>
      <div class="card-actions">
        <el-button
          v-if="card.Status === 2 && card.ExecutorType === 1 && !card.NeedsManualIntervention"
          type="primary"
          size="small"
          @click="onTriggerExecute"
        >
          执行
        </el-button>
        <el-button
          v-if="card.Status === 2 || card.NeedsManualIntervention"
          type="warning"
          size="small"
          @click="onTriggerReExecute"
        >
          重新执行
        </el-button>
        <el-button
          size="small"
          @click="onViewDetail"
        >
          详情
        </el-button>
      </div>
    </template>
  </el-card>
</template>

<script setup lang="ts">
import { defineEmits } from 'vue';
import type { ExecutionCard } from '@/types/kanban';

const props = defineProps<{
  card: ExecutionCard;
}>();

const emit = defineEmits<{
  triggerExecute: [id: number];
  triggerReExecute: [id: number];
  viewDetail: [card: ExecutionCard];
}>();

const statusMap: Record<number, { text: string; type: string }> = {
  0: { text: '新建', type: 'info' },
  1: { text: '就绪', type: 'primary' },
  2: { text: '执行中', type: 'warning' },
  3: { text: '已提交', type: '' },
  4: { text: '已完成', type: 'success' },
  5: { text: '失败', type: 'danger' },
};

const executorMap: Record<number, string> = {
  0: '人工',
  1: 'AI',
  2: '系统',
};

const getStatusText = (status: number): string => {
  return statusMap[status]?.text || '未知';
};

const getTagType = (status: number): string => {
  return statusMap[status]?.type || 'info';
};

const getExecutorText = (type: number): string => {
  return executorMap[type] || '未知';
};

const onTriggerExecute = () => {
  emit('triggerExecute', props.card.Id);
};

const onTriggerReExecute = () => {
  emit('triggerReExecute', props.card.Id);
};

const onViewDetail = () => {
  emit('viewDetail', props.card);
};
</script>

<style scoped>
.kanban-card {
  margin-bottom: 12px;
  cursor: pointer;
  transition: box-shadow 0.3s;
}

.kanban-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.kanban-card.needs-manual {
  border: 2px solid #e6a23c;
}

.kanban-card.manual-created {
  border-left: 4px solid #409eff;
}

.header-right {
  display: flex;
  align-items: center;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.card-title {
  font-weight: 600;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  max-width: 70%;
}

.card-body {
  padding: 8px 0;
}

.card-description {
  color: #666;
  font-size: 13px;
  margin-bottom: 12px;
  overflow: hidden;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
}

.card-meta {
  display: flex;
  gap: 16px;
  font-size: 12px;
  color: #888;
}

.meta-label {
  margin-right: 4px;
}

.manual-warning {
  margin-top: 12px;
}

.card-actions {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
}

.title-container {
  display: flex;
  align-items: center;
  flex: 1;
}

.project-info {
  margin: 12px 0;
  padding: 10px;
  background-color: #f9f9f9;
  border-radius: 4px;
}

.project-divider {
  margin: 0 0 10px 0;
}

.project-details {
  font-size: 13px;
}

.project-item {
  margin-bottom: 4px;
  display: flex;
  align-items: center;
}

.project-label {
  font-weight: 500;
  margin-right: 8px;
  color: #666;
  min-width: 80px;
}

.project-value {
  color: #333;
  word-break: break-all;
}
</style>
