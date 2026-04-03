# E-Kanban 开发任务：第五批次 前端开发

## 任务说明
按照 E-Kanban 开发计划，第五批次：创建 Vue 前端 Kanban 看板页面。

## 前置条件
- ✅ 第四批次 API 控制器已完成

## 开发规范
$(cat docs/volcore-development-guide.md)

## 需要创建的文件

### 5.1 API 封装
- 位置：`vol.web/src/api/ekanban.ts`
- 需要导出：
  - `getKanbanData()` - 获取看板数据
  - `getCardDetail(id)` - 获取卡片详情
  - `triggerReExecute(id)` - 触发重新执行
  - `syncFromAzureBoards()` - 触发同步
  - `getExecutionHistory(cardId)` - 获取执行历史

### 5.2 Kanban 卡片组件
- 位置：`vol.web/src/components/ekanban/KanbanCard.vue`
- 功能：
  - 展示卡片信息（标题、描述、状态、失败次数）
  - 显示重新执行按钮（当需要重试或人工干预时）
  - 点击卡片查看详情

### 5.3 Kanban 看板页面
- 位置：`vol.web/src/views/ekanban/Kanban/Index.vue`
- 功能：
  - 按状态分成四列展示（Ready / InProgress / Completed / Failed）
  - 每个卡片使用 KanbanCard 组件
  - 顶部有"同步 Azure Boards"按钮
  - 点击重新执行按钮触发重新执行
  - 刷新看板数据

### 5.4 添加路由配置
- 文件：`vol.web/src/router/index.ts`
- 添加 E-Kanban 路由
- 路径：`/ekanban/kanban`

## UI 要求
- 使用 Element Plus 组件
- 看板布局：横向四列，每列固定宽度
- 卡片有边框，hover 效果
- 不同状态用不同颜色标识：
  - Ready：蓝色
  - InProgress：黄色
  - Completed：绿色
  - Failed：红色
- 需要人工干预的卡片有明显标识

## 输出要求
1. 创建 `vol.web/src/api/ekanban.ts`
2. 创建 `vol.web/src/components/ekanban/KanbanCard.vue`
3. 创建 `vol.web/src/views/ekanban/Kanban/Index.vue`
4. 更新 `vol.web/src/router/index.ts` 添加路由
5. 在 `implementation-plan.md` 中更新所有任务状态为 ✅ 完成

## 检查清单
- [ ] 5.1 API 封装已创建
- [ ] 5.2 KanbanCard 组件已创建
- [ ] 5.3 Kanban 页面已创建
- [ ] 5.4 路由已添加
- [ ] 按状态分组展示正确
- [ ] 重新执行按钮功能正常
- [ ] 同步按钮功能正常
- [ ] 样式符合要求，状态颜色区分正确
- [ ] TypeScript 语法正确
