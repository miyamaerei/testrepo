using SqlSugar;
using VOL.Core.BaseProvider;
using VOL.Core.DbContext;
using EKanban.Models;
using EKanban.IRepositories;

namespace EKanban.Repositories;

public class SpecRepository : RepositoryBase<Spec>, ISpecRepository
{
    public SpecRepository(VOLContext dbContext) : base(dbContext)
    {
    }
}
