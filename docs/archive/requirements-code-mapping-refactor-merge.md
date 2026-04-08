# 需求-代码映射：重构合并 E-Kanban 项目结构

---

## 需求摘要

将当前错误拆分为两个项目的 E-Kanban 重构合并为一个项目，使得结构与 VOL.MES 原框架保持一致。

---

## 映射表

| 功能点 | 当前状态 | 操作类型 | 需要修改/新增的文件 | 依赖模块 |
|--------|----------|----------|-------------------|----------|
| 1. 分析原框架结构 | 已完成 | 已完成 | - | - |
| 2. 对比现有结构差异 | 已完成 | 已完成 | - | - |
| 3. 创建目标目录结构 | 不存在 | 新增 | EKanban/{AiExecution,Data,DI,Jobs,Models,Specs} | - |
| 4. 移动所有代码文件 | 存在 E-Kanban.Backend | 移动 | 从 E-Kanban.Backend/* → EKanban/* | - |
| 5. 重命名文件夹（单数 → 复数） | IRepository/Repository | 修改 | IRepository → IRepositories, Repository → Repositories | - |
| 6. 更新所有文件命名空间 | E_Kanban.Backend.* → EKanban.* | 修改 | 所有 .cs 文件 in E-Kanban.Backend | - |
| 7. 更新 DomainModels → Models | EKanban/DomainModels 存在 | 合并 | DomainModels/* → Models/* | - |
| 8. 创建新的 EKanban.csproj | 存在旧 EKanban.csproj | 修改 | EKanban/EKanban.csproj | VOL.Core, VOL.Entity, VOL.Sys |
| 9. 删除旧 E-Kanban.Backend 项目 | 存在 | 删除 | entire src/modules/E-Kanban.Backend | - |
| 10. 删除 EKanban 中旧重复文件 | 存在旧文件 | 删除 | EKanban/{AiExecution,Jobs,Repositories,Services,Specs} 旧文件 | - |
| 11. 更新 VOL.WebApi 项目引用 | 当前引用 E-Kanban.Backend | 修改 | src/backend/VOL.WebApi/VOL.WebApi.csproj | EKanban |
| 12. 更新 E-Kanban.sln | 存在 E-Kanban.Backend 项目引用 | 修改 | E-Kanban.sln | - |
| 13. 更新 VOL.sln | 存在 E-Kanban.Backend 项目引用 | 修改 | VOL.sln | - |
| 14. 添加 DI 注册 | DI 存在 E-Kanban.Backend | 移动 | DI → EKanban/DI | - |
| 15. 编译验证 | 需执行 | 验证 | 执行 dotnet build E-Kanban.sln | 所有项目 |
| 16. 修复编译错误 | 可能存在 | 修改 | 根据错误信息修复 | 所有项目 |
| 17. 更新文档 | 需要更新 | 修改 | docs 中相关文档 | - |

---

## 修改范围总结

- **新增目录**: 7 个新目录在 EKanban/
- **移动文件**: 58 个 C# 文件从 E-Kanban.Backend → EKanban
- **修改文件**:
  - EKanban.csproj（合并更新）
  - VOL.WebApi.csproj（更新项目引用）
  - E-Kanban.sln（更新项目引用）
  - VOL.sln（更新项目引用）
  - 所有 58 个 C# 文件（更新命名空间）
- **删除**:
  - 整个 `src/modules/E-Kanban.Backend` 目录
  - EKanban/ 中重复的旧文件

---

## 执行顺序（按依赖）

1. 创建目标目录结构
2. 移动文件并更新命名空间和文件夹命名
3. 合并 DomainModels → Models
4. 更新 EKanban.csproj
5. 删除旧项目和旧文件
6. 更新其他项目中的项目引用（VOL.WebApi）
7. 更新解决方案文件
8. 编译验证
9. 修复错误
10. 更新文档

---

映射完成。
