using System.Linq.Expressions;
using VOL.Core.BaseProvider;

namespace EKanban.IRepositories;

public interface IRepositoryBase<T> where T : class, new()
{
    /// <summary>
    /// 根据 ID 查询
    /// </summary>
    Task<T?> GetByIdAsync(int id);
    
    /// <summary>
    /// 获取所有
    /// </summary>
    Task<List<T>> GetAllAsync();
    
    /// <summary>
    /// 根据条件查询
    /// </summary>
    Task<List<T>> GetListAsync(Expression<Func<T, bool>> where);
    
    /// <summary>
    /// 分页查询
    /// </summary>
    Task<(int total, List<T> items)> GetPageListAsync(Expression<Func<T, bool>> where, int pageIndex, int pageSize);
    
    /// <summary>
    /// 新增
    /// </summary>
    Task<int> InsertAsync(T entity);
    
    /// <summary>
    /// 批量新增
    /// </summary>
    Task<int> InsertRangeAsync(List<T> entities);
    
    /// <summary>
    /// 更新
    /// </summary>
    Task<bool> UpdateAsync(T entity);
    
    /// <summary>
    /// 删除
    /// </summary>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// 根据条件删除
    /// </summary>
    Task<bool> DeleteAsync(Expression<Func<T, bool>> where);
    
    /// <summary>
    /// 是否存在
    /// </summary>
    Task<bool> AnyAsync(Expression<Func<T, bool>> where);
    
    /// <summary>
    /// 计数
    /// </summary>
    Task<int> CountAsync(Expression<Func<T, bool>> where);
}
