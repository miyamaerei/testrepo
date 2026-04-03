using VOL.Core.BaseProvider;
using VOL.Entity.DomainModels;
using System.Threading.Tasks;

namespace EKanban.IRepositories
{
    public partial interface IExecutionRunRepository : IRepository<ExecutionRun>
    {
        Task<ExecutionRun> FindOneAsync(int id);
        Task AddAsync(ExecutionRun run);
    }
}
