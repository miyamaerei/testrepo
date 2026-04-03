using VOL.Core.BaseProvider;
using VOL.Entity.DomainModels;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EKanban.IRepositories
{
    public partial interface IBoardWorkItemRepository : IRepository<BoardWorkItem>
    {
        Task<BoardWorkItem> FindSingleAsync(Expression<Func<BoardWorkItem, bool>> predicate);
        Task AddAsync(BoardWorkItem item);
        Task UpdateAsync(BoardWorkItem item);
    }
}
