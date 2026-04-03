using SqlSugar;
using E_Kanban.Backend.Models;
using E_Kanban.Backend.IRepository;

namespace E_Kanban.Backend.Repository;

public class SpecRepository : BaseRepository<Spec>, ISpecRepository
{
    public SpecRepository(SqlSugarClient db) : base(db)
    {
    }
}
