using System;
using System.Threading.Tasks;
using VOL.Entity.DomainModels;

namespace EKanban.Specs
{
    public interface ISpecGenerator
    {
        Task<Spec> GenerateSpecAsync(ExecutionCard card);
    }
}
