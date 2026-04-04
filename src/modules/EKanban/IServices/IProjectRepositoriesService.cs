using System.Threading.Tasks;
using EKanban.Models;
using System.Collections.Generic;

namespace EKanban.IServices;

public interface IProjectRepositoriesService
{
    Task<List<ProjectRepositories>> GetAllAsync();
    Task<ProjectRepositories?> GetByIdAsync(int id);
    Task<ProjectRepositories> CreateAsync(ProjectRepositories entity);
    Task<bool> UpdateAsync(ProjectRepositories entity);
    Task<bool> DeleteAsync(int id);
}
