using System.Threading.Tasks;
using EKanban.Models;

namespace EKanban.AiExecution;

public interface IAiExecutionService
{
    Task ExecuteAiTaskAsync(ExecutionCard card);
}
