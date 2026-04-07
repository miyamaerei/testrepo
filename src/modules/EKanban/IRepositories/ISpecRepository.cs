using EKanban.Models;
using VOL.Core.BaseProvider;
using VOL.Core.Extensions.AutofacManager;

namespace EKanban.IRepositories;

public interface ISpecRepository : IRepository<Spec>, IDependency
{
}
