using System.Threading.Tasks;
using VOL.Core.BaseProvider;
using VOL.Entity.DomainModels;

namespace EKanban.IServices
{
    public interface ISubmitService
    {
        Task<SubmitResult> SubmitExecutionResultAsync(int cardId, int executorType, string executorName, string evidence, string output);
    }

    public class SubmitResult
    {
        public bool IsSuccess { get; set; }
        public bool IsSpecPassed { get; set; }
        public string EvaluationResult { get; set; }
        public ExecutionCardStatus NewStatus { get; set; }
    }
}
