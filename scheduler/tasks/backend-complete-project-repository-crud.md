# 执行任务：补全 ProjectRepository 后端 CRUD 开发

## 任务说明
前端已经开发完成项目管理页面，后端实体和仓储层已创建，但缺少 Controller、Service 接口/实现，需要补全。

## 当前已有：
- ✅ 实体: `E_Kanban.Backend.Models.ProjectRepository`
- ✅ 仓储接口: `E_Kanban.Backend.IRepository.IProjectRepositoryRepository`
- ✅ 仓储实现: `E_Kanban.Backend.Repository.ProjectRepositoryRepository`
- ✅ 前端路由: `/kanban/project` 已配置
- ✅ 前端 API: `src/frontend/vol.web/src/api/project.ts` 已创建
- ✅ 前端页面: `Project/Index.vue` 已开发完成

## 需要完成：

### 批次 1：添加 Service 接口和实现
1. **IService 接口** - `src/modules/E-Kanban.Backend/IServices/IProjectRepositoryService.cs`
   - 添加接口方法：`GetAllAsync()`, `GetByIdAsync(int id)`, `CreateAsync(ProjectRepository entity)`, `UpdateAsync(ProjectRepository entity)`, `DeleteAsync(int id)`

2. **Service 实现** - `src/modules/E-Kanban.Backend/Services/ProjectRepositoryService.cs`
   - 注入 `IProjectRepositoryRepository`
   - 实现接口方法，调用仓储

### 批次 2：添加 Controller
1. **Controller** - `src/backend/VOL.WebApi/Controllers/EKanban/ProjectRepositoryController.cs`
   - 路由: `[Route("api/ekanban/[controller]/[action]")]`
   - 实现以下 Action:
     - `GetAll` - 获取所有列表
     - `GetById` - 根据 ID 获取单个
     - `Create` - 新增
     - `Update` - 更新
     - `Delete` - 删除
   - 遵循现有 `ExecutionCardController.cs` 的代码风格

### 批次 3：验证 DI 注册
1. 检查 `src/modules/E-Kanban.Backend/DI/ModuleInitializer.cs`
   - 确保 Service 和 Repository 已经注册
   - 如果没有注册，添加注册代码

### 批次 4：编译验证
1. 执行编译: `dotnet build E-Kanban.Backend`
2. 修复编译错误，确保零错误编译通过

### 批次 5：菜单添加说明
1. 整理出需要在系统后台添加的菜单配置说明
   - 父菜单: E-Kanban (已存在)
   - 子菜单: 项目管理
   - 路径: `/kanban/project`
   - 权限配置说明

## 开发规范
- 严格遵循现有代码风格
- 使用依赖注入
- 遵循 VOL 框架的约定

## 验收标准
- [ ] IProjectRepositoryService 接口已添加
- [ ] ProjectRepositoryService 实现已添加
- [ ] ProjectRepositoryController 控制器已添加
- [ ] DI 注册正确
- [ ] dotnet build 编译零错误
- [ ] 整理出菜单配置说明

完成后提交到 `feature/multi-project-progress-tracking` 分支，并推送到远程。
