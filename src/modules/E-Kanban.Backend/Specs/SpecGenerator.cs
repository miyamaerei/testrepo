using System;
using System.Text;
using System.Threading.Tasks;
using E_Kanban.Backend.AiExecution;
using E_Kanban.Backend.IRepository;
using E_Kanban.Backend.Models;
using Microsoft.Extensions.Logging;
using VOL.Core.Extensions.AutofacManager;

namespace E_Kanban.Backend.Specs;

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
        _logger.LogInformation($"Generating Spec for card: {card.Id} - {card.Title}");

        var prompt = new StringBuilder();
        prompt.AppendLine("我需要为这个 E-Kanban 执行任务生成一个验收 Spec 定义。");
        prompt.AppendLine();
        prompt.AppendLine("任务卡片信息：");
        prompt.AppendLine($"标题：{card.Title}");
        prompt.AppendLine($"描述：");
        prompt.AppendLine(card.Description);
        prompt.AppendLine();
        prompt.AppendLine("请帮我生成一份清晰、可验证的完成标准 Spec。要求：");
        prompt.AppendLine("1. 列出所有必须完成的验收条件");
        prompt.AppendLine("2. 每个条件应该是可验证的（能明确判断是否满足）");
        prompt.AppendLine("3. 使用简洁的 Markdown 格式");
        prompt.AppendLine("4. 只输出 Spec 内容，不需要额外解释");

        var specContent = await _copilotCli.ExecutePromptAsync(prompt.ToString());

        var spec = new Spec
        {
            ExecutionCardId = card.Id,
            Definition = specContent,
            CreatedAt = DateTime.UtcNow
        };

        var newId = await _specRepository.InsertAsync(spec);
        spec.Id = newId;
        _logger.LogInformation($"Generated Spec {spec.Id} for card {card.Id}");

        return spec;
    }
}
