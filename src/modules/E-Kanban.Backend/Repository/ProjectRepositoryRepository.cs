using SqlSugar;
using E_Kanban.Backend.Models;
using E_Kanban.Backend.IRepository;

namespace E_Kanban.Backend.Repository;

public class ProjectRepositoryRepository : BaseRepository<ProjectRepository>, IProjectRepositoryRepository
{
    public ProjectRepositoryRepository(SqlSugarClient db) : base(db)
    {
    }
}
