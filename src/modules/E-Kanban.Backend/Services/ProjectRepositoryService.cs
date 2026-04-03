using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using E_Kanban.Backend.IRepository;
using E_Kanban.Backend.IServices;
using E_Kanban.Backend.Models;
using VOL.Core.Extensions.AutofacManager;

namespace E_Kanban.Backend.Services;

public class ProjectRepositoryService : IProjectRepositoryService, IDependency
{
    private readonly IProjectRepositoryRepository _repository;

    public ProjectRepositoryService(IProjectRepositoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ProjectRepository>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<ProjectRepository?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<ProjectRepository> CreateAsync(ProjectRepository entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        var id = await _repository.InsertAsync(entity);
        entity.Id = id;
        return entity;
    }

    public async Task<bool> UpdateAsync(ProjectRepository entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        return await _repository.UpdateAsync(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }
}
