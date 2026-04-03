using System.Text;
using System;
using System.Threading.Tasks;
using EKanban.IRepositories;
using EKanban.IServices;
using EKanban.Specs;
using Microsoft.Extensions.Logging;
using VOL.Entity.DomainModels;
using VOL.Core.Extensions.AutofacManager;

namespace EKanban.AiExecution
{
    public class AiExecutionService : IAiExecutionService, IDependency
    {
        private readonly IExecutionCardRepository _executionCardRepository;
        private readonly IStateMachineService _stateMachineService;
        private readonly ISubmitService _submitService;
        private readonly ICopilotCliClient _copilotCli;
        private readonly ILogger<AiExecutionService> _logger;

        public AiExecutionService(
            IExecutionCardRepository executionCardRepository,
            IStateMachineService stateMachineService,
            ISubmitService submitService,
            ICopilotCliClient copilotCli,
            ILogger<AiExecutionService> logger)
        {
            _executionCardRepository = executionCardRepository;
            _stateMachineService = stateMachineService;
            _submitService = submitService;
            _copilotCli = copilotCli;
            _logger = logger;
        }

        public async Task ExecuteAiTaskAsync(ExecutionCard card)
        {
            _logger.LogInformation($"Starting AI execution for card {card.ExecutionCardId}: {card.Title}");

            try
            {
                // Mark as InProgress
                await _stateMachineService.StartAiExecutionAsync(card);

                // Refresh the card after state transition
                card = await _executionCardRepository.FindOneAsync(card.ExecutionCardId);

                // Build the execution prompt
                var prompt = BuildExecutionPrompt(card);

                // Execute via Copilot CLI
                var output = await _copilotCli.ExecutePromptAsync(prompt);

                // Submit the result automatically
                var result = await _submitService.SubmitExecutionResultAsync(
                    card.ExecutionCardId,
                    (int)ExecutorType.AI,
                    "GitHub Copilot CLI",
                    output,
                    output);

                _logger.LogInformation(
                    "AI execution completed for card {CardId}, Spec passed: {IsPassed}, new status: {NewStatus}",
                    card.ExecutionCardId, result.IsSpecPassed, result.NewStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AI execution failed for card {card.ExecutionCardId}");

                // Get fresh card instance
                var cardRef = await _executionCardRepository.FindOneAsync(card.ExecutionCardId);
                if (cardRef != null && cardRef.Status == (int)ExecutionCardStatus.InProgress)
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

            if (card.CurrentSpecId.HasValue)
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
}
