# 经验教训记录

## 问题：项目目录结构对齐问题

### 问题描述
一开始在 `E-Kanban/` 下面又创建了 `E-Kanban/` 子目录，导致层级不对，Vol.Core 框架要求直接在解决方案根目录下创建 `EKanban/`（没有连字符）作为业务模块。

### 原因分析
对 Vue.NetCore (Vol.Core) 框架的项目结构不熟悉，框架约定：
- 解决方案根目录直接是各个项目的层级
- 业务模块直接放在根目录，命名为 `EKanban`（没有连字符）
- `VOL.Entity`, `VOL.Core`, `VOL.WebApi` 都在根目录

### 解决方法
将 `E-Kanban/E-Kanban.Backend/EKanban/` 移动到解决方案根目录 `./EKanban/`，保持和框架其他模块（`VOL.Sys`, `VOL.MES`）一致的结构。

### 预防措施
- 创建新模块前先看框架已有模块的目录结构
- 遵循框架约定，保持一致

---

## 问题：GitHub Copilot CLI 调用方式

### 问题描述
一开始不确定怎么在 C# 中调用 `copilot` 编程模式，获取输出结果。

### 原因分析
GitHub Copilot CLI 的编程模式 (`-p`) 是比较新的功能，文档不多。

### 解决方法
使用 `System.Diagnostics.Process` 启动 `copilot` 进程，设置：
- `RedirectStandardOutput = true` 捕获标准输出
- `RedirectStandardError = true` 捕获错误输出
- `UseShellExecute = false` 必须设置为 false 才能重定向
- 设置超时时间，超时强制杀死进程

### 代码示例：
```csharp
var process = new Process
{
    StartInfo = new ProcessStartInfo
    {
        FileName = _options.CommandPath,
        Arguments = $"-p \"{prompt.Escape()}\" --allow-all-tools",
        WorkingDirectory = _options.WorkingDirectory,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    },
    EnableRaisingEvents = true
};
process.Start();
// 等待输出...
var output = process.StandardOutput.ReadToEnd();
```

### 预防措施
- 保留这个示例，后续调用外部 CLI 都用这个模式
- 一定要设置超时，防止进程挂住

---

## 问题：防偷懒机制设计

### 问题描述
一开始想不清楚 AI 分批迭代执行和防偷懒的具体逻辑应该怎么设计。

### 原因分析
需求比较新颖，AI 分批执行 + 超时检测 + 自动重试 + 人工干预这个机制需要仔细梳理流程。

### 解决方法
梳理出清晰的状态流转和扫描逻辑：

1. **状态流转**：
```
New → Ready → InProgress → Submitted → (Spec 评估) → Completed / Ready (重试)
```

2. **超时检测逻辑**：
```csharp
foreach (var card in inProgressAiCards)
{
    if (Now - card.LastUpdated > timeout && !card.IsStale)
    {
        if (card.FailureCount < maxRetries)
        {
            // 自动重试
            card.Status = CardStatus.Ready;
            card.FailureCount++;
        }
        else
        {
            // 需要人工干预
            card.NeedsManualIntervention = true;
        }
    }
}
```

3. **前端支持**：
- 对需要人工干预的卡片显示红色标记
- 提供"重新执行"按钮，管理员手动触发

---

## 问题：Azure Boards API 认证

### 问题描述
不清楚 Azure DevOps REST API 认证方式，怎么用 PAT 认证。

### 解决方法
- PAT 需要添加到 HTTP Header `Authorization: Basic BASE64(PAT)`
- 编码方式：`":pat"` → base64 编码
- PAT 需要权限：Work Items (Read & Write)

示例代码：
```csharp
var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{pat}"));
_client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Basic", credentials);
```

---

## 总结

### 本次开发获得的经验：

1. **遵循框架约定**：在已有框架上增量开发，**一定要**遵循框架现有的目录结构、命名规范、分层方式，不要自己发明一套。

2. **小步快跑，依赖顺序开发**：按照先实体 → 仓储 → 服务 → API → 前端的顺序，一步一步来，每一步验证编译，比一下子写很多代码再调试好很多。

3. **GitHub Copilot CLI 编程模式非常强大**：可以被程序调用自动完成开发任务，这是实现 AI 自动执行调度的基础。

4. **结构化开发方法论非常有效**：
   - 需求结构化分析 → 提前理清所有需求
   - 代码资产盘点 → 了解现有框架
   - 需求-代码映射 → 知道每个功能放哪里
   - 增量开发按批次执行 → 不会乱

5. **防偷懒机制是 AI 自动执行的关键保障**：AI 会卡住，会失败，必须有超时检测、自动重试、人工干预三级保障。

---

创建日期：2026-04-01
