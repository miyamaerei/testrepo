using EKanban.Models;
using VOL.Core.BaseProvider;
using VOL.Entity.DomainModels;

namespace EKanban.IRepositories
{
    public partial interface IExecutionTaskRepository : IRepository<EKanban.Models.ExecutionTask>
    {
    }
}
