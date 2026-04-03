using System.Threading.Tasks;
using E_Kanban.Backend.Models;

namespace E_Kanban.Backend.Specs
{
    public interface ISpecEvaluator
    {
        Task<SpecEvaluationResult> EvaluateAsync(int specId, int executionRunId, string evidence);
    }

    public class SpecEvaluationResult
    {
        public bool IsPassed { get; set; }
        public string EvaluationResult { get; set; }
    }
}
