<template>
  <div class="project-container">
    <el-card class="box-card">
      <template #header>
        <div class="card-header">
          <span>项目仓库管理</span>
          <el-button type="primary" @click="handleAdd">新增项目</el-button>
        </div>
      </template>
      <el-table :data="tableData" border style="width: 100%">
        <el-table-column prop="Id" label="ID" width="80" />
        <el-table-column prop="Name" label="项目名称" width="200" />
        <el-table-column prop="LocalWorkingDir" label="本地工作目录" min-width="250" />
        <el-table-column prop="GitRemoteUrl" label="Git 远程地址" min-width="250" />
        <el-table-column prop="DefaultBranch" label="默认分支" width="120" />
        <el-table-column prop="IsActive" label="状态" width="100">
          <template #default="{ row }">
            <el-tag :type="row.IsActive ? 'success' : 'info'">
              {{ row.IsActive ? '启用' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" size="small" @click="handleEdit(row)">编辑</el-button>
            <el-button link type="danger" size="small" @click="handleDelete(row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 新增/编辑对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="isEdit ? '编辑项目' : '新增项目'"
      width="600px"
      destroy-on-close
    >
      <el-form ref="formRef" :model="formData" label-width="120px">
        <el-form-item label="项目名称" required>
          <el-input v-model="formData.Name" placeholder="请输入项目名称" />
        </el-form-item>
        <el-form-item label="本地工作目录" required>
          <el-input v-model="formData.LocalWorkingDir" placeholder="例如：/root/workspace/my-project" />
        </el-form-item>
        <el-form-item label="Git 远程地址">
          <el-input v-model="formData.GitRemoteUrl" placeholder="例如：https://github.com/username/repo.git" />
        </el-form-item>
        <el-form-item label="默认分支">
          <el-input v-model="formData.DefaultBranch" placeholder="例如：main" />
        </el-form-item>
        <el-form-item label="项目描述">
          <el-input
            v-model="formData.Description"
            type="textarea"
            :rows="3"
            placeholder="请输入项目描述"
          />
        </el-form-item>
        <el-form-item label="是否启用">
          <el-switch v-model="formData.IsActive" />
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="dialogVisible = false">取消</el-button>
          <el-button type="primary" @click="handleSubmit">确定</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import type { ProjectRepository } from "@/types/kanban";
import {
  getProjectList,
  getProjectById,
  createProject,
  updateProject,
  deleteProject,
} from "@/api/project";

const tableData = ref<ProjectRepository[]>([]);
const dialogVisible = ref(false);
const formRef = ref();
const isEdit = ref(false);
const formData = reactive<Partial<ProjectRepository>>({
  Name: "",
  LocalWorkingDir: "",
  GitRemoteUrl: "",
  DefaultBranch: "main",
  Description: "",
  IsActive: true,
});
let currentEditId = ref<number | null>(null);

const loadData = async () => {
  const res = await getProjectList();
  if (res.code === 200) {
    tableData.value = res.data;
  }
};

const handleAdd = () => {
  isEdit.value = false;
  currentEditId.value = null;
  resetForm();
  dialogVisible.value = true;
};

const handleEdit = async (row: ProjectRepository) => {
  isEdit.value = true;
  currentEditId.value = row.Id;
  const res = await getProjectById(row.Id);
  if (res.code === 200) {
    Object.assign(formData, res.data);
    dialogVisible.value = true;
  } else {
    ElMessage.error("获取项目信息失败");
  }
};

const handleDelete = (row: ProjectRepository) => {
  ElMessageBox.confirm(`确定要删除项目 "${row.Name}" 吗？`, "提示", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  }).then(async () => {
    const res = await deleteProject(row.Id);
    if (res.code === 200) {
      ElMessage.success("删除成功");
      loadData();
    }
  }).catch(() => {});
};

const resetForm = () => {
  Object.assign(formData, {
    Name: "",
    LocalWorkingDir: "",
    GitRemoteUrl: "",
    DefaultBranch: "main",
    Description: "",
    IsActive: true,
  });
};

const handleSubmit = async () => {
  if (!formData.Name || !formData.LocalWorkingDir || !formData.DefaultBranch) {
    ElMessage.warning("请填写必填项");
    return;
  }

  let res;
  if (isEdit.value && currentEditId.value) {
    res = await updateProject(formData as ProjectRepository);
  } else {
    res = await createProject(formData);
  }

  if (res.code === 200) {
    ElMessage.success(isEdit.value ? "更新成功" : "创建成功");
    dialogVisible.value = false;
    loadData();
  }
};

onMounted(() => {
  loadData();
});
</script>

<style scoped>
.project-container {
  padding: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.box-card {
  margin-bottom: 20px;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
}
</style>
