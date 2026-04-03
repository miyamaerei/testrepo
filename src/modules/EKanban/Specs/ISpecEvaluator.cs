using System;
using System.Threading.Tasks;
using VOL.Entity.DomainModels;

namespace EKanban.Specs
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
