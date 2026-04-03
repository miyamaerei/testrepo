using SqlSugar;
using E_Kanban.Backend.Models;
using E_Kanban.Backend.IRepository;

namespace E_Kanban.Backend.Repository;

public class SpecEvaluationRepository : BaseRepository<SpecEvaluation>, ISpecEvaluationRepository
{
    public SpecEvaluationRepository(SqlSugarClient db) : base(db)
    {
    }
}
