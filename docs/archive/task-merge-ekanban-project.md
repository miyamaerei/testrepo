# 重构任务：合并 E-Kanban 项目结构（交接文档）

---

## 📋 任务概述

### 原始需求

> 我看到后端是有代码了，但是e-kanban是被拆成两个库，所以有点奇怪，你能研究一下最初的框架，先确定一下项目结构，然后再重构一下项目，使得项目的结构和原来的系统框架结构是一致的。

### 任务目标

将当前错误拆分的两个项目 `EKanban` + `E-Kanban.Backend` 合并为**一个项目 `EKanban`**，使得整体结构与原框架 `VOL.MES` 保持一致。

---

## 🔍 原框架结构标准（VOL.MES 参考）

```
src/modules/VOL.MES/
├── VOL.MES.csproj              # 项目名 = 目录名（一个项目）
├── IRepositories/              # 仓储接口（复数）
├── Repositories/               # 仓储实现（复数）
├── IServices/                  # 服务接口（复数）
├── Services/                   # 服务实现（复数）
│   └── mes/                    # 按业务分组（可选）
└── ... 其他业务目录
```

**框架规范总结：**
| 规范项 | 要求 |
|--------|------|
| 项目数量 | 一个业务模块 = 一个 csproj 项目 |
| 项目命名 | 项目文件名 = 目录名 |
| 文件夹命名 | 接口：`IRepositories`（复数），实现：`Repositories`（复数） |
| 基类继承 | 仓储：`RepositoryBase<T>`，接口：`IRepository<T>` |
| 命名空间 | `{项目名}.{目录名}` → `EKanban.Repositories` |

---

## ✅ 已完成工作

| 工作项 | 状态 | 说明 |
|--------|------|------|
| 1. 需求结构化分析 | ✅ 完成 | `docs/requirements-structured-refactor-merge.md` |
| 2. 代码资产盘点 | ✅ 完成 | `docs/code-inventory-refactor-merge.md` |
| 3. 需求-代码映射 | ✅ 完成 | `docs/requirements-code-mapping-refactor-merge.md` |
| 4. 创建目标目录结构 | ✅ 完成 | 在 `EKanban` 下创建了所有需要的目录 |
| 5. 移动所有代码文件 | ✅ 完成 | 从 `E-Kanban.Backend/*` → `EKanban/*` |
| 6. 批量替换命名空间 | ✅ 完成 | `E_Kanban.Backend.*` → `EKanban.*` |
| 7. 修正文件夹命名 | ✅ 完成 | `IRepository` → `IRepositories`，`Repository` → `Repositories` |
| 8. 删除旧 E-Kanban.Backend | ✅ 完成 | 删除整个 `src/modules/E-Kanban.Backend` 目录 |
| 9. 更新 EKanban.csproj | ✅ 完成 | 合并所有包引用和项目引用 |
| 10. 更新 VOL.WebApi | ✅ 完成 | 移除 `E-Kanban.Backend` 引用，保留 `EKanban` |
| 11. 更新解决方案 | ✅ 完成 | `VOL.sln` / `E-Kanban.sln` 都已更新 |
| 12. 批量修复基类/接口名 | ✅ 完成 | `BaseRepositories` → `RepositoryBase`，`IBaseRepositories` → `IRepository` |

---

## 🚧 待完成工作

### 剩余任务：修复 Repository 构造函数名称

共有 **10 个** Repository 文件，其中 1 个已修复，还剩 **9 个** 需要修改。

**修改规则：**
- 文件名已经是 `XXXRepository.cs`（单数）✅
- 类名已经是 `XXXRepository`（单数）✅
- **只有构造函数名**还是 `XXXRepositories`（复数），需要改为 `XXXRepository`（单数）

### 待修复文件列表

| 文件路径 | 当前构造函数名 | 需要改为 | 状态 |
|----------|----------------|----------|------|
| `src/modules/EKanban/Repositories/BoardWorkItemRepository.cs` | `BoardWorkItemRepositories` | `BoardWorkItemRepository` | ✅ 已修复 |
| `src/modules/EKanban/Repositories/ExecutionCardRepository.cs` | `ExecutionCardRepositories` | `ExecutionCardRepository` | ⚠️ 待修复 |
| `src/modules/EKanban/Repositories/ExecutionRunRepository.cs` | `ExecutionRunRepositories` | `ExecutionRunRepository` | ⚠️ 待修复 |
| `src/modules/EKanban/Repositories/ExecutionTaskRepository.cs` | `ExecutionTaskRepositories` | `ExecutionTaskRepository` | ✅ 已修复 |
| `src/modules/EKanban/Repositories/ProjectRepositoryRepository.cs` | `ProjectRepositoriesRepositories` | `ProjectRepositoryRepository` | ⚠️ 待修复 |
| `src/modules/EKanban/Repositories/SpecEvaluationRepository.cs` | `SpecEvaluationRepositories` | `SpecEvaluationRepository` | ⚠️ 待修复 |
| `src/modules/EKanban/Repositories/SpecRepository.cs` | `SpecRepositories` | `SpecRepository` | ⚠️ 待修复 |
| `src/modules/EKanban/Repositories/TaskFileChangeRepository.cs` | `TaskFileChangeRepositories` | `TaskFileChangeRepository` | ⚠️ 待修复 |
| `src/modules/EKanban/Repositories/TaskPhaseProgressRepository.cs` | `TaskPhaseProgressRepositories` | `TaskPhaseProgressRepository` | ⚠️ 待修复 |
| `src/modules/EKanban/Repositories/ProjectRepositoryRepository.cs` | `ProjectRepositoriesRepositories` | `ProjectRepositoryRepository` | ⚠️ 待修复 |

### 修复示例（已修复的第一个文件）

**修改前：**
```csharp
public class BoardWorkItemRepository : RepositoryBase<BoardWorkItem>, IBoardWorkItemRepository
{
    public BoardWorkItemRepositories(SqlSugarClient db) : base(db)
    {
    }
```

**修改后：**
```csharp
public class BoardWorkItemRepository : RepositoryBase<BoardWorkItem>, IBoardWorkItemRepository
{
    public BoardWorkItemRepository(SqlSugarClient db) : base(db)
    {
    }
```

---

## 🧪 完成后验证

所有文件修复完成后，执行编译验证：

```bash
dotnet build E-Kanban.sln
```

**预期结果：编译零错误**

如果还有其他错误，根据错误信息修复即可。

---

## 📝 完成后需要做

1. **提交代码**：
```bash
git add .
git commit -m "refactor: 合并 E-Kanban 项目结构，符合原框架规范"
git push origin master
```

2. **更新 README.md**：说明新的项目结构

3. **检查编译警告**：处理掉所有警告

---

## 📊 项目最终结构（预期）

```
src/modules/EKanban/
├── EKanban.csproj
├── AiExecution/           # AI 执行相关（Copilot CLI 集成）
├── Data/                 # 数据库配置
├── DI/                   # 依赖注入注册
├── IRepositories/        # 仓储接口
├── IServices/            # 服务接口
├── Jobs/                 # Quartz 定时任务
├── Models/               # 领域模型
├── Repositories/         # 仓储实现
├── Services/             # 服务实现
└── Specs/                # Spec 生成和评估
```

**完全符合原框架 VOL.MES 的结构规范 ✅**

---

**文档创建时间**: 2026-04-04  
**创建人**: Claude  
**当前状态**: 准备交接，待修复 9 个构造函数
