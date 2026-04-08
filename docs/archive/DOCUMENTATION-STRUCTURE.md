# E-Kanban 文档结构说明

## 文档目录概览

E-Kanban 项目采用结构化的文档管理方式，将文档分为以下几个部分：

```
e:\git\ekanban\testrepo/
├── docs/                          # 项目文档目录
│   ├── templates/                 # 文档模板
│   ├── DEVELOPMENT-GUIDE.md       # 开发引导
│   ├── development-plan.md        # 当前开发计划
│   ├── code-inventory.md          # 代码资产盘点
│   ├── requirements-code-mapping.md  # 需求代码映射表
│   ├── requirements-structured.md # 结构化需求文档
│   ├── volcore-development-guide.md  # Vol.Core 开发规范
│   ├── lessons-learned.md         # 经验教训
│   └── [其他文档]                 # 其他参考文档
├── src/
│   └── feature-records/           # 历史需求实现记录
│       ├── copilot-project-path/  # Copilot 项目路径关联需求
│       ├── kanban-frontend-backend-check/  # 看板前后端检查需求
│       └── manual-kanban-creation/  # 手动看板创建需求
└── README.md                      # 项目主文档
```

## 目录用途说明

### 1. docs/ - 项目文档目录

**用途**：存放项目的通用文档、指南、模板和当前开发计划。

**包含内容**：
- **templates/**：文档模板目录，包含标准化的文档模板供新需求使用
- **DEVELOPMENT-GUIDE.md**：开发引导文档，提供详细的开发流程指导
- **development-plan.md**：当前开发计划，显示当前要做的任务和进度
- **code-inventory.md**：代码资产盘点报告，记录现有代码结构
- **requirements-code-mapping.md**：需求代码映射表，记录需求与代码的对应关系
- **requirements-structured.md**：结构化需求文档，记录项目的整体需求
- **volcore-development-guide.md**：Vol.Core 框架的开发规范
- **lessons-learned.md**：经验教训记录
- **其他文档**：其他参考文档和说明文档

**特点**：
- 通用性强，适用于整个项目
- 包含模板和指南，便于团队协作
- 显示当前开发状态，便于进度跟踪

### 2. docs/templates/ - 文档模板目录

**用途**：存放标准化的文档模板，供新需求开发时使用。

**包含内容**：
- **01-需求分析.md**：需求分析模板
- **02-后端实现方案.md**：后端实现方案模板
- **03-前端实现方案.md**：前端实现方案模板
- **04-实施步骤.md**：实施步骤模板
- **05-测试计划.md**：测试计划模板
- **06-知识沉淀.md**：知识沉淀模板
- **07-预期效果.md**：预期效果模板

**使用方法**：
1. 在 `src/feature-records/` 目录下创建新需求文件夹
2. 复制需要的模板文件到新文件夹
3. 根据实际需求填写文档内容

### 3. src/feature-records/ - 历史需求实现记录

**用途**：存放历史需求的完整实现记录，包括需求分析、技术方案、实施步骤、测试计划等。

**包含内容**：
- 每个需求一个文件夹，文件夹名称为需求名称
- 每个需求文件夹包含完整的文档记录：
  - README.md：需求概述和文档导航
  - 01-需求分析.md：功能需求分析和技术实现要点
  - 02-后端实现方案.md：后端技术实现细节
  - 03-前端实现方案.md：前端技术实现细节
  - 04-实施步骤.md：具体实施流程和部署步骤
  - 05-测试计划.md：测试方案与用例
  - 06-知识沉淀.md：开发经验和问题解决记录
  - 07-预期效果.md：功能预期与验收标准

**特点**：
- 完整记录需求实现过程
- 便于后续查阅和参考
- 形成知识沉淀和经验积累

## 文档分类

### 1. 通用文档

**位置**：`docs/` 目录下

**用途**：适用于整个项目的通用文档

**包括**：
- 开发引导
- 开发规范
- 代码资产盘点
- 需求代码映射表
- 结构化需求文档

### 2. 模板文档

**位置**：`docs/templates/` 目录下

**用途**：标准化的文档模板，供新需求使用

**包括**：
- 需求分析模板
- 后端实现方案模板
- 前端实现方案模板
- 实施步骤模板
- 测试计划模板
- 知识沉淀模板
- 预期效果模板

### 3. 历史需求文档

**位置**：`src/feature-records/` 目录下

**用途**：记录历史需求的完整实现过程

**包括**：
- 每个需求的完整文档记录
- 需求分析、技术方案、实施步骤等
- 知识沉淀和经验总结

### 4. 当前开发文档

**位置**：`docs/development-plan.md`

**用途**：显示当前要做的任务和进度

**包括**：
- 当前开发阶段
- 任务列表和状态
- 进度跟踪

## 文档使用指南

### 1. 开始新需求开发

1. 查看 `docs/code-inventory.md` 了解现有代码结构
2. 参考 `docs/DEVELOPMENT-GUIDE.md` 了解开发流程
3. 在 `src/feature-records/` 创建新需求文件夹
4. 从 `docs/templates/` 复制需要的模板文件
5. 填写文档内容

### 2. 查看历史需求

1. 进入 `src/feature-records/` 目录
2. 找到对应需求的文件夹
3. 查看相关文档

### 3. 更新项目文档

1. 需求完成后，更新 `docs/code-inventory.md`
2. 更新 `docs/requirements-code-mapping.md`
3. 更新项目根目录的 `README.md`
4. 编写 `src/feature-records/需求名称/06-知识沉淀.md`

### 4. 查看当前开发进度

1. 查看 `docs/development-plan.md`
2. 了解当前开发阶段和任务状态

## 文档维护原则

1. **及时更新**：文档应与代码同步更新
2. **保持一致**：使用统一的文档格式和结构
3. **清晰明了**：文档内容应清晰易懂
4. **便于查找**：合理组织文档结构，便于查找和使用
5. **知识沉淀**：记录开发经验和教训，形成知识库

## 注意事项

- `docs/` 目录下的文档是通用文档，不要随意修改
- `docs/templates/` 目录下的模板是标准模板，可根据实际需求调整
- `src/feature-records/` 目录下的文档是历史记录，不要删除或修改
- 新需求开发时，应在 `src/feature-records/` 下创建新文件夹
- 需求完成后，务必更新相关文档

## 相关文档

- [开发引导](./DEVELOPMENT-GUIDE.md)
- [开发计划](./development-plan.md)
- [文档模板](./templates/README.md)
- [代码资产盘点](./code-inventory.md)
