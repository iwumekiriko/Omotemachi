using Asp.Versioning;
using Omotemachi.Extensions;

namespace Omotemachi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddEnvironmentVariables(prefix: "api_");
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var connectionString = builder.Configuration["DatabaseConnection"] 
            ?? throw new InvalidOperationException("DB Connection string not found");

        builder.Services.AddDatabase(connectionString);
        builder.Services.AddApplicationServices();
        builder.Services.AddBackgroundWorkers();

        builder.Services.AddHttpClient();
        builder.Services.AddConfigServices();

        builder.Services.AddCustomLogging(builder);

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
