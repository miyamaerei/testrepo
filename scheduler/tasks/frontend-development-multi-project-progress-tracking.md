# 执行任务：E-Kanban 前端功能开发 - 多项目支持 + 细粒度进度跟踪

## 任务说明

后端已经完成了功能扩展，现在需要前端配套开发：

新增后端功能：
1. `ProjectRepository` - 项目仓库实体，支持多项目管理
2. `TaskPhaseProgress` - 六阶段进度跟踪实体
3. `TaskFileChange` - 文件变更记录实体
4. `ExecutionCard` 新增 `ProjectRepositoryId` 字段关联项目

需要前端配套实现这些功能。

## 开发前必读

请先阅读：
- `docs/NEW_AI_ONBOARDING_GUIDE.md` - E-Kanban 开发规范
- `scheduler/EXECUTION_NOTE.md` - 注意事项

## 技术栈

- Vue 3 + TypeScript + Vite
- Element Plus
- 现有 API 后端已经实现，只需要前端展示和交互

## 具体开发任务

按顺序完成：

### 批次 1：更新类型定义

1. ✅ `src/frontend/vol.web/src/types/kanban.ts`
   - 添加 `DevelopmentPhase` 枚举
   - 添加 `PhaseStatus` 枚举
   - 添加 `ChangeType` 枚举
   - 添加 `ProjectRepository` 接口定义
   - 添加 `TaskPhaseProgress` 接口定义
   - 添加 `TaskFileChange` 接口定义
   - 更新 `ExecutionCard` 接口，添加 `projectRepositoryId?: number` 字段

### 批次 2：添加 API 接口

1. ✅ `src/frontend/vol.web/src/api/`
   - 添加 `project.ts` - 项目 CRUD API
   - 添加 `phaseProgress.ts` - 阶段进度 API
   - 添加 `fileChange.ts` - 文件变更 API
   - 或者在 `kanban.ts` 中添加获取卡片详情（包含阶段和文件）的 API

### 批次 3：新增页面

1. ✅ `src/frontend/vol.web/src/views/Project/Index.vue` - 项目列表管理页面
   - 支持查看项目列表
   - 支持新增/编辑/删除项目
   - 表单包含：名称、本地工作目录、Git 远程地址、默认分支、描述

### 批次 4：增强卡片详情

1. ✅ 修改 `src/frontend/vol.web/src/views/Kanban/Index.vue` - 卡片详情对话框
   - 如果有关联项目，显示项目信息
   - 添加"阶段进度"标签页，展示每个阶段的当前状态
   - 添加"文件变更"标签页，展示该任务所有变更的文件列表

2. ✅ 阶段进度展示：
   - 列出六个阶段：需求分析、代码盘点、需求映射、增量开发、验证测试、知识沉淀
   - 用不同颜色标识状态：未开始/进行中/已完成
   - 显示开始时间、完成时间
   - 可展开查看阶段日志

3. ✅ 文件变更展示：
   - 列出所有变更的文件
   - 用不同颜色标识：新增(绿色)、修改(黄色)、删除(灰色)
   - 显示 commit hash（如果有）

### 批次 5：路由配置

1. ✅ `src/frontend/vol.web/src/router/index.ts`
   - 添加项目管理页面路由

### 批次 6：验证测试

1. ✅ `npm run build` 编译零错误
2. ✅ 测试项目列表增删改查
3. ✅ 测试看板卡片详情显示阶段进度和文件变更

## 开发规范

- 严格遵循现有代码风格
- TypeScript 类型要正确
- 保持和现有 UI 风格一致（Element Plus）
- 小步提交，每个批次一个提交

## 验收标准

- [ ] 类型定义更新完成
- [ ] API 接口添加完成
- [ ] 项目管理页面实现完成
- [ ] 卡片详情中阶段进度展示完成
- [ ] 卡片详情中文件变更展示完成
- [ ] 路由配置完成
- [ ] `npm run build` 编译零错误
- [ ] 功能测试正常

完成后提交到 `feature/multi-project-progress-tracking` 分支。
