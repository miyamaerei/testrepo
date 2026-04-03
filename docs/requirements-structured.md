# 结构化需求文档：E-Kanban 执行型 Kanban 协调系统

## 1. 背景和目标

### 背景
- 现有 Azure Boards 用于业务需求管理，但缺乏可执行、可验证、可追溯的执行闭环
- 需要支持人、AI、系统三种角色统一执行模型
- 需要通过 Spec 校验避免"假完成"
- **AI 执行不是一次性完成，通常需要分批迭代执行**
- 需要防止 AI 偷懒，要有检查机制和重新唤起能力
- 开发环境受公司限制，只能使用 GitHub Copilot CLI 开发

### 目标
构建一个**以可视化 Kanban 为核心的人机协同执行平台**：
- 承接 Azure Boards 中的工作项
- 将工作项转化为可执行、可验证、可回溯的执行过程
- Azure Boards 作为业务事实源
- Kanban 作为执行中枢与唯一操作入口
- 统一 Submit API 作为执行触发点
- 支持人 / AI / 系统执行一致化
- 使用 Spec 判定任务是否真正完成
- **支持 AI 分批迭代执行，提供超时检查和重新执行机制，防止 AI 偷懒**

### 价值
- 打通从业务需求到执行落地的闭环
- 让人机协同执行过程可视化、可追溯
- 通过 Spec 校验保证任务真正完成，避免假完成
- **防止 AI 声称在执行但实际不执行，保证进度推进**
- 统一不同执行者的执行模型

---

## 2. 功能范围

### 2.1 做什么
| 模块 | 功能点 | 说明 |
|-----|--------|------|
| **Azure Boards 集成** | 定时拉取 | 定时从 Azure Boards 拉取工作项，同步标题、描述、状态 |
| **Azure Boards 集成** | 状态回写 | 执行完成后回写评论和状态到 Azure Boards |
| **Kanban 核心** | ExecutionCard 管理 | Kanban 卡片生命周期管理 |
| **Kanban 核心** | 状态机推进 | New → Ready → InProgress → Submitted → Completed / Failed |
| **Kanban 核心** | 可视化展示 | Vue 前端展示 Kanban 看板，支持按状态列分组 |
| **Submit API** | 统一提交入口 | POST /api/execution-cards/{id}/submit，支持人/AI/系统提交 |
| **Submit API** | 执行记录 | 每次提交创建 ExecutionRun，记录执行者、时间、证据 |
| **Spec Engine** | Spec 生成 | AI（GitHub Copilot）自动生成 Spec 定义完成标准 |
| **Spec Engine** | Spec 评估 | 根据提交的证据评估是否符合 Spec，判定通过/失败 |
| **Spec Engine** | 状态驱动 | 根据评估结果驱动 Kanban 卡片状态流转 |
| **AI 执行调度** | AI 任务调度 | Quartz 定时调度 AI 执行，支持分批迭代 |
| **AI 执行调度** | 超时检查 | 检查 InProgress 状态的 AI 任务是否超时，超时触发警告/重新执行 |
| **AI 执行调度** | 重新执行 | 支持手动/自动重新唤起 AI 执行，应对 AI 偷懒 |
| **AI 执行调度** | 执行历史 | 记录每次 AI 执行结果，可追溯多次迭代 |
| **系统执行** | Quartz 后台任务 | 支持系统自动执行后台任务（框架已有） |
| **用户认证** | 用户登录 | 支持用户登录认证 |
| **执行追溯** | 执行历史查看 | 查看卡片的所有执行提交和评估结果 |
| **防偷懒机制** | 状态监控 | 监控 InProgress 状态的 AI 任务，长时间不提交自动标记 |
| **防偷懒机制** | 人工干预 | 支持手动触发重新执行，管理员可以推进卡住的任务 |

### 2.2 不做什么
- 不替代 Azure Boards 的业务需求管理功能
- 不做复杂的高级看板功能（如泳道、多个项目看板），核心是简单可视化
- Spec Engine 只做结果判定，不参与具体任务执行
- AI 执行本身由 GitHub Copilot CLI 完成，本系统只做任务协调和结果接收
- 不替代 Git 操作，AI 在本地 working directory 执行代码修改

---

## 3. AI 分批执行与防偷懒机制设计

### 3.1 问题背景

AI 执行大任务通常不能一次性完成，需要多轮迭代：
1. AI 执行一部分 → 提交结果 → Spec 评估 → 未通过 → 下一轮继续
2. AI 可能卡住，长时间停留在 InProgress 不提交 → 需要机制检测并重新唤起
3. AI 可能声称"正在进行"但实际不推进 → 需要超时检测和人工干预

### 3.2 核心机制

| 机制 | 说明 |
|-----|------|
| **执行轮次记录** | 每次 AI 执行都是一个独立的 `ExecutionRun`，记录完整输入输出 |
| **超时检测** | 定时扫描 InProgress 状态的 AI 任务，超过配置阈值（如 2 小时）没提交 → 自动标记为需要重新执行 |
| **自动重试** | 超时任务可以自动重新调度执行，或通知管理员人工处理 |
| **手动重新执行** | 支持在看板上手动触发 AI 任务重新执行 |
| **迭代进度可见** | 在看板上显示 AI 任务已经执行了多少轮，每次结果是什么 |
| **失败次数限制** | 连续失败 N 次后停止自动重试，需要人工干预 |

### 3.3 AI 执行工作流程（分批迭代版本）

```
1. 从 Azure Boards 同步得到新任务
   ↓
2. 创建 ExecutionCard，ExecutorType = AI，状态 = New
   ↓
3. 生成 Spec → 状态转为 Ready
   ↓
4. 用户/系统触发 AI 执行 → 状态 = InProgress，记录开始时间
   ↓
5. Quartz 调度 AI 执行任务
   ↓
6. AiExecutionService 调用 copilot CLI → 获取输出 → 创建 ExecutionRun → 自动 Submit
   ↓
7. SpecEvaluator 评估 → Passed/Failed
   ↓
8. 如果 Passed → 状态 = Completed → 回写 Azure Boards → 结束
   ↓
9. 如果 Failed → 状态 = Ready（等待下一轮）→ 记录失败原因
   ↓
10. 防偷懒检查：定时扫描 InProgress 任务
    - 如果开始时间超过 X 小时且没有 ExecutionRun → 标记为超时
    - 如果失败次数 < N → 自动调度重新执行
    - 如果失败次数 >= N → 转为需要人工干预，通知管理员
```

---

## 4. 技术约束

| 约束项 | 具体要求 |
|--------|----------|
| **后端技术栈** | .NET + C# |
| **前端技术栈** | Vue.js |
| **数据库** | SQL Server |
| **后台任务** | Quartz.NET（Vue.NetCore 框架已有） |
| **部署方式** | 本地部署，发布在 IIS 上 |
| **认证** | 用户登录认证 |
| **Azure Boards 认证** | PAT（个人访问令牌）认证 |
| **开发工具限制** | 只能使用 GitHub Copilot CLI 开发 |
| **同步机制** | 定时拉取模式 |

---

## 5. 验收标准

- [ ] Azure Boards 工作项能正常定时拉取同步
- [ ] Kanban 看板能正确可视化展示所有卡片，按状态分组
- [ ] 支持人/AI/系统通过统一 Submit API 提交执行结果
- [ ] Spec 能自动生成，能正确评估执行结果
- [ ] 只有 Spec 评估通过后，卡片才能进入 Completed 状态
- [ ] AI 任务支持分批迭代执行，每轮执行都有记录
- [ ] 超时检测机制能发现长时间停留在 InProgress 的 AI 任务
- [ ] 支持手动/自动重新执行卡住的 AI 任务
- [ ] 执行结果和状态能正确回写到 Azure Boards
- [ ] 所有执行过程有记录，可追溯
- [ ] 能正常部署到 IIS 运行

---

## 6. 优先级评估

| 模块 | 业务价值 | 实现成本 | 优先级 |
|-----|---------|---------|--------|
| Azure Boards 集成 | 高 | 中 | P0 |
| Kanban 核心领域模型 | 高 | 低 | P0 |
| Submit API | 高 | 低 | P0 |
| Spec Engine | 高 | 中 | P0 |
| GitHub Copilot CLI 集成 | 高 | 中 | P0 |
| AI 超时检测和重试机制 | 高 | 低 | P0 |
| Vue Kanban UI | 中 | 中 | P0 |
| 用户认证 | 中 | 低 | P1 |
| Quartz 系统执行 | 中 | 低 | P1（框架已有）|
| 执行历史查看 | 中 | 低 | P1 |

---

## 7. 风险和应对

| 风险 | 影响 | 概率 | 应对措施 |
|-----|------|------|---------|
| .NET + Vue 前后端分离，GitHub Copilot 理解项目结构有难度 | 中 | 中 | 严格按照六阶段方法论，先做代码资产盘点和需求映射，增量开发 |
| Spec 自动生成质量不稳定 | 中 | 中 | MVP 阶段先支持简单 Spec，后续迭代优化 |
| SQL Server 连接和部署到 IIS 有环境问题 | 低 | 中 | 文档化连接字符串配置和部署步骤 |
| 定时拉取任务可能重复执行 | 低 | 低 | 使用 Quartz 可靠调度，做好幂等处理 |
| AI 持续失败无法完成 | 高 | 中 | 设置失败次数上限，超过后停止自动重试，通知人工干预 |

---

## 8. 总体执行闭环

```
Azure Boards
    ↓ 定时拉取同步
执行型 Kanban（可视化）
    ↓ 触发 AI 执行 → InProgress（记录开始时间）
Quartz 调度
    ↓
AI 执行（GitHub Copilot CLI）
    ↓ 提交证据（Submit API）
Spec Engine 校验
    ↓ Pass?
    Y → Completed → 回写 Azure Boards
    N → Ready（下一轮）
防偷懒检查：定时扫描 InProgress
    ↓ 超时？
    Y → 超过失败次数?
        Y → 需人工干预 → 通知管理员
        N → 自动重新调度执行
    N → 继续等待
```

---

## 9. MVP 范围

**MVP 包含**：
- 完整领域模型（ExecutionCard, ExecutionTask, ExecutionRun, Spec 等）
- Azure Boards 定时拉取同步
- 简单 Vue Kanban 可视化
- 统一 Submit API
- Spec 生成与评估
- GitHub Copilot CLI 集成调用
- **AI 超时检测和自动重试**
- **支持手动重新执行**
- 状态回写到 Azure Boards

**后续迭代**：
- 更丰富的 UI 交互（拖拽改状态等）
- 更复杂的 Spec 语法
- 更多统计报表
- 权限控制细化
- 邮件/消息通知管理员

---

## 10. 澄清记录

| 问题 | 澄清结果 |
|-----|---------|
| 技术选型 | 后端 .NET，前端 Vue，数据库 SQL Server，框架使用 Vue.NetCore (Vol.Core) SqlSugar 版本 |
| 部署方式 | 本地部署，发布在 IIS 上 |
| 认证授权 | 需要用户登录，Azure Boards 使用 PAT 认证 |
| 同步机制 | 定时拉取模式，同步标题、描述、状态 |
| Spec 定义方式 | AI（GitHub Copilot）自动生成 Spec 来检查 |
| UI 需求 | 简单看板即可，核心是可视化 |
| 环境约束 | 受公司限制，AI 部分只能用 GitHub Copilot CLI |
| AI 执行模式 | AI 执行通常需要分批迭代，需要防偷懒机制：超时检测、重新执行、人工干预 |

---

## 11. 附录

**文档版本**：v1.1
**创建日期**：2026-04-01
**最后更新**：2026-04-01
**更新内容**：增加 AI 分批执行和防偷懒机制
**创建人**：Claude
**审核人**：
