using SqlSugar;
using VOL.Core.DbContext;
using VOL.Core.BaseProvider;
using VOL.Entity.DomainModels;
using EKanban.IRepositories;
using System.Threading.Tasks;

namespace EKanban.Repositories
{
    public partial class SpecEvaluationRepository : RepositoryBase<SpecEvaluation>, ISpecEvaluationRepository
    {
        public SpecEvaluationRepository(VOLContext dbContext) : base(dbContext)
        {
        }

        public async Task AddAsync(SpecEvaluation evaluation)
        {
            await DbContext.Insertable(evaluation).ExecuteCommandAsync();
        }
    }
}
