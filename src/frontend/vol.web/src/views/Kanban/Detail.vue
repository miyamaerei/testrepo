<template>
  <div class="kanban-detail">
    <div class="detail-header">
      <el-button type="primary" @click="goBack">
        <el-icon><ArrowLeft /></el-icon>
        返回看板
      </el-button>
      <h2>卡片详情</h2>
    </div>

    <div class="detail-content" v-loading="loading">
      <div v-if="cardDetail" class="card-info">
        <el-tabs v-model="activeTab">
          <!-- 基本信息标签页 -->
          <el-tab-pane label="基本信息" name="basic">
            <el-descriptions :column="2" border>
              <el-descriptions-item label="标题" :span="2">{{ cardDetail.Title }}</el-descriptions-item>
              <el-descriptions-item label="ID">{{ cardDetail.Id }}</el-descriptions-item>
              <el-descriptions-item label="状态">{{ getStatusText(cardDetail.Status) }}</el-descriptions-item>
              <el-descriptions-item label="执行者">{{ getExecutorText(cardDetail.ExecutorType) }}</el-descriptions-item>
              <el-descriptions-item label="失败次数">{{ cardDetail.FailureCount }}</el-descriptions-item>
              <el-descriptions-item label="需要人工干预">
                <el-tag :type="cardDetail.NeedsManualIntervention ? 'danger' : 'success'">
                  {{ cardDetail.NeedsManualIntervention ? '是' : '否' }}
                </el-tag>
              </el-descriptions-item>
              <el-descriptions-item v-if="cardDetail.projectRepository" label="关联项目">
                {{ cardDetail.projectRepository.Name }}
              </el-descriptions-item>
              <el-descriptions-item label="创建时间">{{ formatDate(cardDetail.CreatedAt) }}</el-descriptions-item>
              <el-descriptions-item label="更新时间">{{ formatDate(cardDetail.UpdatedAt) }}</el-descriptions-item>
            </el-descriptions>

            <div v-if="cardDetail.Description" class="detail-section">
              <h4>描述</h4>
              <p>{{ cardDetail.Description }}</p>
            </div>

            <div v-if="cardDetail.projectRepository" class="detail-section">
              <h4>项目信息</h4>
              <el-descriptions :column="1" border>
                <el-descriptions-item label="项目名称">{{ cardDetail.projectRepository.Name }}</el-descriptions-item>
                <el-descriptions-item label="本地工作目录">{{ cardDetail.projectRepository.LocalWorkingDir }}</el-descriptions-item>
                <el-descriptions-item v-if="cardDetail.projectRepository.GitRemoteUrl" label="Git 远程地址">
                  {{ cardDetail.projectRepository.GitRemoteUrl }}
                </el-descriptions-item>
                <el-descriptions-item label="默认分支">{{ cardDetail.projectRepository.DefaultBranch }}</el-descriptions-item>
                <el-descriptions-item v-if="cardDetail.projectRepository.Description" label="描述">
                  {{ cardDetail.projectRepository.Description }}
                </el-descriptions-item>
              </el-descriptions>
            </div>
          </el-tab-pane>

          <!-- 阶段进度标签页 -->
          <el-tab-pane label="阶段进度" name="phase">
            <div class="phase-list">
              <el-timeline>
                <el-timeline-item
                  v-for="phase in sortedPhaseProgress"
                  :key="phase.Id"
                  :timestamp="formatPhaseTime(phase)"
                  :color="getPhaseStatusColor(phase.Status)"
                >
                  <el-card>
                    <div class="phase-header">
                      <span class="phase-name">{{ getPhaseName(phase.Phase) }}</span>
                      <el-tag :type="getPhaseStatusType(phase.Status)">
                        {{ getPhaseStatusText(phase.Status) }}
                      </el-tag>
                    </div>
                    <div v-if="phase.PhaseLog" class="phase-log">
                      <el-collapse-transition>
                        <div v-show="phase.expanded">{{ phase.PhaseLog }}</div>
                      </el-collapse-transition>
                      <el-link
                        type="primary"
                        :underline="false"
                        @click="phase.expanded = !phase.expanded"
                      >
                        {{ phase.expanded ? '收起日志' : '展开日志' }}
                      </el-link>
                    </div>
                  </el-card>
                </el-timeline-item>
              </el-timeline>
            </div>
          </el-tab-pane>

          <!-- 文件变更标签页 -->
          <el-tab-pane label="文件变更" name="file">
            <el-table :data="cardDetail.fileChangeList" border style="width: 100%">
              <el-table-column prop="FilePath" label="文件路径" min-width="300" />
              <el-table-column prop="ChangeType" label="变更类型" width="120">
                <template #default="{ row }">
                  <el-tag :type="getChangeTypeTag(row.ChangeType)">
                    {{ getChangeTypeName(row.ChangeType) }}
                  </el-tag>
                </template>
              </el-table-column>
              <el-table-column prop="CommitHash" label="Commit Hash" width="100" />
              <el-table-column prop="CommitMessage" label="提交信息" min-width="200" />
              <el-table-column prop="ChangedAt" label="变更时间" width="160">
                <template #default="{ row }">
                  {{ formatDate(row.ChangedAt) }}
                </template>
              </el-table-column>
            </el-table>
          </el-tab-pane>
        </el-tabs>
      </div>
      <div v-else class="empty-state">
        <el-empty description="暂无卡片详情" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { ElMessage } from 'element-plus';
import { ArrowLeft } from '@element-plus/icons-vue';
import * as api from '@/api/kanban';

const router = useRouter();
const route = useRoute();
const loading = ref(false);
const activeTab = ref('basic');
const cardDetail = ref<any>(null);

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
const formatPhaseTime = (phase: any): string => {
  if (phase.CompletedAt) {
    return formatDate(phase.CompletedAt);
  }
  if (phase.StartedAt) {
    return `开始于 ${formatDate(phase.StartedAt)}`;
  }
  return '';
};

// 对阶段进度按顺序排序
const sortedPhaseProgress = computed(() => {
  if (!cardDetail.value?.phaseProgressList) return [];
  // 添加 expanded 属性用于展开日志
  return cardDetail.value.phaseProgressList
    .sort((a: any, b: any) => a.Phase - b.Phase)
    .map((p: any) => ({ ...p, expanded: false }));
});

const loadCardDetail = async () => {
  const id = route.params.id as string;
  if (!id) {
    ElMessage.error('缺少卡片ID');
    return;
  }

  loading.value = true;
  try {
    const res = await api.getExecutionCardDetail(Number(id));
    if (res && res.status) {
      // 转换数据格式，将蛇形命名转换为驼峰命名
      const data = res.data;
      cardDetail.value = {
        Id: data.id,
        Title: data.title,
        Description: data.description,
        Status: data.status,
        ExecutorType: data.executorType,
        BoardWorkItemId: data.boardWorkItemId,
        BoardId: data.boardId,
        ProjectRepositoryId: data.projectRepositoryId,
        SpecId: data.specId,
        FailureCount: data.failureCount,
        NeedsManualIntervention: data.needsManualIntervention,
        InProgressStartedAt: data.inProgressStartedAt,
        CreatedAt: data.createdAt,
        UpdatedAt: data.updatedAt,
        IsManualCreated: data.isManualCreated,
        phaseProgressList: data.phaseProgressList || [],
        fileChangeList: data.fileChangeList || [],
        projectRepository: data.projectRepository ? {
          Id: data.projectRepository.id,
          Name: data.projectRepository.name,
          LocalWorkingDir: data.projectRepository.localWorkingDir,
          GitRemoteUrl: data.projectRepository.gitRemoteUrl,
          DefaultBranch: data.projectRepository.defaultBranch,
          Description: data.projectRepository.description
        } : null
      };
    }
  } catch (e) {
    ElMessage.error('加载卡片详情失败');
    console.error(e);
  } finally {
    loading.value = false;
  }
};

const goBack = () => {
  router.push('/kanban');
};

onMounted(() => {
  loadCardDetail();
});
</script>

<style scoped>
.kanban-detail {
  padding: 16px;
  min-height: 100vh;
  background-color: #f5f7fa;
}

.detail-header {
  display: flex;
  align-items: center;
  margin-bottom: 24px;
  padding-bottom: 16px;
  border-bottom: 1px solid #e4e7ed;
}

.detail-header h2 {
  margin: 0 0 0 16px;
  font-size: 20px;
  font-weight: 600;
}

.detail-content {
  background-color: #fff;
  border-radius: 8px;
  padding: 24px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
}

.card-info {
  width: 100%;
}

.detail-section {
  margin-top: 24px;
}

.detail-section h4 {
  margin-bottom: 12px;
  font-weight: 600;
  font-size: 16px;
  color: #303133;
}

.detail-section p {
  color: #606266;
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

.empty-state {
  text-align: center;
  padding: 48px 0;
}
</style>