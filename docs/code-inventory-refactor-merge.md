# 代码资产盘点：重构合并 E-Kanban 项目结构

---

## 一、原框架标准结构（VOL.MES 为例）

### 1.1 项目结构

```
src/modules/VOL.MES/
├── VOL.MES.csproj              → 项目文件与文件夹同名
├── IRepositories/              → 仓储接口
│   └── mes/                    → 按业务分组
│       └── IXXXRepository.cs
├── Repositories/               → 仓储实现
│   └── mes/
│       └── XXXRepository.cs
├── IServices/                  → 服务接口
│   └── mes/
│       └── IXXXService.cs
├── Services/                   → 服务实现
│   └── mes/
│       ├── XXXService.cs
│       └── Partial/            → 部分方法放这里（分部类）
│           └── PartialXXXService.cs
└── ... 其他按业务分组的目录
```

### 1.2 项目文件特点

- **SDK**: `Microsoft.NET.Sdk`（不是 Web）
- **TargetFramework**: `net8.0`
- **项目引用**: 只依赖 core 层（VOL.Core、VOL.Entity、VOL.Sys）
- **命名规范**: 项目名 = 文件夹名，全部一致（`VOL.MES` 文件夹 → `VOL.MES.csproj`）

---

## 二、当前 E-Kanban 现状

### 2.1 两个项目拆分

| 项目 | 文件数量 | 位置 | SDK 类型 | 问题 |
|------|----------|------|----------|------|
| `EKanban` | 48 | `src/modules/EKanban/` | `Microsoft.NET.Sdk` | 旧实现，大部分代码被 E-Kanban.Backend 取代，结构不完整 |
| `E-Kanban.Backend` | 58 | `src/modules/E-Kanban.Backend/` | `Microsoft.NET.Sdk.Web` | 完整的新实现，但位置和命名不对，不应该是独立项目 |

### 2.2 E-Kanban.Backend 当前结构（完整实现）

```
src/modules/E-Kanban.Backend/
├── E-Kanban.Backend.csproj
├── AiExecution/          → AI 执行相关（Copilot CLI 集成）
├── Data/                → 数据库配置
├── DI/                  → 依赖注入注册
├── IRepository/         → 仓储接口
├── IServices/           → 服务接口
├── Jobs/                → Quartz 定时任务
├── Models/              → 领域模型（实体）
├── Repository/          → 仓储实现
├── Services/            → 服务实现
├── Specs/               → Spec 生成和评估
└── appsettings.json
```

### 2.3 现有 EKanban 结构（旧实现）

```
src/modules/EKanban/
├── EKanban.csproj
├── AiExecution/
├── DomainModels/
├── IRepositories/
├── IServices/
├── Jobs/
├── Repositories/
├── Services/
└── Specs/
```

### 2.4 可复用资产

| 资产 | 来源 | 是否复用 |
|------|------|----------|
| 所有完整业务代码 | E-Kanban.Backend | ✅ 全部复用，合并到 EKanban |
| EKanban 项目文件 | EKanban | ✅ 复用项目文件结构（SDK 类型正确） |
| DomainModels | EKanban | ⚠️ 需要检查，合并到 Models |
| DI/依赖注入注册 | E-Kanban.Backend | ✅ 复用，合并过来 |

---

## 三、现有项目引用关系

```
VOL.WebApi → 引用 E-Kanban.Backend → 引用 EKanban → 引用 core
需要改成：
VOL.WebApi → 引用 EKanban → 引用 core
```

`E-Kanban.Backend` 本身引用了 `EKanban`，说明这本来就应该是一个项目。

---

## 四、代码风格和命名规范

### 4.1 文件夹命名

- 原框架：PascalCase → `IRepositories`, `Repositories`, `IServices`, `Services`
- 当前 E-Kanban.Backend：IRepository（单数）, Repository（单数）→ 需要改复数对齐原框架

### 4.2 命名空间

原框架：`VOL.MES.IRepositories.mes` → `{项目名}.{目录}.{业务组}`

所以合并后应该是：`EKanban.IRepositories.{分组}`

---

## 五、盘点总结

| 项目 | 现状 | 处理方式 |
|------|------|----------|
| `E-Kanban.Backend` 完整代码 | 功能完整但位置不对 | ✅ 全部移动到 `EKanban` 对应目录 |
| `E-Kanban.Backend` 项目 | SDK 是 Web 不对，位置不对 | 🗑️ 删除整个项目目录 |
| `EKanban` 旧代码 | 大部分被取代，不完整 | 🗑️ 删除旧的重复文件，保留目录结构 |
| `EKanban.csproj` | 结构正确（Microsoft.NET.Sdk） | ✅ 保留，更新内容 |
| 命名空间 `E_Kanban.Backend.*` | 需要统一 | 🔧 全部改为 `EKanban.*` |
| 文件夹命名 IRepository/Repository | 单数，需要改复数 | 🔧 统一为 IRepositories/Repositories |
| 解决方案 | VOL.sln 和 E-Kanban.sln | 🔧 更新项目路径 |

---

## 六、依赖分析

需要更新项目引用的项目：

1. **`src/backend/VOL.WebApi/VOL.WebApi.csproj`** → 当前引用 `E-Kanban.Backend`，需要改成引用 `EKanban`
2. **`E-Kanban.sln`** → 需要更新路径，移除 `E-Kanban.Backend`
3. **`VOL.sln`** → 已经添加了 `E-Kanban.Backend`，需要改成 `EKanban`

---

盘点完成。
