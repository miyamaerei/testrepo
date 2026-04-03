using VOL.Core.BaseProvider;
using VOL.Entity.DomainModels;
using System.Threading.Tasks;

namespace EKanban.IRepositories
{
    public partial interface ISpecRepository : IRepository<Spec>
    {
        Task<Spec> FindOneAsync(int id);
        Task AddAsync(Spec spec);
    }
}
