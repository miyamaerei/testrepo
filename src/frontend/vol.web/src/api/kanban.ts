// E-Kanban API 封装
import request from "@/api/http";

// 获取看板数据（按状态分组）
export function getKanbanData() {
  return request({
    url: "/api/ekanban/ExecutionCard/GetKanbanData",
    method: "get",
  });
}

// 获取单个卡片详情
export function getExecutionCardById(id: number) {
  return request({
    url: `/api/ekanban/ExecutionCard/GetById?id=${id}`,
    method: "get",
  });
}

// 获取卡片详情（包含阶段进度和文件变更）
export function getExecutionCardDetail(id: number) {
  return request({
    url: `/api/ekanban/ExecutionCard/GetDetail?id=${id}`,
    method: "get",
  });
}

// 获取卡片阶段进度列表
export function getCardPhaseProgress(executionCardId: number) {
  return request({
    url: `/api/ekanban/TaskPhaseProgress/GetByCardId?executionCardId=${executionCardId}`,
    method: "get",
  });
}

// 获取卡片文件变更列表
export function getCardFileChanges(executionCardId: number) {
  return request({
    url: `/api/ekanban/TaskFileChange/GetByCardId?executionCardId=${executionCardId}`,
    method: "get",
  });
}

// 触发重新执行 AI 任务
export function triggerReExecute(id: number) {
  return request({
    url: `/api/ekanban/ExecutionCard/TriggerReExecute?id=${id}`,
    method: "post",
  });
}

// 触发从 Azure Boards 同步
export function triggerSyncFromAzureBoards() {
  return request({
    url: "/api/ekanban/AzureBoardsSync/TriggerSync",
    method: "post",
  });
}

// 触发 AI 执行
export function triggerAiExecution(id: number) {
  return request({
    url: `/api/ekanban/AiExecution/TriggerExecution?id=${id}`,
    method: "post",
  });
}

export default {
  getKanbanData,
  getExecutionCardById,
  getExecutionCardDetail,
  getCardPhaseProgress,
  getCardFileChanges,
  triggerReExecute,
  triggerSyncFromAzureBoards,
  triggerAiExecution,
};
