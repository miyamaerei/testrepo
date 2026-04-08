# 基于 Vol.Core (Vue.NetCore) 开发指南

本文档描述了如何在现有的 Vol.Core SqlSugar + Vue3 骨架基础上，开发 E-Kanban 新业务模块。

---

## 目录

- [1. 项目整体架构](#1-项目整体架构)
- [2. 后端开发规范](#2-后端开发规范)
  - [2.1 实体层（VOL.Entity）](#21-实体层volentity)
  - [2.2 仓储层（IRepositories + Repositories）](#22-仓储层irepositories--repositories)
  - [2.3 服务层（IServices + Services）](#23-服务层iservices--services)
  - [2.4 控制器（VOL.WebApi.Controllers）](#24-控制器volwebapicontrollers)
  - [2.5 定时任务（Quartz）](#25-定时任务quartz)
- [3. 前端开发规范](#3-前端开发规范)
  - [3.1 页面文件位置](#31-页面文件位置)
  - [3.2 页面模板](#32-页面模板)
  - [3.3 API 封装](#33-api-封装)
  - [3.4 路由配置](#34-路由配置)
- [4. 代码生成器使用](#4-代码生成器使用)
- [5. E-Kanban 模块开发计划](#5-ekanban-模块开发计划)

---

## 1. 项目整体架构

Vol.Core 采用经典分层架构：

```
VOL.Entity/           → 实体层，所有领域实体放这里
├── DomainModels/     → 业务领域实体
├── SystemModels/     → 系统实体（用户、角色等，已有）
└── BaseCore/        → 基类（BaseEntity）

VOL.Core/            → 核心层（已有）
├── BaseRepository/  → 基类仓储
├── Quartz/          → Quartz 定时任务支持
└── Jwt/             → JWT 认证

VOL.Sys/             → 系统模块（用户权限，已有）
├── IRepositories/
├── Repositories/
├── IServices/
└── Services/

EKanban/             → ← 我们新增的 E-Kanban 业务模块（遵循相同分层）
├── IRepositories/   → 仓储接口
├── Repositories/    → 仓储实现
├── IServices/       → 服务接口
├── Services/        → 服务实现
├── AiExecution/     → GitHub Copilot CLI 集成
├── Specs/           → Spec Engine
└── Jobs/            → Quartz 定时任务

VOL.WebApi/          → Web API 入口（已有）
└── Controllers/
    └── EKanban/    → ← 我们新增的 E-Kanban 控制器

vol.web/             → Vue3 前端（已有）
└── src/
    ├── views/
    │   └── ekanban/ → ← 我们新增的 E-Kanban 页面
    ├── components/
    │   └── ekanban/ → ← 我们新增的 E-Kanban 组件
    ├── api/
    │   └── ekanban/ → ← 我们新增的 API 封装
    └── router/
        └── index.ts → 需要添加路由
```

---

## 2. 后端开发规范

### 2.1 实体层（VOL.Entity）

#### 规则
- 所有业务实体放在 `VOL.Entity/DomainModels/`
- 实体必须继承 `VOL.Entity.SystemModels.BaseEntity`
- 使用 SqlSugar 特性标注主键、数据库类型等
- 遵循 Vol.Core 注释模板

#### 实体模板

```csharp
/*
 *代码由框架生成,任何更改都可能导致被代码生成器覆盖
 *如果数据库字段发生变化，请在代码生器重新生成此Model
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SqlSugar;
using VOL.Entity.SystemModels;

namespace VOL.Entity.DomainModels
{
    [Entity(TableCnName = "执行卡片", TableName = "ExecutionCards")]
    public partial class ExecutionCard : BaseEntity
    {
        /// <summary>
        ///主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        [Key]
        [Display(Name = "Id")]
        [Column(TypeName = "int")]
        [Required(AllowEmptyStrings = false)]
        public int Id { get; set; }

        /// <summary>
        ///关联BoardWorkItemID
        /// </summary>
        [Display(Name = "BoardWorkItemId")]
        [Column(TypeName = "int")]
        [Editable(true)]
        [Required(AllowEmptyStrings = false)]
        public int BoardWorkItemId { get; set; }

        // ... 其他字段
    }
}
```

> **注意**：Vol.Core 代码生成器会生成 `partial` 类，如果需要扩展业务逻辑，可以在同名目录新建 `partial` 文件添加自定义方法，不会被代码生成器覆盖。

---

### 2.2 仓储层

#### 规则
- 接口放在 `EKanban/IRepositories/`
- 实现放在 `EKanban/Repositories/`
- 继承 `BaseRepository<T>`，实现对应接口
- Vol.Core 会自动扫描注册到 Autofac

#### 接口模板

```csharp
using VOL.Entity.DomainModels;
using VOL.Core.BaseProvider;
using VOL.Entity.DomainModels;

namespace EKanban.IRepositories
{
    public partial interface IExecutionCardRepository : IBaseRepository<ExecutionCard>
    {
        // 自定义查询方法在这里定义
        Task<Dictionary<ExecutionCardStatus, List<ExecutionCard>>> GetCardsGroupedByStatusAsync();
    }
}
```

#### 实现模板

```csharp
using SqlSugar;
using VOL.Entity.DomainModels;
using VOL.Core.BaseProvider;
using EKanban.IRepositories;

namespace EKanban.Repositories
{
    public partial class ExecutionCardRepository : BaseRepository<ExecutionCard>, IExecutionCardRepository
    {
        public ExecutionCardRepository(ISqlSugarClient db) : base(db)
        {
        }

        public async Task<Dictionary<ExecutionCardStatus, List<ExecutionCard>>> GetCardsGroupedByStatusAsync()
        {
            var allCards = await Queryable()
                .OrderByDescending(c => c.LastUpdated)
                .ToListAsync();

            return allCards.GroupBy(c => c.Status)
                .ToDictionary(g => g.Key, g => g.ToList());
        }
    }
}
```

---

### 2.3 服务层

#### 规则
- 接口放在 `EKanban/IServices/`
- 实现放在 `EKanban/Services/`
- 继承 `ServiceBase<T, IRepository<T>>`，实现对应接口
- 必须添加 `IDependency` 接口，让 Autofac 自动注册
- 遵循 partial 类规则：框架生成的放在主文件，自定义业务放在 `Partial/` 目录下

#### 接口模板

```csharp
using VOL.Core.BaseProvider;
using VOL.Entity.DomainModels;
using VOL.Core.CacheManager;
using VOL.Core.Utilities;
using System.Linq.Expressions;

namespace EKanban.IServices
{
    public partial interface IExecutionCardService : IBaseService<ExecutionCard>
    {
        // 自定义服务方法在这里定义
        Task<PageGrid<ExecutionCard>> GetPageListAsync(PageGrid pageGrid);
    }
}
```

#### 实现模板

```csharp
/*
 *Author：jxx
 *Contact：283591387@qq.com
 *代码由框架生成,此处任何更改都可能导致被代码生成器覆盖
 *所有业务编写全部应在Partial文件夹下ExecutionCardService与IExecutionCardService中编写
 */
using EKanban.IRepositories;
using EKanban.IServices;
using VOL.Core.BaseProvider;
using VOL.Core.Extensions.AutofacManager;
using VOL.Entity.DomainModels;

namespace EKanban.Services
{
    public partial class ExecutionCardService : ServiceBase<ExecutionCard, IExecutionCardRepository>
    , IExecutionCardService, IDependency
    {
    public ExecutionCardService(IExecutionCardRepository repository)
    : base(repository)
    {
    Init(repository);
    }
    public static IExecutionCardService Instance
    {
      get { return AutofacContainerModule.GetService<IExecutionCardService>(); }
    }
 }
 }
```

> 自定义业务逻辑放在 `Partial/ExecutionCardService.cs`：

```csharp
// EKanban/Services/Partial/ExecutionCardService.cs
using EKanban.IServices;
using VOL.Entity.DomainModels;

namespace EKanban.Services
{
    public partial class ExecutionCardService : IExecutionCardService
    {
        public async Task<PageGrid<ExecutionCard>> GetPageListAsync(PageGrid pageGrid)
        {
            // 在这里写你的自定义业务逻辑
            return await FindPageListAsync(pageGrid);
        }
    }
}
```

---

### 2.4 控制器（VOL.WebApi.Controllers）

#### 规则
- 放在 `VOL.WebApi/Controllers/EKanban/`
- 继承 `BaseController`
- 添加 `[Route("api/[controller]/[action]")]` 特性

#### 控制器模板

```csharp
using Microsoft.AspNetCore.Mvc;
using VOL.Core.BaseProvider;
using VOL.Core.Utilities;
using EKanban.IServices;
using VOL.Entity.DomainModels;

namespace VOL.WebApi.Controllers.EKanban
{
    [Route("api/ekanban/[controller]/[action]")]
    public partial class ExecutionCardController : BaseController
    {
        private readonly IExecutionCardService _service;
        public ExecutionCardController(IExecutionCardService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<PageGrid<ExecutionCard>> GetPageList([FromBody] PageGrid pageGrid)
        {
            return await _service.GetPageListAsync(pageGrid);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.FindOneAsync(id);
            return Success(data);
        }

        // 自定义操作：手动重新触发 AI 执行
        [HttpPost]
        public async Task<IActionResult> TriggerReExecute(int id)
        {
            await _service.TriggerReExecuteAsync(id);
            return Success();
        }
    }
}
```

---

### 2.5 定时任务（Quartz）

Vol.Core 已经集成了 Quartz，通过界面管理定时任务：

#### 方式一：通过 UI 添加（推荐）
1. 系统登录后 → 系统管理 → 定时任务
2. 添加任务：
   - **任务名称**：任意名称
   - **分组**：E-Kanban
   - **请求方式**：GET/POST
   - **Cron 表达式**：比如 `0 */15 * * * ?` 表示每 15 分钟
   - **API Url**：`api/ekanban/AiTaskCheck/CheckInProgressTasks`
   - 保存后启动任务

#### 方式二：代码实现 IJob

```csharp
using Quartz;
using EKanban.IServices;
using Microsoft.Extensions.Logging;

namespace EKanban.Jobs
{
    public class AiTaskCheckJob : IJob
    {
        private readonly IAiTaskCheckService _checkService;
        private readonly ILogger<AiTaskCheckJob> _logger;

        public AiTaskCheckJob(
            IAiTaskCheckService checkService,
            ILogger<AiTaskCheckJob> logger)
        {
            _checkService = checkService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("开始检查 AI 任务超时...");
            await _checkService.CheckInProgressTasksAsync();
            _logger.LogInformation("AI 任务超时检查完成");
        }
    }
}
```

---

## 3. 前端开发规范

### 3.1 页面文件位置

```
vol.web/src/
├── views/
│   └── ekanban/
│       ├── ExecutionCard/
│       │   ├── index.vue           ← 主页面
│       │   └── options.js         ← 页面配置（表格列、表单字段等）
│       └── ...
├── components/
│   └── ekanban/
│       └── KanbanCard.vue         ← 自定义组件
├── api/
│   └── ekanban.ts                 ← API 封装
└── router/
    └── index.ts                  ← 需要添加路由
```

---

### 3.2 页面模板

Vol.Core Vue3 版本使用 `view-grid` 组件快速生成 CRUD 页面：

```vue
<!--
 *Author：jxx
 *Date：{Date}
 *Contact：283591387@qq.com
 *业务请在@/extension/ekanban/ExecutionCard.jsx或ExecutionCard.vue文件编写
 *新版本支持vue或【表.jsx]文件编写业务,文档见:https://v3.volcore.xyz/docs/view-grid、https://v3.volcore.xyz/docs/web
 -->
<template>
    <view-grid ref="grid"
               :columns="columns"
               :detail="detail"
               :details="details"
               :editFormFields="editFormFields"
               :editFormOptions="editFormOptions"
               :searchFormFields="searchFormFields"
               :searchFormOptions="searchFormOptions"
               :table="table"
               :extend="extend"
               :onInit="onInit"
               :onInited="onInited"
               :searchBefore="searchBefore"
               :searchAfter="searchAfter"
               :addBefore="addBefore"
               :updateBefore="updateBefore"
               :rowClick="rowClick"
               :modelOpenBefore="modelOpenBefore"
               :modelOpenAfter="modelOpenAfter">
        <!-- 自定义按钮 -->
        <template #gridHeader>
            <el-button type="primary" @click="syncFromAzure">同步 Azure Boards</el-button>
        </template>
    </view-grid>
</template>
<script setup lang="jsx">
    import extend from "@/extension/ekanban/ExecutionCard.jsx";
    import viewOptions from './ExecutionCard/options.js'
    import { ref, reactive, getCurrentInstance } from "vue";
    import { ElMessage } from "element-plus";
    import * as api from "@/api/ekanban";

    const grid = ref(null);
    const { proxy } = getCurrentInstance();
    const { table, editFormFields, editFormOptions, searchFormFields, searchFormOptions, columns, detail, details } = reactive(viewOptions());

    let gridRef;
    const onInit = async ($vm) => {
        gridRef = $vm;
    };
    const onInited = async () => {};
    const searchBefore = async (param) => {
        return true;
    };
    const searchAfter = async (rows, result) => {
        return true;
    };
    const addBefore = async (formData) => {
        return true;
    };
    const updateBefore = async (formData) => {
        return true;
    };
    const rowClick = ({ row, column, event }) => {};
    const modelOpenBefore = async (row) => {
        return true;
    };
    const modelOpenAfter = (row) => {};

    // 自定义方法：同步 Azure Boards
    const syncFromAzure = async () => {
        await api.syncFromAzureBoards();
        ElMessage.success("同步任务已触发");
        gridRef?.gridUpdate?.();
    };

    defineExpose({});
</script>
```

#### options.js 配置模板（表格列 + 表单字段）

```javascript
// 页面配置：定义表格列、搜索字段、表单字段
export default function () {
  return {
    table: {
      // 表格 API 请求地址
      url: "/api/ekanban/ExecutionCard/GetPageList",
    },
    columns: [
      {
        title: "ID",
        dataIndex: "id",
        width: 60,
      },
      {
        title: "标题",
        dataIndex: "title",
        ellipsis: true,
      },
      {
        title: "状态",
        dataIndex: "status",
        width: 100,
        dict: [
          { value: 0, label: "New" },
          { value: 1, label: "Ready" },
          { value: 2, label: "InProgress" },
          { value: 3, label: "Submitted" },
          { value: 4, label: "Completed" },
          { value: 5, label: "Failed" },
        ],
      },
      {
        title: "执行者类型",
        dataIndex: "executorType",
        width: 80,
      },
      {
        title: "最后更新",
        dataIndex: "lastUpdated",
        width: 160,
        type: "date",
      },
    ],
    searchFormFields: [
      {
        label: "标题",
        field: "title",
      },
      {
        label: "状态",
        field: "status",
        type: "select",
        dict: [
          { value: 0, label: "New" },
          { value: 1, label: "Ready" },
          // ...
        ],
      },
    ],
    searchFormOptions: {
      labelWidth: 80,
      baseCol: { span: 6 },
    },
    editFormFields: [
      {
        label: "标题",
        field: "title",
        rules: [{ required: true, message: "请输入标题" }],
      },
      {
        label: "描述",
        field: "description",
        type: "textarea",
      },
    ],
    editFormOptions: {
      labelWidth: 80,
      baseCol: { span: 24 },
    },
  };
};
```

---

### 3.3 API 封装

```typescript
// vol.web/src/api/ekanban.ts
import request from "@/utils/request";

// 获取看板数据（按状态分组）
export function getKanbanData() {
  return request({
    url: "/api/ekanban/ExecutionCard/GetKanbanData",
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

// 同步 Azure Boards
export function syncFromAzureBoards() {
  return request({
    url: "/api/ekanban/Sync/TriggerSync",
    method: "post",
  });
}

export default {
  getKanbanData,
  triggerReExecute,
  syncFromAzureBoards,
};
```

---

### 3.4 路由配置

在 `vol.web/src/router/index.ts` 中添加路由：

```typescript
{
  path: '/ekanban',
  redirect: '/ekanban/kanban',
  meta: {
    title: 'E-Kanban',
    icon: 'el-icon-s-data',
  },
  children: [
    {
      path: 'kanban',
      component: () => import('@/views/ekanban/ExecutionCard/index.vue'),
      meta: { title: 'Kanban 看板', icon: 'el-icon-s-grid' },
    },
  ],
},
```

---

## 4. 代码生成器使用

Vol.Core 提供了代码生成器，可以自动生成 CRUD 代码：

1. 先创建数据库表
2. 登录系统 → 系统管理 → 代码生成
3. 点击「创建代码」，选择数据库、表
4. 配置模块名称（`EKanban`）、所属项目
5. 点击生成代码
6. 下载生成的代码 zip 包，解压到对应目录

> 生成后的代码结构：
> - 实体 → `VOL.Entity/DomainModels/`
> - 仓储 → `EKanban/IRepositories/` + `EKanban/Repositories/`
> - 服务 → `EKanban/IServices/` + `EKanban/Services/`
> - 控制器 → `VOL.WebApi/Controllers/EKanban/`
> - 前端 → `vol.web/src/views/ekanban/` + `vol.web/src/api/`

**生成后你需要做**：
1. 检查生成的代码是否正确
2. 在 `Partial/` 文件夹添加你的自定义业务逻辑
3. 前端添加路由
4. 测试 API 是否正常工作

---

## 5. E-Kanban 模块开发清单

按照这个开发指南，我们需要创建以下文件：

### 后端

| 文件位置 | 说明 |
|---------|------|
| `VOL.Entity/DomainModels/BoardWorkItem.cs` | Azure Boards 工作项实体 |
| `VOL.Entity/DomainModels/ExecutionCard.cs` | Kanban 执行卡片实体 |
| `VOL.Entity/DomainModels/ExecutionTask.cs` | 执行定义任务实体 |
| `VOL.Entity/DomainModels/ExecutionRun.cs` | 执行事实记录实体 |
| `VOL.Entity/DomainModels/Spec.cs` | Spec 定义实体 |
| `VOL.Entity/DomainModels/SpecEvaluation.cs` | Spec 评估记录实体 |
| `EKanban/IRepositories/IExecutionCardRepository.cs` | |
| `EKanban/Repositories/ExecutionCardRepository.cs` | |
| `EKanban/IRepositories/IExecutionRunRepository.cs` | |
| `EKanban/Repositories/ExecutionRunRepository.cs` | |
| `EKanban/IRepositories/ISpecRepository.cs` | |
| `EKanban/Repositories/SpecRepository.cs` | |
| `EKanban/IRepositories/ISpecEvaluationRepository.cs` | |
| `EKanban/Repositories/SpecEvaluationRepository.cs` | |
| `EKanban/IServices/IAzureBoardsClient.cs` | |
| `EKanban/Services/AzureBoardsClient.cs` | |
| `EKanban/IServices/ISyncService.cs` | |
| `EKanban/Services/SyncService.cs` | |
| `EKanban/IServices/IExecutionCardService.cs` | |
| `EKanban/Services/ExecutionCardService.cs` | |
| `EKanban/IServices/IStateMachineService.cs` | |
| `EKanban/Services/StateMachineService.cs` | |
| `EKanban/IServices/ISubmitService.cs` | |
| `EKanban/Services/SubmitService.cs` | |
| `EKanban/IServices/IAiTaskCheckService.cs` | |
| `EKanban/Services/AiTaskCheckService.cs` | |
| `EKanban/Specs/ISpecGenerator.cs` | |
| `EKanban/Specs/SpecGenerator.cs` | |
| `EKanban/Specs/ISpecEvaluator.cs` | |
| `EKanban/Specs/SpecEvaluator.cs` | |
| `EKanban/AiExecution/ICopilotCliClient.cs` | |
| `EKanban/AiExecution/CopilotCliClient.cs` | |
| `EKanban/AiExecution/IAiExecutionService.cs` | |
| `EKanban/AiExecution/AiExecutionService.cs` | |
| `EKanban/Jobs/AiTaskCheckJob.cs` | 定时检查超时 AI 任务 |
| `VOL.WebApi/appsettings.json` | 添加配置 |
| `VOL.WebApi/Controllers/EKanban/ExecutionCardController.cs` | |

### 前端

| 文件位置 | 说明 |
|---------|------|
| `vol.web/src/views/ekanban/KanbanBoard/index.vue` | Kanban 看板主页面 |
| `vol.web/src/views/ekanban/KanbanBoard/options.js` | 页面配置 |
| `vol.web/src/components/ekanban/KanbanCard.vue` | Kanban 卡片组件 |
| `vol.web/src/api/ekanban.ts` | API 封装 |
| `vol.web/src/router/index.ts` | 添加路由 |

---

## 6. 总结

Vol.Core 的核心开发思想：
- **代码生成器优先**：先用代码生成器生成基础 CRUD 代码
- **partial 扩展**：生成的代码不修改，自定义业务放 partial 类
- **自动依赖注入**：遵循接口命名规范 `IXXX` / `XXX`，Autofac 自动注册
- **前端模板化**：使用 `view-grid` 快速生成 CRUD 页面

按照这个指南开发，可以保持和 Vol.Core 整体架构一致，利用框架提供的能力减少重复编码。
