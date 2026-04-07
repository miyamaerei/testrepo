using EKanban.Models;
using VOL.Core.BaseProvider;

namespace EKanban.IRepositories;

public interface ISpecRepository : IRepository<Spec>, IDependency
{
}
