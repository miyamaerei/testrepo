// E-Kanban API 封装
import request from "@/api/http";

// 获取看板数据（按状态分组）
export function getKanbanData() {
  return request.get("/api/ekanban/ExecutionCard/GetKanbanData");
}

// 获取单个卡片详情
export function getExecutionCardById(id: number) {
  return request.get(`/api/ekanban/ExecutionCard/GetById?id=${id}`);
}

// 获取卡片详情（包含阶段进度和文件变更）
export function getExecutionCardDetail(id: number) {
  return request.get(`/api/ekanban/ExecutionCard/GetDetail?id=${id}`);
}

// 获取卡片阶段进度列表
export function getCardPhaseProgress(executionCardId: number) {
  return request.get(`/api/ekanban/TaskPhaseProgress/GetByCardId?executionCardId=${executionCardId}`);
}

// 获取卡片文件变更列表
export function getCardFileChanges(executionCardId: number) {
  return request.get(`/api/ekanban/TaskFileChange/GetByCardId?executionCardId=${executionCardId}`);
}

// 触发重新执行 AI 任务
export function triggerReExecute(id: number) {
  return request.post(`/api/ekanban/ExecutionCard/TriggerReExecute?id=${id}`);
}

// 触发从 Azure Boards 同步
export function triggerSyncFromAzureBoards() {
  return request.post("/api/ekanban/AzureBoardsSync/TriggerSync");
}

// 触发 AI 执行
export function triggerAiExecution(id: number) {
  return request.post(`/api/ekanban/AiExecution/TriggerExecution?id=${id}`);
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