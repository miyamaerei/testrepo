using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace E_Kanban.Backend.DI;

public static class ServiceInject
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var serviceTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Any(i => i.Name.StartsWith("I")))
            .ToList();

        foreach (var type in serviceTypes)
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
