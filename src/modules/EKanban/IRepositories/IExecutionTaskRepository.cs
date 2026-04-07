using EKanban.Models;
using VOL.Core.BaseProvider;
using VOL.Entity.DomainModels;
using VOL.Core.Extensions.AutofacManager;

namespace EKanban.IRepositories
{
    public partial interface IExecutionTaskRepository : IRepository<EKanban.Models.ExecutionTask>, IDependency
    {
    }
}
