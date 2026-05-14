using Omotemachi.Infrastructure.Logging.Dispatcher;
using Omotemachi.Infrastructure.Logging.Queue;
using Omotemachi.Models.V1.Domain.Jester.Config;
using Omotemachi.Services;
using Omotemachi.Services.Jester;
using Omotemachi.Services.Logs;
using Omotemachi.Services.Wacky;
using System.Reflection;

namespace Omotemachi.Extensions;

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
        
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<Dictionary<string, int>>();

        services.AddScoped<IMembersService, MembersService>();
        services.AddScoped<IUserSettingsService, UserSettingsService>();
        services.AddScoped<IEconomyService, EconomyService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<ILootboxesService, LootboxesService>();
        services.AddScoped<IShopService, ShopService>();
        services.AddScoped<IQuestsService, QuestsService>();
        services.AddScoped<ITicketsService, TicketsService>();
        services.AddScoped<IDuetsService, DuetsService>();
        services.AddScoped<IStatisticsService, StatisticsService>();
        services.AddScoped<IInteractionsService, InteractionsService>();
        services.AddScoped<ITopService, TopService>();
        services.AddScoped<IDNDService, DNDService>();
        services.AddScoped<ICCGService, CCGService>();
        services.AddScoped<IAppaService, AppaService>();
        services.AddScoped<IPacksService, PacksService>();
        AddConfigServices(services);

        services.AddScoped<IWebhookDispatcher, LogDispatcher>();
        services.AddScoped<ILogsService, LogsService>();
        services.AddSingleton<ILogQueue, LogQueue>();

        return services;
    }
}