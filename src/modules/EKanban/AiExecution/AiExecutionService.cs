using System;
using System.Text;
using System.Threading.Tasks;
using EKanban.IRepositories;
using EKanban.IServices;
using EKanban.Models;
using EKanban.Specs;
using Microsoft.Extensions.Logging;
using VOL.Core.Extensions.AutofacManager;

namespace EKanban.AiExecution;

public class AiExecutionService : IAiExecutionService, IDependency
{
    private readonly IExecutionCardRepository _executionCardRepositories;
    private readonly ITaskPhaseProgressRepository _taskPhaseProgressRepositories;
    private readonly IStateMachineService _stateMachineService;
    private readonly ISubmitService _submitService;
    private readonly ICopilotCliClient _copilotCli;
    private readonly ILogger<AiExecutionService> _logger;

    public AiExecutionService(
        IExecutionCardRepository executionCardRepositories,
        ITaskPhaseProgressRepository taskPhaseProgressRepositories,
        IStateMachineService stateMachineService,
        ISubmitService submitService,
        ICopilotCliClient copilotCli,
        ILogger<AiExecutionService> logger)
    {
        _executionCardRepositories = executionCardRepositories;
        _taskPhaseProgressRepositories = taskPhaseProgressRepositories;
        _stateMachineService = stateMachineService;
        _submitService = submitService;
        _copilotCli = copilotCli;
        _logger = logger;
    }

    public async Task ExecuteAiTaskAsync(ExecutionCard card)
    {
        _logger.LogInformation($"Starting AI execution for card {card.Id}: {card.Title}");

        try
        {
            // Mark as InProgress
            await _stateMachineService.StartAiExecutionAsync(card);

            // Initialize all six phases if not already initialized
            var existingPhases = await _taskPhaseProgressRepositories.GetByExecutionCardIdAsync(card.Id);
            if (!existingPhases.Any())
            {
                // Initialize all six development phases with NotStarted status
                foreach (DevelopmentPhase phase in Enum.GetValues(typeof(DevelopmentPhase)))
                {
                    var phaseProgress = new TaskPhaseProgress
                    {
                        ExecutionCardId = card.Id,
                        Phase = phase,
                        Status = PhaseStatus.NotStarted
                    };
                    _taskPhaseProgressRepositories.Add(phaseProgress, false);
                }
                await _taskPhaseProgressRepositories.SaveChangesAsync();
            }

            // Find the first not started phase and mark it as InProgress
            var allPhases = await _taskPhaseProgressRepositories.GetByExecutionCardIdAsync(card.Id);
            var currentPhase = allPhases.FirstOrDefault(p => p.Status == PhaseStatus.NotStarted);
            if (currentPhase != null)
            {
                currentPhase.Status = PhaseStatus.InProgress;
                currentPhase.StartedAt = DateTime.UtcNow;
                _taskPhaseProgressRepositories.Update<TaskPhaseProgress>(currentPhase, (string[])null!, false);
                await _taskPhaseProgressRepositories.SaveChangesAsync();
                _logger.LogInformation($"Started phase {currentPhase.Phase} for card {card.Id}");
            }

            // Refresh the card after state transition
            card = await _executionCardRepositories.FindFirstAsync(p => p.Id == card.Id);

            // Build the execution prompt
            var prompt = BuildExecutionPrompt(card);

            // Execute via Copilot CLI
            var output = await _copilotCli.ExecutePromptAsync(prompt);

            // Submit the result automatically
            var result = await _submitService.SubmitExecutionResultAsync(
                card.Id,
                (int)ExecutorType.AI,
                "GitHub Copilot CLI",
                output,
                output);

            _logger.LogInformation(
                "AI execution completed for card {CardId}, Spec passed: {IsPassed}, new status: {NewStatus}",
                card.Id, result.IsSpecPassed, result.NewStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"AI execution failed for card {card.Id}");

            // Get fresh card instance
            var cardRef = await _executionCardRepositories.FindFirstAsync(p => p.Id == card.Id);
            if (cardRef != null && cardRef.Status == ExecutionCardStatus.InProgress)
            {
                cardRef.FailureCount++;
                await _stateMachineService.TransitionToAsync(cardRef, ExecutionCardStatus.Ready);
            }

            throw;
        }
    }

    private string BuildExecutionPrompt(ExecutionCard card)
    {
        var prompt = new StringBuilder();

        prompt.AppendLine(@"你现在需要执行 E-Kanban 中的一项开发任务。请按照要求完成工作。");
        prompt.AppendLine();
        prompt.AppendLine("任务信息：");
        prompt.AppendLine($"标题: {card.Title}");
        prompt.AppendLine($"描述:");
        prompt.AppendLine(card.Description);
        prompt.AppendLine();

        if (card.SpecId.HasValue)
        {
            prompt.AppendLine("验收标准(Spec)：");
            // TODO: Load spec from database and append
            prompt.AppendLine("需要满足上述 Spec 中的所有验收条件");
            prompt.AppendLine();
        }

        prompt.AppendLine("请：");
        prompt.AppendLine("1. 理解任务需求");
        prompt.AppendLine("2. 修改/创建符合要求的代码");
        prompt.AppendLine("3. 在输出中说明你做了哪些修改，以及如何验证满足需求");
        prompt.AppendLine();
        prompt.AppendLine("你的执行结果：");

        return prompt.ToString();
    }
}
