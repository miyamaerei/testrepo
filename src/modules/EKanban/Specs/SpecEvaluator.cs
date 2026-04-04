using System;
using System.Threading.Tasks;
using EKanban.AiExecution;
using EKanban.IRepositories;
using EKanban.Models;
using Microsoft.Extensions.Logging;
using VOL.Core.Extensions.AutofacManager;

namespace EKanban.Specs;

public class SpecEvaluator : ISpecEvaluator, IDependency
{
    private readonly ISpecRepository _specRepositories;
    private readonly IExecutionRunRepository _executionRunRepositories;
    private readonly ISpecEvaluationRepository _specEvaluationRepositories;
    private readonly ICopilotCliClient _copilotCli;
    private readonly ILogger<SpecEvaluator> _logger;

    public SpecEvaluator(
        ISpecRepository specRepositories,
        IExecutionRunRepository executionRunRepositories,
        ISpecEvaluationRepository specEvaluationRepositories,
        ICopilotCliClient copilotCli,
        ILogger<SpecEvaluator> logger)
    {
        _specRepositories = specRepositories;
        _executionRunRepositories = executionRunRepositories;
        _specEvaluationRepositories = specEvaluationRepositories;
        _copilotCli = copilotCli;
        _logger = logger;
    }

    public async Task<SpecEvaluationResult> EvaluateAsync(int specId, int executionRunId, string evidence)
    {
        _logger.LogInformation($"Evaluating execution run {executionRunId} against Spec {specId}");

        var spec = await _specRepositories.FindFirstAsync(p => p.Id == specId);
        if (spec == null)
        {
            throw new ArgumentException($"Spec {specId} not found");
        }

        var run = await _executionRunRepositories.FindFirstAsync(p => p.Id == executionRunId);
        if (run == null)
        {
            throw new ArgumentException($"Execution run {executionRunId} not found");
        }

        var prompt = $@"
请根据 Spec 验收标准评估这份提交的证据是否满足所有要求。

Spec 验收标准：
{spec.Definition}

提交的证据：
{evidence}

请评估：
1. 所有验收条件是否都满足？
2. 如果有不满足的，请指出哪些不满足。
最后请给出结论：PASS 或者 FAIL。

格式要求：
先列出你的评估分析，最后一行必须只写 ""PASS"" 或者 ""FAIL""。
";

        var evaluationResult = await _copilotCli.ExecutePromptAsync(prompt);

        var isPassed = evaluationResult.Trim().EndsWith("PASS");

        var evaluation = new SpecEvaluation
        {
            SpecId = specId,
            ExecutionRunId = executionRunId,
            Message = evaluationResult,
            Result = isPassed ? EvaluationResult.Passed : EvaluationResult.Failed,
            EvaluatedAt = DateTime.UtcNow
        };

        _specEvaluationRepositories.Add(evaluation, true);

        _logger.LogInformation($"Evaluation completed for run {executionRunId}: {(isPassed ? "PASSED" : "FAILED")}");

        return new SpecEvaluationResult
        {
            IsPassed = isPassed,
            EvaluationResult = evaluationResult
        };
    }
}
