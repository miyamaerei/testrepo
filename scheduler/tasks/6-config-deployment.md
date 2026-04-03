# E-Kanban 开发任务：第六批次 配置和部署

## 任务说明
按照 E-Kanban 开发计划，第六批次：添加配置项、更新 Dockerfile、编写运行文档。

## 前置条件
- ✅ 前五批次所有任务已完成

## 需要完成的工作

### 6.1 添加配置项到 appsettings.json
位置：`VOL.WebApi/appsettings.json`

需要添加以下配置节点：
```json
"AzureBoards": {
  "OrganizationUrl": "https://dev.azure.com/your-org",
  "Project": "your-project",
  "PersonalAccessToken": "your-pat"
},
"AiExecution": {
  "CopilotCliPath": "copilot",
  "InProgressTimeoutMinutes": 120,
  "MaxAutoRetries": 3,
  "CheckIntervalMinutes": 15,
  "CommandTimeoutMinutes": 30
}
```

### 6.2 更新 Dockerfile
- 位置：项目根目录 `Dockerfile`
- 确保能正确构建 .NET 后端和 Vue 前端
- 多阶段构建，减小镜像体积

### 6.3 更新项目 README.md
位置：项目根目录 `README.md`

需要包含：
1. 项目介绍（E-Kanban 执行型 Kanban 协调系统）
2. 功能特性
3. 技术栈
4. 配置说明（Azure Boards PAT、AI 执行配置）
5. 构建步骤
6. 部署步骤（IIS 部署说明）
7. 使用说明

## 输出要求
1. 更新 `VOL.WebApi/appsettings.json` 添加配置
2. 更新 `Dockerfile`
3. 创建/更新 `README.md`
4. 在 `implementation-plan.md` 中更新所有任务状态为 ✅ 完成

## 检查清单
- [ ] 6.1 AzureBoards 配置已添加
- [ ] 6.1 AiExecution 配置已添加
- [ ] 6.2 Dockerfile 更新完成
- [ ] 6.3 README.md 包含所有必要信息
- [ ] 配置说明清晰
- [ ] 构建部署步骤清晰
