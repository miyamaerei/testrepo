using System;
using System.Diagnostics;
using System.Threading.Tasks;
using EKanban.AiExecution;
using EKanban.IRepositories;
using Microsoft.Extensions.Logging;
using VOL.Entity.DomainModels;
using VOL.Core.Extensions.AutofacManager;

namespace EKanban.Specs
{
    public class SpecGenerator : ISpecGenerator, IDependency
    {
        private readonly ISpecRepository _specRepository;
        private readonly ICopilotCliClient _copilotCli;
        private readonly ILogger<SpecGenerator> _logger;

        public SpecGenerator(
            ISpecRepository specRepository,
            ICopilotCliClient copilotCli,
            ILogger<SpecGenerator> logger)
        {
            _specRepository = specRepository;
            _copilotCli = copilotCli;
            _logger = logger;
        }

        public async Task<Spec> GenerateSpecAsync(ExecutionCard card)
        {
            _logger.LogInformation($"Generating Spec for card: {card.ExecutionCardId} - {card.Title}");

            var prompt = $@"
我需要为这个 E-Kanban 执行任务生成一个验收 Spec 定义。

任务卡片信息：
标题：{card.Title}
描述：
{card.Description}

请帮我生成一份清晰、可验证的完成标准 Spec。要求：
1. 列出所有必须完成的验收条件
2. 每个条件应该是可验证的（能明确判断是否满足）
3. 使用简洁的 Markdown 格式
4. 只输出 Spec 内容，不需要额外解释
";

            var specContent = await _copilotCli.ExecutePromptAsync(prompt);

            var spec = new Spec
            {
                ExecutionCardId = card.ExecutionCardId,
                SpecContent = specContent,
                CreatedDate = DateTime.UtcNow
            };

            await _specRepository.AddAsync(spec);
            _logger.LogInformation($"Generated Spec {spec.SpecId} for card {card.ExecutionCardId}");

            return spec;
        }
    }
}
