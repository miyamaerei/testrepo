<template>
  <div class="manual-kanban-page">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>手动看板管理</span>
          <el-button type="primary" @click="openCreateDialog">创建看板</el-button>
        </div>
      </template>
      
      <!-- 搜索和筛选 -->
      <div class="search-filter">
        <el-input
          v-model="searchQuery"
          placeholder="按标题搜索"
          style="width: 300px; margin-right: 10px"
          clearable
        />
        <el-select
          v-model="statusFilter"
          placeholder="按状态筛选"
          style="width: 150px"
          clearable
        >
          <el-option label="全部" value="" />
          <el-option 
            v-for="(text, value) in ExecutionCardStatusText" 
            :key="value" 
            :label="text" 
            :value="value" 
          />
        </el-select>
      </div>
      
      <!-- 卡片列表 -->
      <el-table
        :data="cards"
        style="width: 100%"
        v-loading="loading"
        height="600px"
        border
        stripe
      >
        <el-table-column prop="Id" label="ID" width="80" />
        <el-table-column prop="Title" label="标题" />
        <el-table-column prop="Description" label="描述" :show-overflow-tooltip="true" />
        <el-table-column prop="Status" label="状态" width="120">
          <template #default="scope">
            <el-tag :type="getStatusTagType(scope.row.Status)">
              {{ getStatusText(scope.row.Status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="BoardId" label="看板ID" width="150" />
        <el-table-column prop="CreatedAt" label="创建时间" width="180" />
        <el-table-column label="操作" width="150" fixed="right">
          <template #default="scope">
            <el-button
              type="primary"
              size="small"
              @click="openEditDialog(scope.row)"
              :disabled="!canEdit(scope.row)"
            >
              编辑
            </el-button>
            <el-button
              type="danger"
              size="small"
              @click="confirmDelete(scope.row.Id)"
            >
              删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>
      
      <!-- 分页 -->
      <div class="pagination" v-if="total > 0">
        <el-pagination
          v-model:current-page="currentPage"
          v-model:page-size="pageSize"
          :page-sizes="[10, 20, 50, 100]"
          layout="total, sizes, prev, pager, next, jumper"
          :total="total"
          @size-change="handleSizeChange"
          @current-change="handleCurrentChange"
        />
      </div>
    </el-card>
    
    <!-- 创建对话框 -->
    <el-dialog
      v-model="createDialogVisible"
      title="创建手动看板"
      width="600px"
    >
      <el-form :model="createForm" :rules="rules" ref="createFormRef" label-width="100px">
        <el-form-item label="标题" prop="title">
          <el-input v-model="createForm.title" placeholder="请输入看板标题" />
        </el-form-item>
        <el-form-item label="描述" prop="description">
          <el-input
            v-model="createForm.description"
            type="textarea"
            placeholder="请输入看板描述"
            :rows="3"
          />
        </el-form-item>
        <el-form-item label="看板ID" prop="boardId">
          <el-input v-model="createForm.boardId" placeholder="请输入看板ID（默认：Manual）" />
        </el-form-item>
        <el-form-item label="项目仓库ID" prop="projectRepositoryId">
          <el-input v-model.number="createForm.projectRepositoryId" placeholder="请输入项目仓库ID（可选）" />
        </el-form-item>
        <el-form-item label="规格ID" prop="specId">
          <el-input v-model.number="createForm.specId" placeholder="请输入规格ID（可选）" />
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="createDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="handleCreate">创建</el-button>
        </span>
      </template>
    </el-dialog>
    
    <!-- 编辑对话框 -->
    <el-dialog
      v-model="editDialogVisible"
      title="编辑手动看板"
      width="600px"
    >
      <el-form :model="editForm" :rules="rules" ref="editFormRef" label-width="100px">
        <el-form-item label="标题" prop="title">
          <el-input v-model="editForm.title" placeholder="请输入看板标题" />
        </el-form-item>
        <el-form-item label="描述" prop="description">
          <el-input
            v-model="editForm.description"
            type="textarea"
            placeholder="请输入看板描述"
            :rows="3"
          />
        </el-form-item>
        <el-form-item label="看板ID" prop="boardId">
          <el-input v-model="editForm.boardId" placeholder="请输入看板ID" />
        </el-form-item>
        <el-form-item label="项目仓库ID" prop="projectRepositoryId">
          <el-input v-model.number="editForm.projectRepositoryId" placeholder="请输入项目仓库ID（可选）" />
        </el-form-item>
        <el-form-item label="规格ID" prop="specId">
          <el-input v-model.number="editForm.specId" placeholder="请输入规格ID（可选）" />
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="editDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="handleUpdate">保存</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import * as kanbanApi from '@/api/kanban';
import type { ExecutionCard, ManualCardCreateRequest, ManualCardUpdateRequest } from '@/types/kanban';
import { 
  ExecutionCardStatus, 
  ExecutionCardStatusText, 
  ExecutionCardStatusTagType 
} from '@/types/kanban';

/**
 * 响应式数据
 */
const loading = ref(false);
const searchQuery = ref('');
const statusFilter = ref<number | string>('');
const currentPage = ref(1);
const pageSize = ref(10);
const total = ref(0);
const cards = ref<ExecutionCard[]>([]);

/**
 * 监听搜索和筛选变化
 */
watch([searchQuery, statusFilter], () => {
  currentPage.value = 1;
  fetchCards();
});

/**
 * 对话框相关
 */
const createDialogVisible = ref(false);
const editDialogVisible = ref(false);
const createForm = ref<ManualCardCreateRequest>({
  title: '',
  description: '',
  boardId: '',
  projectRepositoryId: undefined,
  specId: undefined
});
const editForm = ref<ManualCardUpdateRequest>({
  id: 0,
  title: '',
  description: '',
  boardId: '',
  projectRepositoryId: undefined,
  specId: undefined
});
const createFormRef = ref();
const editFormRef = ref();

/**
 * 验证规则
 */
const rules = {
  title: [
    { required: true, message: '请输入标题', trigger: 'blur' },
    { max: 500, message: '标题长度不能超过500个字符', trigger: 'blur' }
  ]
};

/**
 * 获取手动看板卡片列表
 */
async function fetchCards() {
  loading.value = true;
  try {
    const response = await kanbanApi.getManualCards({
      page: currentPage.value,
      pageSize: pageSize.value,
      search: searchQuery.value,
      status: typeof statusFilter.value === 'number' ? statusFilter.value : undefined
    });
    // 转换后端返回的小写字段为大写字段
    cards.value = response.items.map((item: any) => ({
      Id: item.id,
      Title: item.title,
      Description: item.description,
      Status: item.status,
      ExecutorType: item.executorType,
      BoardWorkItemId: item.boardWorkItemId,
      BoardId: item.boardId,
      ProjectRepositoryId: item.projectRepositoryId,
      SpecId: item.specId,
      FailureCount: item.failureCount,
      NeedsManualIntervention: item.needsManualIntervention,
      InProgressStartedAt: item.inProgressStartTime,
      CreatedAt: item.createdAt,
      UpdatedAt: item.lastUpdated,
      IsManualCreated: item.isManualCreated
    }));
    total.value = response.total;
  } catch (error: any) {
    ElMessage.error(error.message || '获取卡片列表失败');
    console.error('Failed to fetch cards:', error);
  } finally {
    loading.value = false;
  }
}

/**
 * 生命周期
 */
onMounted(() => {
  fetchCards();
});

/**
 * 打开创建对话框
 */
function openCreateDialog() {
  createForm.value = {
    title: '',
    description: '',
    boardId: '',
    projectRepositoryId: undefined,
    specId: undefined
  };
  createDialogVisible.value = true;
}

/**
 * 打开编辑对话框
 * @param card 卡片数据
 */
function openEditDialog(card: ExecutionCard) {
  editForm.value = {
    id: card.Id,
    title: card.Title,
    description: card.Description,
    boardId: card.BoardId,
    projectRepositoryId: card.ProjectRepositoryId,
    specId: card.SpecId
  };
  editDialogVisible.value = true;
}

/**
 * 处理创建操作
 */
async function handleCreate() {
  if (!createFormRef.value) return;
  
  await createFormRef.value.validate(async (valid: boolean) => {
    if (valid) {
      try {
        await kanbanApi.createManualCard(createForm.value);
        ElMessage.success('创建成功');
        createDialogVisible.value = false;
        await fetchCards();
      } catch (error: any) {
        ElMessage.error(error.message || '创建失败');
        console.error('Failed to create card:', error);
      }
    }
  });
}

/**
 * 处理更新操作
 */
async function handleUpdate() {
  if (!editFormRef.value) return;
  
  await editFormRef.value.validate(async (valid: boolean) => {
    if (valid) {
      try {
        await kanbanApi.updateManualCard(editForm.value);
        ElMessage.success('更新成功');
        editDialogVisible.value = false;
        await fetchCards();
      } catch (error: any) {
        ElMessage.error(error.message || '更新失败');
        console.error('Failed to update card:', error);
      }
    }
  });
}

/**
 * 确认删除操作
 * @param id 卡片ID
 */
async function confirmDelete(id: number) {
  try {
    await ElMessageBox.confirm(
      '确定要删除此看板吗？',
      '删除确认',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    );
    
    await kanbanApi.deleteManualCard(id);
    ElMessage.success('删除成功');
    await fetchCards();
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error(error.message || '删除失败');
      console.error('Failed to delete card:', error);
    }
  }
}

/**
 * 获取状态标签类型
 * @param status 状态值
 * @returns 标签类型
 */
function getStatusTagType(status: ExecutionCardStatus): string {
  return ExecutionCardStatusTagType[status] || 'info';
}

/**
 * 获取状态文本
 * @param status 状态值
 * @returns 状态文本
 */
function getStatusText(status: ExecutionCardStatus): string {
  return ExecutionCardStatusText[status] || '未知';
}

/**
 * 判断是否可以编辑
 * @param card 卡片数据
 * @returns 是否可以编辑
 */
function canEdit(card: ExecutionCard): boolean {
  return card.Status === ExecutionCardStatus.New || card.Status === ExecutionCardStatus.Failed;
}

/**
 * 处理分页大小变化
 * @param size 每页大小
 */
function handleSizeChange(size: number) {
  pageSize.value = size;
  currentPage.value = 1;
  fetchCards();
}

/**
 * 处理当前页码变化
 * @param page 当前页码
 */
function handleCurrentChange(page: number) {
  currentPage.value = page;
  fetchCards();
}
</script>

<style scoped>
.manual-kanban-page {
  padding: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.search-filter {
  margin: 20px 0;
  display: flex;
  align-items: center;
}

.pagination {
  margin-top: 20px;
  display: flex;
  justify-content: flex-end;
}

.dialog-footer {
  width: 100%;
  display: flex;
  justify-content: flex-end;
  padding: 0 20px 20px;
}

/* 修复对话框内边距 */
:deep(.el-dialog) {
  padding: 20px !important;
}

:deep(.el-dialog__body) {
  padding: 20px 0 !important;
}
</style>