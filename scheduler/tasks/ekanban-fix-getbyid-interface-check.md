# AI 任务：修复 ProjectRepositoryController.GetById 接口遗漏调用问题

## 🎯 任务目标

在接口检查中发现，`ProjectRepositoryController.GetById` 接口已经在 `src/frontend/vol.web/src/api/project.ts` 中完成 API 封装，但是在 `Project/Index.vue` 项目管理页面的编辑功能中，没有调用这个接口从后端获取最新数据，而是直接使用表格中的缓存数据。

需要修复这个问题，确保编辑时获取最新数据。

## 🔧 修复内容

### 当前问题

在 `Project/Index.vue` 的 `handleEdit` 方法中：
```typescript
const handleEdit = (row: ProjectRepository) => {
  isEdit.value = true;
  currentEditId.value = row.Id;
  Object.assign(formData, {
    Id: row.Id,
    Name: row.Name,
    LocalWorkingDir: row.LocalWorkingDir,
    GitRemoteUrl: row.GitRemoteUrl,
    DefaultBranch: row.DefaultBranch,
    Description: row.Description,
    IsActive: row.IsActive,
  });
  dialogVisible.value = true;
};
```

直接使用了表格行 `row` 的数据填充表单，没有调用 `getProjectById` 从后端获取最新数据。

### 修复方案

修改 `handleEdit` 方法，调用 `getProjectById` 接口从后端获取最新数据后再填充表单，确保数据一致性。

## ✅ 验收标准

1. 修改 `handleEdit` 方法，调用 `getProjectById` 获取最新数据
2. 测试编辑功能正常工作
3. 前端编译无错误
4. 提交修改到远程分支
