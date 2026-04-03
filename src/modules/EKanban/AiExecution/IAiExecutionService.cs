using System.Threading.Tasks;
using VOL.Entity.DomainModels;

namespace EKanban.AiExecution
{
    public interface IAiExecutionService
    {
        Task ExecuteAiTaskAsync(ExecutionCard card);
    }
}
