using Asp.Versioning;
using Omotemachi.Extensions;
using Omotemachi.Services;
using Omotemachi.Services.Jester;
using Omotemachi.Services.Wacky;
using Omotemachi.Tools;
using Microsoft.EntityFrameworkCore;

namespace Omotemachi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Configuration.AddEnvironmentVariables(prefix: "api_");
        builder.Services.AddDbContext<AppContext>(options =>
            options.UseNpgsql(builder.Configuration["DatabaseConnection"]
            ?? throw new InvalidOperationException("DB Connection string not found")));
        builder.Services.AddSingleton<Dictionary<string, int>>();
        builder.Services.AddScoped<IMembersService, MembersService>();
        builder.Services.AddScoped<IUserSettingsService, UserSettingsService>();
        builder.Services.AddScoped<IEconomyService, EconomyService>();
        builder.Services.AddScoped<IInventoryService, InventoryService>();
        builder.Services.AddScoped<ILootboxesService, LootboxesService>();
        builder.Services.AddScoped<IShopService, ShopService>();
        builder.Services.AddScoped<IQuestsService, QuestsService>();
        builder.Services.AddScoped<ITicketsService, TicketsService>();
        builder.Services.AddScoped<IDuetsService, DuetsService>();
        builder.Services.AddScoped<IStatisticsService, StatisticsService>();
        builder.Services.AddScoped<IInteractionsService, InteractionsService>(); 
        builder.Services.AddScoped<ITopService, TopService>();
        builder.Services.AddScoped<IDNDService, DNDService>();
        builder.Services.AddScoped<ICCGService, CCGService>();
        builder.Services.AddScoped<IAppaService, AppaService>();
        builder.Services.AddScoped<IPacksService, PacksService>();
        builder.Services.AddHostedService<PackMaintenanceService>();
        builder.Services.AddSingleton<IImageRenderer, ImageRenderer>();
        builder.Services.AddHttpClient();
        builder.Services.AddConfigServices();
        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new QueryStringApiVersionReader("version")
            );
        }).AddMvc();

        var app = builder.Build();
        System.AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        System.AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
