using System.Threading.Tasks;

namespace EKanban.AiExecution;

public interface ICopilotCliClient
{
    Task<string> ExecutePromptAsync(string prompt, string workingDirectory = null);
}
