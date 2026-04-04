using SqlSugar;
using VOL.Core.BaseProvider;
using VOL.Core.DbContext;
using EKanban.Models;
using EKanban.IRepositories;

namespace EKanban.Repositories;

public class SpecEvaluationRepository : RepositoryBase<SpecEvaluation>, ISpecEvaluationRepository
{
    public SpecEvaluationRepository(VOLContext dbContext) : base(dbContext)
    {
    }
}
