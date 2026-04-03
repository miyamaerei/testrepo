using SqlSugar;
using VOL.Core.DbContext;
using VOL.Core.BaseProvider;
using VOL.Entity.DomainModels;
using EKanban.IRepositories;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EKanban.Repositories
{
    public partial class BoardWorkItemRepository : RepositoryBase<BoardWorkItem>, IBoardWorkItemRepository
    {
        public BoardWorkItemRepository(VOLContext dbContext) : base(dbContext)
        {
        }

        public async Task<BoardWorkItem> FindSingleAsync(Expression<Func<BoardWorkItem, bool>> predicate)
        {
            return await DbContext.Queryable<BoardWorkItem>().Where(predicate).FirstAsync();
        }

        public async Task AddAsync(BoardWorkItem item)
        {
            await DbContext.Insertable(item).ExecuteCommandAsync();
        }

        public async Task UpdateAsync(BoardWorkItem item)
        {
            await DbContext.Updateable(item).ExecuteCommandAsync();
        }
    }
}
