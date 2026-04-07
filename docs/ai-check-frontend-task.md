# AI 任务：检查 E-Kanban 前端接口对接情况

## 🎯 任务目标

检查 `src/frontend/vol.web` 项目中，是否已经对接了后端 E-Kanban 提供的所有 API 接口，找出哪些接口已经对接，哪些遗漏了，输出对接清单和结论。

## 📋 检查范围

需要检查以下所有后端 API 接口是否已经在前端对接（在 `src/frontend/vol.web/src` 中是否有对应的 API 调用代码）：

### 1. ExecutionCardController 执行卡片相关

| 接口 | 方法 | 是否需要检查 |
|------|------|-------------|
| `GetById` | GET | ✅ 必须检查 |
| `GetKanbanData` | GET | ✅ 必须检查 |
| `TriggerReExecute` | POST | ✅ 必须检查 |

### 2. AiExecutionController AI 执行相关

| 接口 | 方法 | 是否需要检查 |
|------|------|-------------|
| `TriggerExecution` | POST | ✅ 必须检查 |

### 3. ProjectRepositoryController 项目仓库管理

| 接口 | 方法 | 是否需要检查 |
|------|------|-------------|
| `GetAll` | GET | ✅ 必须检查 |
| `GetById` | GET | ✅ 必须检查 |
| `Create` | POST | ✅ 必须检查 |
| `Update` | PUT | ✅ 必须检查 |
| `Delete` | DELETE | ✅ 必须检查 |

### 4. AzureBoardsSyncController 同步

| 接口 | 方法 | 是否需要检查 |
|------|------|-------------|
| `TriggerSync` | POST | ✅ 必须检查 |

## 🔍 检查步骤

1. **查找 API 封装文件**：搜索 `src/frontend/vol.web/src/api/` 目录下是否有 `kanban.ts` 或类似文件
2. **检查每个接口是否有封装**：每个接口是否都有对应的封装方法
3. **检查页面调用**：检查相关页面（KanbanBoard、ProjectRepository 等）是否实际调用了这些 API 方法
4. **输出检查结果**：整理成对接状态表格

## 📊 输出要求

必须输出以下格式的检查结果：

### 对接状态总览

| 控制器 | 接口名称 | 方法 | API 已封装 | 页面已调用 | 对接状态 |
|--------|----------|------|------------|------------|----------|
| ExecutionCardController | GetById | GET | | | |
| ExecutionCardController | GetKanbanData | GET | | | |
| ExecutionCardController | TriggerReExecute | POST | | | |
| AiExecutionController | TriggerExecution | POST | | | |
| ProjectRepositoryController | GetAll | GET | | | |
| ProjectRepositoryController | GetById | GET | | | |
| ProjectRepositoryController | Create | POST | | | |
| ProjectRepositoryController | Update | PUT | | | |
| ProjectRepositoryController | Delete | DELETE | | | |
| AzureBoardsSyncController | TriggerSync | POST | | | |

### 对接状态说明

- ✅ **已完成** - API 已封装且页面已调用
- ⚠️ **部分完成** - API 已封装但页面未调用（或反之）
- ❌ **未对接** - API 未封装也未调用

### 总结结论

- 已对接接口数量 / 总接口数量
- 列出遗漏的接口（如果有）
- 给出修复建议

## ⚠️ 注意事项

- 只检查 **E-Kanban 业务接口**，不要检查系统原有其他接口（如 Sys 系列控制器接口）
- 如果 API 已经封装，但还没有页面调用，也算部分完成
- 如果接口已经在页面中直接调用（没有封装），也算已对接，但建议补充 API 封装

## ✅ 验收标准

- 检查结果完整，覆盖所有 11 个接口
- 每个接口的状态判断准确
- 输出清晰的表格和结论
- 指出遗漏的接口，便于修复

---

*任务创建时间：2026-04-04*
