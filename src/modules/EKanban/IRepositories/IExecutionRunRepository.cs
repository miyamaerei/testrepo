using EKanban.Models;
using VOL.Core.BaseProvider;

namespace EKanban.IRepositories;

public interface IExecutionRunRepository : IRepository<ExecutionRun>, IDependency
{
}
