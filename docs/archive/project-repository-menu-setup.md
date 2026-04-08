# 项目仓库管理 - 菜单配置说明

## 菜单配置

在系统后台添加以下菜单配置：

### 基本信息

| 项 | 值 |
|---|---|
| **父菜单** | E-Kanban (已存在) |
| **菜单名称** | 项目管理 |
| **路由地址** | `/kanban/project` |
| **组件路径** | `/Project/Index` |
| **是否外链** | 否 |
| **是否缓存** | 是 |
| **是否显示** | 是 |
| **菜单类型** | 菜单 (非按钮) |
| **权限标识** | `kanban:project:view` |

### 按钮权限配置

需要添加以下按钮权限：

| 按钮名称 | 权限标识 |
|---|---|
| 查询 | `kanban:project:query` |
| 新增 | `kanban:project:add` |
| 修改 | `kanban:project:edit` |
| 删除 | `kanban:project:delete` |

## API 端点

后端已提供以下 API：

| 方法 | 端点 | 说明 |
|---|---|---|
| GET | `/api/ekanban/ProjectRepository/GetAll` | 获取所有项目列表 |
| GET | `/api/ekanban/ProjectRepository/GetById?id={id}` | 根据 ID 获取单个项目 |
| POST | `/api/ekanban/ProjectRepository/Create` | 创建新项目 |
| PUT | `/api/ekanban/ProjectRepository/Update` | 更新项目信息 |
| DELETE | `/api/ekanban/ProjectRepository/Delete?id={id}` | 删除项目 |
