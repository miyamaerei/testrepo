using System.Threading.Tasks;

namespace E_Kanban.Backend.AiExecution;

public interface ICopilotCliClient
{
    Task<string> ExecutePromptAsync(string prompt);
}
