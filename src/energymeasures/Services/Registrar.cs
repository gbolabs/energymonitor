using energymeasures.Config;
using Microsoft.EntityFrameworkCore;

namespace energymeasures.Services;

public static class Registrar
{
    public static void AddLogging(this WebApplicationBuilder builder)
    {
        builder.Services.AddLogging(b =>
        {
            var appInsightsKey = builder.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];
            if (builder.Environment.EnvironmentName == "Development")
                b.AddConsole();
            else if (!string.IsNullOrEmpty(appInsightsKey))
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
        var cs = builder.Configuration["pr114energymeasures"];
        var dbName = builder.Configuration["CosmosDbName"];

        builder.Services.AddCosmos<CosmosDbContext>(cs, dbName);
        builder.Services.AddCosmos<SolarProductionCosmosDbContext>(cs,
            "solar-production");

        builder.Services.AddSingleton(new CosmosProvider(cs, dbName));
        builder.Services.AddSingleton(new CosmosSolarProductionProvider(cs));

        builder.Services.AddDbContext<MyDatabaseContext>(options =>
            options.UseSqlServer(builder.Configuration["AZURE_SQL_CONNECTIONSTRING"]));
    }

    public static void AddConfiguration(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddEnvironmentVariables();
        builder.Configuration.AddUserSecrets<Program>();
        builder.Services.Configure<MyStromProductionConfig>(
            builder.Configuration.GetSection(nameof(MyStromProductionConfig)));
    }
}
