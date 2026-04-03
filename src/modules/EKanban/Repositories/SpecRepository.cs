using SqlSugar;
using VOL.Core.DbContext;
using VOL.Core.BaseProvider;
using VOL.Entity.DomainModels;
using EKanban.IRepositories;
using System.Threading.Tasks;

namespace EKanban.Repositories
{
    public partial class SpecRepository : RepositoryBase<Spec>, ISpecRepository
    {
        public SpecRepository(VOLContext dbContext) : base(dbContext)
        {
        }

        public async Task<Spec> FindOneAsync(int id)
        {
            return await DbContext.Queryable<Spec>().In(id).FirstAsync();
        }

        public async Task AddAsync(Spec spec)
        {
            await DbContext.Insertable(spec).ExecuteCommandAsync();
        }
    }
}
