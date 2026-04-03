# E-Kanban 开发引导文件

你好！欢迎开始 E-Kanban 项目开发。本文档是**一步一步的引导**，你跟着做就行了。

---

## 📚 先了解整体情况

项目文档都在 `docs/` 目录下：

| 文件 | 说明 | 用途 |
|------|------|------|
| [requirements-structured.md](./requirements-structured.md) | 结构化需求文档 | 了解我们要做什么 |
| [code-inventory.md](./code-inventory.md) | 代码资产盘点 | 了解现有代码结构 |
| [requirements-code-mapping.md](./requirements-code-mapping.md) | 需求代码映射表 | 知道每个功能改哪里新增哪里 |
| [requirement-functional-checklist.md](./requirement-functional-checklist.md) | 功能清单 | 详细的功能点清单 |
| [volcore-development-guide.md](./volcore-development-guide.md) | Vol.Core 开发规范 | 遵循这个规范写代码 |
| [implementation-plan.md](./implementation-plan.md) | 执行计划 | 按这个顺序开发 |
| **DEVELOPMENT-GUIDE.md**（你在这里） | 开发引导 | 一步一步跟着走 |

---

## ✅ 开发前准备

1. **确认你在项目根目录**：
   ```bash
   cd /path/to/E-Kanban
   ```

2. **确认文档都已就绪**：
   ```bash
   ls -la docs/
   ```
   应该能看到上面列出的所有文件。

3. **通读一遍执行计划**：`implementation-plan.md`，了解整体批次和任务。

---

## 🚀 开始开发（跟着执行计划走）

### 第一步：执行第一批次 - 基础设施层

打开 `implementation-plan.md`，找到**第一批次**，按顺序做：

> **第一批次任务**：
> 1.1 → 1.2 → 1.3

**每个任务的执行步骤**：

```
1. 看需求：在 requirements-structured.md 里找到这个实体/功能的需求
2. 看规范：对照 volcore-development-guide.md 看开发规范
3. 确定位置：看 requirements-code-mapping.md 知道该放在哪里
4. 用 Copilot 生成代码：用提示词模板让 Copilot 帮你写
5. 自验：对照检查清单检查，Copilot 静态代码检查
6. 编译：确保编译通过
7. 在 implementation-plan.md 里把这个任务状态改成 ✅ 完成
8. 进入下一个任务
```

**GitHub Copilot 编程模式提示词模板**（直接用）：

```bash
copilot -p "
我正在开发 E-Kanban 项目，基于 Vol.Core 框架，SqlSugar ORM。

开发规范：
$(cat docs/volcore-development-guide.md)

需求：
【这里粘贴需求】

现有代码示例（参考风格）：
$(cat VOL.Entity/SystemModels/User.cs)

请帮我生成代码，放在正确的位置，遵循现有代码风格。
" --allow-all-tools
```

---

### 📋 每个任务完成后检查清单

- [ ] 代码符合 `volcore-development-guide.md` 中的规范
- [ ] 编译能通过
- [ ] 命名风格和项目保持一致
- [ ] 在 `implementation-plan.md` 中更新了任务状态

---

### 批次完成后做什么

一个批次的所有任务都完成后：

1. 整体编译一次项目
2. 确保没有编译错误
3. 确认所有任务状态都已标记为完成
4. 提交代码到 Git
5. 休息一下，然后开始下一个批次

**提交示例**：
```bash
git add .
git commit -m "feat: complete first batch - infrastructure layer"
git push
```

---

## ❓ 遇到问题怎么办

1. **需求不清晰** → 回到 `requirements-structured.md` 重新看，还是不清晰就澄清需求
2. **不知道放哪里** → 看 `requirements-code-mapping.md`，里面写了每个功能该放哪里
3. **代码不编译** → 看错误信息，让 Copilot 帮你检查：
   ```bash
   copilot -p "
   我遇到了编译错误：
   【粘贴错误信息】

   这是相关代码：
   【粘贴代码】

   请帮我分析原因并修复。
   " --allow-all-tools
   ```
4. **Copilot 生成的代码不对** → 提供更多上下文，重新问，明确告诉它你想要什么
5. **记录经验教训** → 把问题和解决方法记到 `lessons-learned.md` 里，方便以后自己或别人参考

---

## 🧪 开发完所有批次后

所有开发任务都完成后，进行整体测试：

1. 按照 `implementation-plan.md` 中的**整体测试计划**逐项测试
2. 修复发现的问题
3. 测试全部通过后，进行知识沉淀

---

## 💾 最后：知识沉淀

所有开发完成并测试通过后：

1. [ ] 更新 `code-inventory.md` - 添加新增模块信息
2. [ ] 创建 `lessons-learned.md` - 记录遇到的问题和解决方法（用这个模板）：
   ```markdown
   # 经验教训记录

   ## 问题：【问题描述】

   ## 原因：【原因分析】

   ## 解决方法：【怎么解决的】

   ## 预防措施：【以后怎么避免】

   ## 日期：YYYY-MM-DD
   ```
3. [ ] 更新项目根目录的 README - 说明如何配置、构建、运行
4. [ ] 提交所有代码和文档
5. [ ] 完成！🎉

---

## ⏱️ 进度跟踪

随时看 `implementation-plan.md` 了解当前进度，里面有表格记录每个任务的状态。

---

## 🎯 完成后最终检查清单

- [ ] 所有 23 个开发任务都已标记完成
- [ ] 所有测试项都通过
- [ ] 文档都已更新
- [ ] 项目能正常构建
- [ ] 核心功能都可用
- [ ] 代码和文档都已提交到 Git

---

**祝你开发顺利！** 🚀

如果严格按照这个引导走，不会乱，一步步就能把项目做出来。

---

创建日期：2026-04-01
