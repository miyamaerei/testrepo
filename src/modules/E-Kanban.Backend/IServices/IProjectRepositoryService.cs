using System.Threading.Tasks;
using E_Kanban.Backend.Models;
using System.Collections.Generic;

namespace E_Kanban.Backend.IServices;

public interface IProjectRepositoryService
{
    Task<List<ProjectRepository>> GetAllAsync();
    Task<ProjectRepository?> GetByIdAsync(int id);
    Task<ProjectRepository> CreateAsync(ProjectRepository entity);
    Task<bool> UpdateAsync(ProjectRepository entity);
    Task<bool> DeleteAsync(int id);
}
