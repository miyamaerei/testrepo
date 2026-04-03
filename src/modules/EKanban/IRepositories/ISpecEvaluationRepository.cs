using VOL.Core.BaseProvider;
using VOL.Entity.DomainModels;
using System.Threading.Tasks;

namespace EKanban.IRepositories
{
    public partial interface ISpecEvaluationRepository : IRepository<SpecEvaluation>
    {
        Task AddAsync(SpecEvaluation evaluation);
    }
}
