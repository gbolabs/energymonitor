using energymeasures.Config;
using Microsoft.EntityFrameworkCore;

namespace energymeasures.Services;

public static class Registrar
{
    public static void AddLogging(this WebApplicationBuilder builder)
    {
        builder.Services.AddLogging(b =>
        {
            if (builder.Environment.EnvironmentName == "Development")
                b.AddConsole();
            else
                // Application insights using the instrumentation key
                b.AddApplicationInsights(builder.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);
        });
    }

    public static void AddBusinessLogic(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<MeasureProvider>();
    }
    
    public static void AddDatabases(this WebApplicationBuilder builder)
    {
        builder.Services.AddCosmos<CosmosDbContext>(builder.Configuration["pr114energymeasures"],
            builder.Configuration["CosmosDbName"]);
        builder.Services.AddCosmos<SolarProductionCosmosDbContext>(builder.Configuration["pr114energymeasures"],
            "solar-production");
        
        builder.Services.AddSingleton(new CosmosProvider(
            builder.Configuration["pr114energymeasures"],
            builder.Configuration["CosmosDbName"]));
        builder.Services.AddSingleton(new CosmosSolarProductionProvider(
            builder.Configuration["pr114energymeasures"]));
        
        builder.Services.AddDbContext<MyDatabaseContext>(options =>
            options.UseSqlServer(builder.Configuration["AZURE_SQL_CONNECTIONSTRING"]));
    }

    public static void AddConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<MyStromProductionConfig>(builder.Configuration.GetSection(nameof(MyStromProductionConfig)));
    }
}