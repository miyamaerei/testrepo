# 需求结构化：重构合并 E-Kanban 项目结构

---

## 原始需求

> 现在请你看一下，我看到后端是有代码了，但是e-kanban是被拆成两个库，所以有点奇怪，你能研究一下最初的框架，先确定一下项目结构，然后再重构一下项目，使得项目的结构和原来的系统框架结构是一致的。

---

## 一、核心目标

解决当前 E-Kanban 被错误拆分成两个项目的问题，重构合并为一个项目，使得整体项目结构与 VOL.MES 保持一致，符合原框架设计。

---

## 二、现状分析

### 当前目录结构

```
src/modules/
├── EKanban/              → 旧的 E-Kanban 模块（48 个 C# 文件）
│   ├── EKanban.csproj
│   ├── AiExecution/
│   ├── DomainModels/
│   ├── IRepositories/
│   ├── IServices/
│   ├── Jobs/
│   ├── Repositories/
│   ├── Services/
│   └── Specs/
├── E-Kanban.Backend/     → 新的 E-Kanban 后端模块（58 个 C# 文件）
│   ├── E-Kanban.Backend.csproj
│   ├── AiExecution/
│   ├── Data/
│   ├── DI/
│   ├── IRepository/
│   ├── IServices/
│   ├── Jobs/
│   ├── Models/
│   ├── Repository/
│   ├── Services/
│   └── Specs/
└── VOL.MES/              → 原框架标准结构（参考）
    ├── VOL.MES.csproj
    ├── IRepositories/
    ├── IServices/
    ├── Repositories/
    └── Services/
```

### 当前问题

1. **不合理拆分**：同一个业务被拆成两个项目，架构混乱
2. **重复代码**：两个项目都有 AiExecution、Jobs、Specs 等重复目录结构
3. **命名不一致**：一个用 `EKanban`（驼峰），一个用 `E-Kanban.Backend`（kebab+后缀）
4. **不符合原框架规范**：原框架每个业务模块就是一个项目，不拆分成"模块+后端"两层

---

## 三、预期目标结构

参考 `VOL.MES` 的结构，合并后应该是：

```
src/modules/
├── EKanban/              → 统一的一个 E-Kanban 模块
│   ├── EKanban.csproj     → 项目文件（保持和文件夹同名）
│   ├── DomainModels/      → 领域模型（原来在这里）
│   ├── IRepositories/     → 仓储接口
│   ├── Repositories/      → 仓储实现
│   ├── IServices/         → 服务接口
│   ├── Services/         → 服务实现
│   ├── Models/           → 新增：E-Kanban 特色领域模型
│   ├── AiExecution/       → AI 执行相关（合并过来）
│   ├── Jobs/             → 定时任务（合并过来）
│   ├── Specs/            → Spec 生成和评估（合并过来）
│   └── DI/               → 依赖注入注册（合并过来）
└── VOL.MES/              → 保持不变
    ...
```

---

## 四、约束条件

1. **必须保持**：与 `VOL.MES` 相同的目录结构和命名规范
2. **必须保留**：E-Kanban.Backend 中的完整功能代码（这是正确实现）
3. **必须删除**：EKanban 中重复的旧代码（已经被新实现取代）
4. **必须更新**：
   - `VOL.sln` 和 `E-Kanban.sln` 中的项目引用路径
   - 所有项目文件中的 `ProjectReference` 路径
   - 所有命名空间需要调整一致
5. **编译必须通过**：重构后 `dotnet build` 零错误

---

## 五、功能点拆解

| 序号 | 功能点 | 说明 |
|------|--------|------|
| 1 | 分析原框架结构 | 研究 VOL.MES 的结构和规范，确定标准 |
| 2 | 对比现有结构差异 | 确定哪些需要移动，哪些需要删除 |
| 3 | 合并文件到正确位置 | 将 E-Kanban.Backend 中完整代码合并到 EKanban 模块正确位置 |
| 4 | 删除旧的 E-Kanban.Backend 项目 | 合并完成后删除整个目录 |
| 5 | 更新命名空间 | 将所有 `E_Kanban.Backend.*` 命名空间改为 `EKanban.*` |
| 6 | 更新项目文件 | 创建新的 EKanban.csproj，合并所有项目引用和包引用 |
| 7 | 更新解决方案文件 | 更新 VOL.sln 和 E-Kanban.sln |
| 8 | 更新所有 ProjectReference 路径 | 其他项目引用 E-Kanban 的路径需要修正 |
| 9 | 编译验证 | 执行 dotnet build，修复所有错误 |
| 10 | 更新文档 | 更新 docs 中的相关文档说明新结构 |

---

## 六、模糊点澄清

✓ 已经澄清：
- 用户明确要求结构和原来的系统框架结构一致 → 参考 VOL.MES
- 用户要求做成任务让后台跑 → 按 S/E 流程完成结构化

---

## 七、确认

以上是结构化后的需求，请确认无误后进入下一阶段。
