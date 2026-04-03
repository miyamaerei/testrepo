using System.Threading.Tasks;
using E_Kanban.Backend.Models;

namespace E_Kanban.Backend.IServices;

public interface IStateMachineService
{
    Task<bool> CanTransitionAsync(ExecutionCard card, ExecutionCardStatus newStatus);
    Task TransitionToAsync(ExecutionCard card, ExecutionCardStatus newStatus);
    Task StartAiExecutionAsync(ExecutionCard card);
    Task CompleteAiExecutionAsync(ExecutionCard card, bool isSpecPassed);
}
