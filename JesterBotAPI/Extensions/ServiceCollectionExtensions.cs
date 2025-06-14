using DellArteAPI.Models.V1.Jester.Config;
using DellArteAPI.Services.Jester;
using System.Reflection;

namespace DellArteAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfigServices(this IServiceCollection services)
    {
        var configTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IConfig).IsAssignableFrom(t));

        foreach (var type in configTypes)
        {
            var serviceType = typeof(IConfigService<>).MakeGenericType(type);
            var implementationType = typeof(ConfigService<>).MakeGenericType(type);

            services.AddScoped(serviceType, implementationType);
        }

        return services;
    }
}