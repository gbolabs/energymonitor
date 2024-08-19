using energymeasures.Config;
using Microsoft.Extensions.Options;

namespace energymeasures.Api;

public static class ProductionApi
{
    internal static void RegisterProductionApis(this WebApplication app)
    {
        app.MapGet("/api/v3/production/solar/last", (
            ILogger<LastProductionResponse> logger,
            SolarProductionCosmosDbContext solarProductionCosmosDbContext) =>
        {
            return solarProductionCosmosDbContext.LastProductions.OrderByDescending(p => p.Sampling)
                .Select(r => new LastProductionResponse()
                {
                    Duration = r.Duration,
                    Sampling = r.Sampling,
                    CurrentPowerW = r.CurrentPower,
                    ProductionTotalKwh = r.ProductionKwhSinceLastSampling,
                }).FirstOrDefault();
        }).Produces<LastProductionResponse>();

        app.MapGet("/api/v3/production/solar/range/{from}/{to}", (
            ILogger<LastProductionResponse> logger,
            SolarProductionCosmosDbContext solarProductionCosmosDbContext,
            DateTime from, DateTime to) =>
        {
            var data = solarProductionCosmosDbContext.LastProductions
                .Where(p => p.Sampling >= from && p.Sampling <= to)
                .OrderBy(p => p.Sampling)
                .Select(r => new LastProductionResponse()
                {
                    Duration = r.Duration,
                    Sampling = r.Sampling,
                    CurrentPowerW = r.CurrentPower,
                    ProductionTotalKwh = r.ProductionKwhSinceLastSampling,
                }).ToArray();

            return data;
        }).Produces<LastProductionResponse[]>();

        app.MapGet("/api/v3/production/solar/day/{offset}", (
            ILogger<DailyProductionReport> logger,
            SolarProductionCosmosDbContext solarProductionCosmosDbContext, int? offset) =>
        {
            var day = DateOnly.FromDateTime(DateTime.Now).AddDays(offset ?? 0);
            var data = solarProductionCosmosDbContext.Daily.FirstOrDefault(d => d.Date == day);

            if (data == null)
                return Results.NoContent();

            var duration = data.LastSampling - data.StartOfSun;
            return Results.Ok(new DailyProductionReport
            {
                Date = day.ToDateTime(TimeOnly.MinValue),
                ProductionKwh = Math.Round(data.AverageProduction * (decimal)duration.TotalSeconds / 3600000, 2),
                Duration = duration,
                LastSampling = data.LastSampling
            });
        }).Produces<DailyProductionReport>();

        app.MapPost("/api/v3/production/solar/mystrom/", (MyStromReport report,
                ILogger<MyStromReport> logger,
                IOptions<MyStromProductionConfig> config,
                IWebHostEnvironment env,
                HttpContext context,
                SolarProductionCosmosDbContext solarProductionCosmosDbContext) =>
            {
                if (report.Ws == 0 || report.Power == 0)
                    return Results.NoContent();

                if (config == null && env.EnvironmentName != "Development")
                {
                    logger.LogCritical("No config found and not in DEV Environment");
                    throw new ApplicationException("No config found and not in DEV Environment");
                }

                var apiKeyHeader = context.Request.Headers["X-Api-Key"].FirstOrDefault();
                // Check for the Api-key in case the environment is not development
                if (!string.Equals(config.Value.UploadKey, apiKeyHeader, StringComparison.OrdinalIgnoreCase)
                    && env.EnvironmentName != "Development")
                {
                    // No API Key or wrong API Key
                    logger.LogCritical("No API Key or wrong API Key");
                    throw new ApplicationException("No API Key or wrong API Key");
                }

                var lastRecordSampling = default(DateTime?);

                foreach (var production in solarProductionCosmosDbContext.LastProductions)
                {
                    lastRecordSampling ??= production.Sampling;

                    solarProductionCosmosDbContext.LastProductions.Remove(production);
                }

                solarProductionCosmosDbContext.LastProductions.Add(new LastProduction
                {
                    Id = report.Sampling.Ticks.ToString(),
                    Sampling = report.Sampling,
                    ProductionAverage = report.Ws,
                    Duration = report.Sampling - (lastRecordSampling ?? report.Sampling),
                    CurrentPower = report.Power,
                    Temperature = report.Temperature
                });


                var dailyAverage = solarProductionCosmosDbContext.Daily
                    .FirstOrDefault(d => d.Date == DateOnly.FromDateTime(report.Sampling.Date));

                if (dailyAverage == null)
                {
                    // Init new day
                    solarProductionCosmosDbContext.Daily.Add(new DailyAverageProduction
                    {
                        Id = DateOnly.FromDateTime(report.Sampling.Date).DayNumber.ToString(),
                        Date = DateOnly.FromDateTime(report.Sampling.Date),
                        TotalEnergy = report.Ws,
                        StartOfSun = report.Sampling,
                        LastSampling = report.Sampling,
                        Records = 1
                    });
                }
                else
                {
                    dailyAverage.TotalEnergy += report.Ws;
                    dailyAverage.LastSampling = report.Sampling;
                    dailyAverage.Records += 1;
                    solarProductionCosmosDbContext.Daily.Update(dailyAverage);
                }

                try
                {
                    solarProductionCosmosDbContext.SaveChanges();
                }
                catch (Exception ce)
                {
                    logger.LogError(ce, "Error saving to CosmosDB");
                }

                return Results.Ok(new
                {
                    report
                });
            })
            .Accepts<MyStromReport>(contentType: "application/json")
            .Produces<DailyProductionReport>();
    }
}