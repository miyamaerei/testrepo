using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EKanban.IRepositories;
using EKanban.IServices;
using EKanban.Models;
using VOL.Core.Extensions.AutofacManager;

namespace EKanban.Services;

public class ProjectRepositoriesService : IProjectRepositoriesService, IDependency
{
    private readonly IProjectRepositoriesRepository _repository;

    public ProjectRepositoriesService(IProjectRepositoriesRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ProjectRepositories>> GetAllAsync()
    {
        return await _repository.FindAsync(_ => true);
    }

    public async Task<ProjectRepositories?> GetByIdAsync(int id)
    {
        return await _repository.FindFirstAsync(p => p.Id == id);
    }

    public async Task<ProjectRepositories> CreateAsync(ProjectRepositories entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _repository.Add(entity, false);
        await _repository.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> UpdateAsync(ProjectRepositories entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _repository.Update<ProjectRepositories>(entity, (string[])null!, true);
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) return false;
        _repository.Delete(entity, true);
        return true;
    }
}
