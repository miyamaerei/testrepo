// 项目仓库 API 封装
import request from "@/api/http";
import type { ProjectRepository } from "@/types/kanban";

// 获取所有项目列表
export function getProjectList() {
  return request({
    url: "/api/ekanban/ProjectRepository/GetAll",
    method: "get",
  });
}

// 获取单个项目
export function getProjectById(id: number) {
  return request({
    url: `/api/ekanban/ProjectRepository/GetById?id=${id}`,
    method: "get",
  });
}

// 新增项目
export function createProject(data: Partial<ProjectRepository>) {
  return request({
    url: "/api/ekanban/ProjectRepository/Create",
    method: "post",
    data,
  });
}

// 更新项目
export function updateProject(data: ProjectRepository) {
  return request({
    url: "/api/ekanban/ProjectRepository/Update",
    method: "post",
    data,
  });
}

// 删除项目
export function deleteProject(id: number) {
  return request({
    url: `/api/ekanban/ProjectRepository/Delete?id=${id}`,
    method: "post",
  });
}

export default {
  getProjectList,
  getProjectById,
  createProject,
  updateProject,
  deleteProject,
};
