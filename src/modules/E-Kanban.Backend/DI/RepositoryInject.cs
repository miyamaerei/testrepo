using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using E_Kanban.Backend.IRepository;
using E_Kanban.Backend.Repository;

namespace E_Kanban.Backend.DI;

public static class RepositoryInject
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var repositoryTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Any(i => i.Name.StartsWith("I")))
            .ToList();

        foreach (var type in repositoryTypes)
        {
            var interfaceType = type.GetInterfaces().FirstOrDefault(i => i.Name == "I" + type.Name);
            if (interfaceType != null)
            {
                services.AddScoped(interfaceType, type);
            }
        }

        return services;
    }
}
