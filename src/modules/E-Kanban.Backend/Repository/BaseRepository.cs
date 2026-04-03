using System.Linq.Expressions;
using SqlSugar;
using E_Kanban.Backend.IRepository;

namespace E_Kanban.Backend.Repository;

public class BaseRepository<T> : IBaseRepository<T> where T : class, new()
{
    protected readonly SqlSugarClient _db;

    public BaseRepository(SqlSugarClient db)
    {
        _db = db;
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _db.Queryable<T>().InSingleAsync(id);
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _db.Queryable<T>().ToListAsync();
    }

    public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> where)
    {
        return await _db.Queryable<T>().Where(where).ToListAsync();
    }

    public async Task<(int total, List<T> items)> GetPageListAsync(Expression<Func<T, bool>> where, int pageIndex, int pageSize)
    {
        var total = await _db.Queryable<T>().Where(where).CountAsync();
        var items = await _db.Queryable<T>().Where(where).ToPageListAsync(pageIndex, pageSize);
        return (total, items);
    }

    public async Task<int> InsertAsync(T entity)
    {
        return await _db.Insertable(entity).ExecuteReturnIdentityAsync();
    }

    public async Task<int> InsertRangeAsync(List<T> entities)
    {
        return await _db.Insertable(entities).ExecuteCommandAsync();
    }

    public async Task<bool> UpdateAsync(T entity)
    {
        return await _db.Updateable(entity).ExecuteCommandHasChangeAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _db.Deleteable<T>().In(id).ExecuteCommandHasChangeAsync();
    }

    public async Task<bool> DeleteAsync(Expression<Func<T, bool>> where)
    {
        return await _db.Deleteable<T>().Where(where).ExecuteCommandHasChangeAsync();
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> where)
    {
        return await _db.Queryable<T>().Where(where).AnyAsync();
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> where)
    {
        return await _db.Queryable<T>().Where(where).CountAsync();
    }
}
