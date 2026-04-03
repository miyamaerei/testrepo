using System.Threading.Tasks;
using E_Kanban.Backend.Models;

namespace E_Kanban.Backend.AiExecution;

public interface IAiExecutionService
{
    Task ExecuteAiTaskAsync(ExecutionCard card);
}
