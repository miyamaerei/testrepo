using SqlSugar;
using VOL.Core.BaseProvider;
using VOL.Core.DbContext;
using EKanban.Models;
using EKanban.IRepositories;

namespace EKanban.Repositories;

public class ProjectRepositoriesRepository : RepositoryBase<ProjectRepositories>, IProjectRepositoriesRepository
{
    public ProjectRepositoriesRepository(VOLContext dbContext) : base(dbContext)
    {
    }
}
