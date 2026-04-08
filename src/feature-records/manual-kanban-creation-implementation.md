# 手动增加看板功能实现方案

## 文档信息
- **版本**：1.0.0
- **作者**：系统开发团队
- **更新时间**：2026-04-07
- **状态**：初稿

## 目录
1. [功能需求分析](#功能需求分析)
2. [后端修改方案](#后端修改方案)
3. [前端修改方案](#前端修改方案)
4. [实现步骤](#实现步骤)
   - [数据库修改](#数据库修改)
   - [后端修改](#后端修改)
   - [前端修改](#前端修改)
   - [部署步骤](#部署步骤)
   - [测试验证](#测试验证)
5. [测试计划](#测试计划)
   - [数据库测试](#数据库测试)
   - [功能测试](#功能测试)
   - [UI测试](#ui测试)
   - [集成测试](#集成测试)
   - [性能测试](#性能测试)
6. [技术风险评估](#技术风险评估)
7. [预期效果](#预期效果)

## 一、功能需求分析

### 1. 核心需求
- **手动创建看板**：用户可以通过前端界面手动创建新的看板卡片
- **与Azure区分**：手动创建的看板需要与Azure Boards同步的看板明确区分
- **后端字段**：在后端ExecutionCard模型中添加字段，用于标识看板的创建来源
- **前端显示**：在前端看板卡片中显示创建来源的标识
- **完整管理**：支持对手动创建的看板进行编辑、删除和查询操作
- **独立组件**：创建独立的手动看板管理组件，实现完整的增删改查功能

### 2. 技术实现要点
- 在后端ExecutionCard模型中添加IsManualCreated字段
- 实现手动创建看板的API接口
- 前端添加创建看板的UI组件
- 在看板卡片中显示创建来源

## 二、后端修改方案

### 1. 数据模型修改
**文件**: `src/modules/EKanban/Models/ExecutionCard.cs`

在ExecutionCard类中添加IsManualCreated字段：
```csharp
/// <summary>
/// 是否手动创建（true: 手动创建, false: Azure同步）
/// </summary>
public bool IsManualCreated { get; set; } = false;

/// <summary>
/// 手动创建时的项目仓库ID（可选）
/// </summary>
public int? ProjectRepositoryId { get; set; }

/// <summary>
/// 手动创建时的规格ID（可选）
/// </summary>
public int? SpecId { get; set; }
```

### 2. API接口添加
**文件**: `src/backend/VOL.WebApi/Controllers/EKanban/ExecutionCardController.cs`

添加CreateManualCard和UpdateManualCard接口：
```csharp
/// <summary>
/// 手动创建看板卡片
/// </summary>
[HttpPost("CreateManualCard")]
public async Task<IActionResult> CreateManualCard([FromBody] ManualCardCreateRequest request)
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return BadRequest("标题不能为空");
    }
    
    if (request.Title.Length > 200)
    {
        return BadRequest("标题长度不能超过200个字符");
    }
    
    var card = new ExecutionCard
    {
        Title = request.Title,
        Description = request.Description,
        Status = ExecutionCardStatus.New,
        ExecutorType = ExecutorType.Human,
        BoardWorkItemId = 0, // 手动创建的卡片没有Azure WorkItem ID
        BoardId = request.BoardId ?? "Manual", // 手动创建的卡片使用指定或默认BoardId
        IsManualCreated = true,
        ProjectRepositoryId = request.ProjectRepositoryId,
        SpecId = request.SpecId,
        CreatedAt = DateTime.UtcNow,
        LastUpdated = DateTime.UtcNow
    };
    
    await _repository.AddAsync(card);
    return Ok(card);
}

/// <summary>
/// 编辑手动看板卡片
/// </summary>
[HttpPut("UpdateManualCard")]
public async Task<IActionResult> UpdateManualCard([FromBody] ManualCardUpdateRequest request)
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return BadRequest("标题不能为空");
    }
    
    if (request.Title.Length > 200)
    {
        return BadRequest("标题长度不能超过200个字符");
    }
    
    var card = await _repository.FindByIdAsync(request.Id);
    if (card == null || !card.IsManualCreated)
    {
        return NotFound("卡片不存在或不是手动创建的");
    }
    
    card.Title = request.Title;
    card.Description = request.Description;
    card.BoardId = request.BoardId ?? card.BoardId;
    card.ProjectRepositoryId = request.ProjectRepositoryId;
    card.SpecId = request.SpecId;
    card.LastUpdated = DateTime.UtcNow;
    
    await _repository.UpdateAsync(card);
    return Ok(card);
}

/// <summary>
/// 删除手动看板卡片
/// </summary>
[HttpDelete("DeleteManualCard/{id}")]
public async Task<IActionResult> DeleteManualCard(int id)
{
    var card = await _repository.FindByIdAsync(id);
    if (card == null || !card.IsManualCreated)
    {
        return NotFound("卡片不存在或不是手动创建的");
    }
    
    await _repository.DeleteAsync(card);
    return Ok(new { message = "删除成功" });
}

/// <summary>
/// 获取所有手动创建的看板卡片（支持分页和筛选）
/// </summary>
[HttpGet("GetManualCards")]
public async Task<IActionResult> GetManualCards(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? search = null,
    [FromQuery] string? status = null)
{
    var query = _repository.Query().Where(c => c.IsManualCreated);
    
    // 搜索筛选
    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(c => c.Title.Contains(search));
    }
    
    // 状态筛选
    if (!string.IsNullOrWhiteSpace(status))
    {
        query = query.Where(c => c.Status == status);
    }
    
    // 计算总数
    var total = await query.CountAsync();
    
    // 分页
    var cards = await query
        .OrderByDescending(c => c.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    
    return Ok(new { total, items = cards });
}

// 创建请求模型
public class ManualCardCreateRequest
{
    [Required(ErrorMessage = "标题不能为空")]
    [StringLength(200, ErrorMessage = "标题长度不能超过200个字符")]
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public string? BoardId { get; set; }
    
    public int? ProjectRepositoryId { get; set; }
    
    public int? SpecId { get; set; }
}

// 更新请求模型
public class ManualCardUpdateRequest
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "标题不能为空")]
    [StringLength(200, ErrorMessage = "标题长度不能超过200个字符")]
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public string? BoardId { get; set; }
    
    public int? ProjectRepositoryId { get; set; }
    
    public int? SpecId { get; set; }
}
```

## 三、前端修改方案

### 1. API接口封装
**文件**: `src/frontend/vol.web/src/api/kanban.ts`

添加创建和编辑手动看板的API调用：
```typescript
// 手动创建看板卡片
export function createManualCard(data: {
  title: string;
  description?: string;
  boardId?: string;
  projectRepositoryId?: number;
  specId?: number;
}) {
  return request.post("/api/ekanban/ExecutionCard/CreateManualCard", data);
}

// 编辑手动看板卡片
export function updateManualCard(data: {
  id: number;
  title: string;
  description?: string;
  boardId?: string;
  projectRepositoryId?: number;
  specId?: number;
}) {
  return request.put("/api/ekanban/ExecutionCard/UpdateManualCard", data);
}

// 删除手动看板卡片
export function deleteManualCard(id: number) {
  return request.delete(`/api/ekanban/ExecutionCard/DeleteManualCard/${id}`);
}

// 获取所有手动创建的看板卡片
export function getManualCards(params: {
  page: number;
  pageSize: number;
  search?: string;
  status?: string;
}) {
  return request.get("/api/ekanban/ExecutionCard/GetManualCards", { params });
}

// 导出时添加createManualCard、updateManualCard、deleteManualCard和getManualCards
export default {
  getKanbanData,
  getExecutionCardById,
  getExecutionCardDetail,
  getCardPhaseProgress,
  getCardFileChanges,
  triggerReExecute,
  triggerSyncFromAzureBoards,
  triggerAiExecution,
  createManualCard, // 新增
  updateManualCard, // 新增
  deleteManualCard, // 新增
  getManualCards // 新增
};
```

### 2. 类型定义修改
**文件**: `src/frontend/vol.web/src/types/kanban.ts`

在ExecutionCard接口中添加IsManualCreated字段：
```typescript
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
  LastUpdated: Date; // 修正字段名称，与后端一致
  IsManualCreated: boolean; // 新增字段
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
```

### 3. 前端UI修改

**新增页面**: `src/frontend/vol.web/src/views/Kanban/ManualKanbanPage.vue`

```vue
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
        >
          <el-option label="全部" value="" />
          <el-option label="新建" value="New" />
          <el-option label="进行中" value="InProgress" />
          <el-option label="完成" value="Completed" />
          <el-option label="失败" value="Failed" />
        </el-select>
      </div>
      
      <!-- 卡片列表 -->
      <el-table
        :data="filteredCards"
        style="width: 100%"
        v-loading="loading"
        height="600px"
        border
        stripe
        :row-height="60"
        :virtual-scroll="true"
        :virtual-item-size="60"
        :virtual-scroll-threshold="100"
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
      <el-form :model="createForm" :rules="rules" ref="createFormRef">
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
      <el-form :model="editForm" :rules="rules" ref="editFormRef">
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
import { ref, computed, onMounted, watch } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import * as kanbanApi from '@/api/kanban';
import type { ExecutionCard, ManualCardCreateRequest, ManualCardUpdateRequest } from '@/types/kanban';

// 响应式数据
const loading = ref(false);
const searchQuery = ref('');
const statusFilter = ref('');
const currentPage = ref(1);
const pageSize = ref(10);

// 监听搜索和筛选变化
watch([searchQuery, statusFilter], () => {
  currentPage.value = 1;
  fetchCards();
});

// 对话框相关
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

// 验证规则
const rules = {
  title: [
    { required: true, message: '请输入标题', trigger: 'blur' },
    { max: 200, message: '标题长度不能超过200个字符', trigger: 'blur' }
  ]
};

// 计算属性
const total = ref(0);
const filteredCards = ref<ExecutionCard[]>([]);

// 方法
async function fetchCards() {
  loading.value = true;
  try {
    const response = await kanbanApi.getManualCards({
      page: currentPage.value,
      pageSize: pageSize.value,
      search: searchQuery.value,
      status: statusFilter.value
    });
    filteredCards.value = response.data.items;
    total.value = response.data.total;
  } catch (error) {
    ElMessage.error('获取卡片列表失败');
    console.error('Failed to fetch cards:', error);
  } finally {
    loading.value = false;
  }
}

// 生命周期
onMounted(() => {
  fetchCards();
});

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

async function handleCreate() {
  if (!createFormRef.value) return;
  
  await createFormRef.value.validate(async (valid: boolean) => {
    if (valid) {
      try {
        await kanbanApi.createManualCard(createForm.value);
        ElMessage.success('创建成功');
        createDialogVisible.value = false;
        await fetchCards();
      } catch (error) {
        ElMessage.error('创建失败');
        console.error('Failed to create card:', error);
      }
    }
  });
}

async function handleUpdate() {
  if (!editFormRef.value) return;
  
  await editFormRef.value.validate(async (valid: boolean) => {
    if (valid) {
      try {
        await kanbanApi.updateManualCard(editForm.value);
        ElMessage.success('更新成功');
        editDialogVisible.value = false;
        await fetchCards();
      } catch (error) {
        ElMessage.error('更新失败');
        console.error('Failed to update card:', error);
      }
    }
  });
}

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
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除失败');
      console.error('Failed to delete card:', error);
    }
  }
}

function getStatusTagType(status: string): string {
  const statusMap: Record<string, string> = {
    New: 'info',
    InProgress: 'warning',
    Completed: 'success',
    Failed: 'danger'
  };
  return statusMap[status] || 'info';
}

function getStatusText(status: string): string {
  const statusMap: Record<string, string> = {
    New: '新建',
    InProgress: '进行中',
    Completed: '完成',
    Failed: '失败'
  };
  return statusMap[status] || status;
}

function canEdit(card: ExecutionCard): boolean {
  return card.Status === 'New' || card.Status === 'Failed';
}

function handleSizeChange(size: number) {
  pageSize.value = size;
  currentPage.value = 1;
  fetchCards();
}

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
}
</style>
```

**修改路由配置**: `src/frontend/vol.web/src/router/index.ts`

添加手动看板页面的路由配置：

```typescript
import { createRouter, createWebHistory } from 'vue-router';
import ManualKanbanPage from '@/views/Kanban/ManualKanbanPage.vue';

const routes = [
  // 现有路由
  {
    path: '/kanban',
    name: 'Kanban',
    component: () => import('@/views/Kanban/Index.vue')
  },
  // 新增手动看板路由
  {
    path: '/kanban/manual',
    name: 'ManualKanban',
    component: ManualKanbanPage,
    meta: {
      title: '手动看板管理'
    }
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

export default router;
```

**修改导航菜单**: `src/frontend/vol.web/src/components/Layout/Sidebar.vue`

在导航菜单中添加手动看板管理入口：

```vue
<template>
  <el-menu
    :default-active="activeMenu"
    class="sidebar-menu"
    router
  >
    <!-- 现有菜单项 -->
    <el-menu-item index="/kanban">
      <el-icon><Board /></el-icon>
      <span>看板管理</span>
    </el-menu-item>
    <!-- 新增手动看板菜单项 -->
    <el-menu-item index="/kanban/manual">
      <el-icon><Edit /></el-icon>
      <span>手动看板管理</span>
    </el-menu-item>
  </el-menu>
</template>
```

### 4. 看板卡片修改
**文件**: `src/frontend/vol.web/src/components/Kanban/KanbanCard.vue`

在卡片中显示IsManualCreated字段，区分手动创建和Azure同步的看板：

```vue
<template>
  <div class="kanban-card" :class="{ 'manual-created': card.IsManualCreated }">
    <!-- 现有内容 -->
    
    <!-- 添加来源标识 -->
    <div class="card-source" v-if="card.IsManualCreated">
      <el-tag size="small" type="info">手动创建</el-tag>
    </div>
    
    <!-- 其他现有内容 -->
  </div>
</template>

<script setup lang="ts">
import type { ExecutionCard } from '@/types/kanban';

const props = defineProps<{
  card: ExecutionCard;
}>();
</script>

<style scoped>
.kanban-card {
  position: relative;
  /* 现有样式 */
}

.manual-created {
  border-left: 4px solid #409eff;
}

.card-source {
  position: absolute;
  top: 10px;
  right: 10px;
  z-index: 1;
}
</style>
```

## 四、实现步骤

### 1. 数据库修改
- **添加字段**：为ExecutionCards表添加IsManualCreated字段
- **设置默认值**：设置默认值为false（因为现有数据都是从Azure同步的）
- **执行迁移**：执行数据库迁移脚本

**数据库迁移脚本**（SQL Server）：
```sql
-- 添加IsManualCreated字段
ALTER TABLE ExecutionCards
ADD IsManualCreated BIT NOT NULL DEFAULT 0;

-- 创建索引（可选，提高查询性能）
CREATE INDEX IX_ExecutionCards_IsManualCreated
ON ExecutionCards(IsManualCreated);
```

### 2. 后端修改
- **修改模型**：修改ExecutionCard模型，添加IsManualCreated字段及相关可选字段
- **添加接口**：在ExecutionCardController中添加以下接口
  - CreateManualCard：创建手动看板卡片
  - UpdateManualCard：编辑手动看板卡片
  - DeleteManualCard：删除手动看板卡片
  - GetManualCards：获取所有手动创建的看板卡片
- **添加请求模型**：添加ManualCardCreateRequest和ManualCardUpdateRequest模型
- **添加验证**：为请求模型添加数据验证

### 3. 前端修改
- **类型定义**：修改types/kanban.ts，添加IsManualCreated字段及相关请求类型
- **API封装**：修改api/kanban.ts，添加createManualCard、updateManualCard、deleteManualCard、getManualCards接口及导出
- **创建页面**：创建ManualKanbanPage.vue页面，实现完整的增删改查功能
- **修改路由**：修改router/index.ts，添加手动看板页面的路由配置
- **修改导航**：修改Layout/Sidebar.vue，添加手动看板管理菜单入口
- **修改卡片**：修改KanbanCard.vue，显示IsManualCreated字段

### 4. 部署步骤
1. **备份数据库**：在执行数据库迁移前，确保对数据库进行备份
2. **执行迁移**：运行数据库迁移脚本，添加IsManualCreated字段
3. **部署后端**：部署修改后的后端代码
4. **部署前端**：部署修改后的前端代码
5. **验证部署**：检查系统是否正常运行，验证手动看板功能

### 5. 测试验证
- **功能测试**：测试手动创建、编辑、删除、查询功能
- **集成测试**：测试手动看板与现有Azure同步功能的集成
- **UI测试**：测试前端组件的显示和交互
- **性能测试**：验证系统性能是否受到影响

## 五、测试计划

### 1. 数据库测试
- **验证迁移**：验证数据库迁移脚本执行成功
- **验证默认值**：验证现有数据的IsManualCreated字段默认值为false
- **验证新数据**：验证新创建的手动看板IsManualCreated字段为true
- **验证索引**：验证索引创建成功（如果添加了索引）

### 2. 功能测试
- **创建功能**：测试手动创建看板的流程，包括必填字段验证
- **编辑功能**：测试编辑手动看板的流程，包括状态限制
- **删除功能**：测试删除手动看板的流程，包括确认对话框
- **查询功能**：测试获取手动看板列表的功能
- **同步功能**：测试Azure同步看板的流程，确保不受影响
- **区分功能**：验证两种来源的看板是否正确区分

### 3. UI测试
- **页面显示**：验证ManualKanbanPage页面是否正常显示
- **对话框**：验证创建、编辑、删除操作的对话框是否正常显示
- **标识显示**：验证看板卡片上的来源标识是否正确显示
- **自动刷新**：验证操作成功后是否自动刷新看板
- **响应式**：验证页面在不同屏幕尺寸下的显示效果
- **导航测试**：验证从导航菜单进入手动看板页面是否正常

### 4. 集成测试
- **执行流程**：测试手动创建的看板是否能正常执行
- **同步流程**：测试Azure同步的看板是否能正常执行
- **API测试**：验证所有API接口是否正常工作
- **错误处理**：测试异常情况下的错误处理

### 5. 性能测试
- **响应时间**：测试API接口的响应时间
- **页面加载**：测试ManualKanbanPage页面的加载时间
- **数据库性能**：测试添加字段后数据库查询性能

## 六、技术风险评估

### 1. 数据库迁移风险
- **影响范围**：需要修改现有数据库表结构
- **风险级别**：中
- **缓解措施**：
  - 在非高峰期执行迁移
  - 提前备份数据库
  - 使用默认值确保现有数据不受影响
  - 考虑添加索引提高查询性能

### 2. 前端组件风险
- **影响范围**：新增组件需要与现有UI风格一致
- **风险级别**：低
- **缓解措施**：
  - 遵循现有组件的设计规范
  - 充分测试组件的各种状态
  - 实现完善的错误处理和加载状态

### 3. API接口风险
- **影响范围**：新增和修改API接口
- **风险级别**：中
- **缓解措施**：
  - 添加数据验证确保输入安全
  - 实现统一的错误处理
  - 添加适当的权限控制
  - 处理并发操作冲突

### 4. 兼容性风险
- **影响范围**：需要确保与现有功能的兼容性
- **风险级别**：低
- **缓解措施**：
  - 前端添加默认值处理，确保旧数据正常显示
  - 保持现有Azure同步功能不变
  - 充分测试集成场景

### 5. 性能风险
- **影响范围**：新增字段和组件可能影响系统性能
- **风险级别**：低
- **缓解措施**：
  - 合理设计数据库索引
  - 优化前端组件渲染
  - 实现分页加载减少数据传输

### 6. 安全性风险
- **影响范围**：API接口可能存在安全隐患
- **风险级别**：中
- **缓解措施**：
  - 添加适当的权限控制
  - 实现请求频率限制
  - 对输入数据进行严格验证
  - 添加操作审计日志

## 七、预期效果

### 1. 用户体验
- **直观操作**：提供独立的手动看板管理页面，操作界面清晰直观
- **完整功能**：支持完整的增删改功能，满足用户的各种操作需求
- **清晰区分**：通过标识清晰区分手动创建和Azure同步的看板，便于用户识别
- **平滑过渡**：保持现有Azure同步功能不变，确保系统平稳升级

### 2. 功能完整性
- **执行流程**：手动创建的看板具有与Azure同步看板相同的执行流程
- **操作支持**：支持所有现有看板操作（执行、重新执行、查看详情等）
- **生命周期管理**：提供独立的管理页面，实现对手动看板的完整生命周期管理
- **数据一致性**：确保手动看板与Azure同步看板数据的一致性

### 3. 系统稳定性
- **最小影响**：最小化对现有功能的影响，确保Azure同步功能正常运行
- **数据可靠**：确保数据一致性和可靠性，特别是在数据库迁移过程中
- **错误处理**：良好的错误处理和用户反馈机制，提升系统稳定性
- **性能稳定**：确保系统性能不受新增功能的影响

### 4. 可维护性
- **模块化设计**：将手动看板管理功能独立出来，便于后续维护
- **代码规范**：清晰的代码结构和文档，降低后续开发和维护成本
- **架构一致性**：遵循现有代码规范和架构，保持系统的一致性
- **可扩展性**：设计考虑未来可能的功能扩展

此方案遵循了最小化改动原则，只在必要的地方添加新功能，同时保持系统的稳定性和可维护性。通过完善的测试计划和风险评估，确保功能的顺利实现和稳定运行。