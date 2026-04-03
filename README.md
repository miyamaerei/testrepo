# E-Kanban - 执行型 Kanban 协调系统

> 基于 Azure Boards + GitHub Copilot CLI 的人机协同执行平台，带 AI 防偷懒机制

## 🎯 项目介绍

E-Kanban 是一个**以可视化 Kanban 为核心的人机协同执行平台**：

- 承接 Azure Boards 中的工作项
- 将工作项转化为可执行、可验证、可追溯的执行闭环
- 支持人 / AI / 系统三种角色统一执行模型
- **AI 分批迭代执行，内置超时检测和自动重试防偷懒机制**
- 通过 Spec 校验保证任务真正完成，避免"假完成"

## ✨ 核心功能

| 功能 | 说明 |
|------|------|
| **Azure Boards 集成** | 定时拉取工作项，状态回写 |
| **Kanban 可视化** | 按状态分组展示，一目了然 |
| **统一 Submit API** | 人/AI/系统统一提交入口 |
| **Spec Engine** | AI 生成验收标准，自动评估是否完成 |
| **GitHub Copilot CLI 集成** | 直接调用 Copilot CLI 执行开发任务 |
| **AI 防偷懒机制** | 超时检测，自动重试，人工干预支持 |
| **分批迭代执行** | 支持大任务多轮迭代开发 |
| **多项目支持** | 支持管理多个项目仓库，每个任务可关联到不同项目 |
| **细粒度进度跟踪** | 跟踪 S/E 六阶段每个阶段的进度状态 |
| **文件变更记录** | 自动记录任务修改过的所有文件 |

## 🛠️ 技术栈

- **后端**: .NET 8 + C# + Vol.Core 框架
- **ORM**: SqlSugar
- **定时任务**: Quartz.NET
- **前端**: Vue 3 + Vite + Element Plus
- **数据库**: SQL Server 2019+
- **AI 执行**: GitHub Copilot CLI

## 📋 前置要求

- .NET 8 SDK
- Node.js 18+ (前端构建)
- SQL Server 2019+
- GitHub Copilot CLI 已安装并认证
- Azure DevOps 组织和项目，已生成 PAT

## ⚙️ 配置说明

### 1. 数据库连接

修改 `VOL.WebApi/appsettings.json` 中的连接字符串：

```json
"Connection": {
  "DBType": "MsSql",
  "DbConnectionString": "Data Source=your-server;Initial Catalog=E-Kanban;Persist Security Info=True;User ID=sa;Password=your-password;Connect Timeout=500;Encrypt=True;TrustServerCertificate=True"
}
```

### 2. Azure Boards 配置

```json
"AzureBoards": {
  "OrganizationUrl": "https://dev.azure.com/your-organization",
  "Project": "your-project",
  "PersonalAccessToken": "your-pat-token",
  "SyncCronExpression": "0 0 */6 * * ?"
}
```

### 3. AI 执行防偷懒配置

```json
"AiExecution": {
  "InProgressTimeoutMinutes": 120,      // InProgress 超时时间（默认 2 小时）
  "MaxAutoRetries": 3,                  // 最大自动重试次数
  "CheckIntervalMinutes": 15            // 检查间隔
}
```

## 🚀 构建运行

### 后端构建

```bash
cd /path/to/E-Kanban
dotnet restore VOL.sln
dotnet build VOL.sln --configuration Release
dotnet publish VOL.WebApi -c Release -o publish
```

### 前端构建

```bash
cd vol.web
npm install
npm run build
```

### Docker 构建

```bash
docker build -t e-kanban:latest .
docker run -d --name e-kanban -p 9991:9991 e-kanban:latest
```

### IIS 部署

1. 发布后端到文件夹
2. 将前端构建产物复制到 `vol.web/dist` 部署到 IIS 静态文件目录
3. 配置 IIS 站点，指向发布目录
4. 确保 ASP.NET Core 托管包已安装

## 📊 使用流程

1. **同步**: 点击"同步 Azure Boards"拉取工作项
2. **生成 Spec**: 系统自动为新卡片生成验收标准
3. **触发执行**: 就绪状态的 AI 任务可以触发执行
4. **自动执行**: Quartz 调度 Copilot CLI 执行任务
5. **自动提交**: 执行完成后自动调用 Submit API
6. **Spec 评估**: Spec Engine 判断是否通过
7. **状态流转**: 通过 → 完成；失败 → 回到就绪等待下一轮
8. **防偷懒检查**: 每 15 分钟扫描，超时任务自动重试

## 🧩 项目结构（已重构）

```
E-Kanban/
├── src/
│   ├── core/                        # Vol.Core 框架核心
│   ├── modules/
│   │   └── E-Kanban.Backend/        # E-Kanban 业务模块
│   │       ├── Models/              # 领域实体
│   │       ├── IRepository/         # 仓储接口
│   │       ├── Repository/          # 仓储实现
│   │       ├── IServices/           # 服务接口
│   │       ├── Services/            # 服务实现
│   │       ├── AiExecution/         # GitHub Copilot CLI 集成
│   │       ├── Specs/               # Spec 生成和评估
│   │       ├── Jobs/                # Quartz 定时任务
│   │       └── DI/                  # 依赖注入注册
│   ├── backend/
│   │   └── VOL.WebApi/              # 后端主项目
│   │       ├── Controllers/EKanban/ # API 控制器
│   │       └── appsettings.json     # 配置文件
│   └── frontend/
│       └── vol.web/                 # 前端项目
│           └── src/
│               ├── api/kanban.ts    # API 封装
│               ├── components/Kanban/ # 组件
│               ├── views/Kanban/    # 页面
│               └── router/index.js  # 路由配置
├── scheduler/                        # AI 任务调度配置
├── scripts/                          # 开发辅助脚本
├── deploy/                           # 部署配置
├── database/                         # 数据库建表脚本
├── docs/                             # 项目文档
├── VOL.sln                           # Vol.Core 解决方案
├── E-Kanban.sln                      # E-Kanban 主解决方案
└── README.md                         # 项目介绍
```

## 🧪 开发进度

- [x] 第一批次：基础设施层（实体、仓储、服务）
- [x] 第二批次：核心领域服务（Azure Boards 同步、Spec Engine、状态机、Submit API）
- [x] 第三批次：AI 调度和防偷懒（Copilot CLI 集成、Quartz 任务、超时检查）
- [x] 第四批次：API 控制器
- [x] 第五批次：前端开发
- [x] 第六批次：配置和部署文档
- [x] **v1.1 新增**: 多项目支持 - 项目仓库配置管理
- [x] **v1.1 新增**: 细粒度进度跟踪 - S/E 六阶段进度跟踪
- [x] **v1.1 新增**: 文件变更记录 - 自动跟踪任务修改的文件

**当前状态**: v1.1 功能扩展开发完成，编译零错误，可进行测试部署

## 📝 开发文档

- [开发引导](./docs/DEVELOPMENT-GUIDE.md)
- [结构化需求](./docs/requirements-structured.md)
- [执行计划](./docs/implementation-plan.md)
- [Vol.Core 开发规范](./docs/volcore-development-guide.md)

## 🎓 致谢

基于 [Vol.Core](https://github.com/RealKai42/NetCore_Vue) 框架开发，感谢原作者。

## 📄 许可证

MIT License
