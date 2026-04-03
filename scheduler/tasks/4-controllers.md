# E-Kanban 开发任务：第四批次 API 控制器

## 任务说明
按照 E-Kanban 开发计划，第四批次：创建 API 控制器，暴露 HTTP 接口给前端调用。

## 前置条件
- ✅ 第三批次所有任务已完成（服务层、定时任务都已完成）

## 开发规范
$(cat docs/volcore-development-guide.md)

## 位置
所有控制器放在 `VOL.WebApi/Controllers/EKanban/`

## 需要创建的控制器

### 4.1 ExecutionCardController - Kanban 卡片核心接口
需要提供：
- `GetPageList` - 分页查询卡片
- `GetById` - 获取单个卡片详情
- `GetKanbanData` - 获取按状态分组的看板数据（给前端看板展示）
- `TriggerReExecute` - 手动触发重新执行（防偷懒功能）
- `TransitionTo` - 手动状态流转

### 4.2 AzureBoardsSyncController - Azure Boards 同步接口
需要提供：
- `TriggerSync` - 手动触发同步
- `GetSyncHistory` - 获取同步历史

### 4.3 AiExecutionController - AI 执行接口
需要提供：
- `GetExecutionHistory` - 获取卡片的执行历史（所有 ExecutionRun）
- `TriggerExecution` - 手动触发 AI 执行

## 代码风格要求
- 继承 `BaseController`
- 路由特性：`[Route("api/ekanban/[controller]/[action]")]`
- 使用依赖注入方式注入服务
- 返回格式遵循 VOL.Core 标准返回格式（`Success(data)`, `Fail(message)`）

## 输出要求
1. 在 `VOL.WebApi/Controllers/EKanban/` 创建三个控制器
2. 所有需要的接口都已实现
3. 遵循 VOL.Core 控制器规范
4. 包含重新执行接口正确集成 `TriggerReExecute
5. 在 `implementation-plan.md` 中更新任务状态为 ✅ 完成

## 检查清单
- [ ] 4.1 `ExecutionCardController.cs` 已创建，包含所有接口
- [ ] 4.2 `AzureBoardsSyncController.cs` 已创建
- [ ] 4.3 `AiExecutionController.cs` 已创建
- [ ] 路由配置正确
- [ ] 依赖注入正确
- [ ] 返回格式符合框架规范
- [ ] GetKanbanData 返回分组数据正确
- [ ] TriggerReExecute 集成正确
- [ ] 语法正确，可编译
