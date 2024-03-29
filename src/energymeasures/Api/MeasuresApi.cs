using System.Globalization;
using energymeasures.Config;
using energymeasures.Db.CosmosDb;
using Microsoft.Extensions.Options;

namespace energymeasures.Api;

public static class MeasuresApi
{
    internal static void RegisterMeasuresApis(this WebApplication app)
    {
        app.MapGet("/api/v1/measures/today", (CosmosDbContext dbContext) =>
        {
            var minutesSafe = 800;

            var records = dbContext.PowerMeasures.OrderByDescending(p => p._ts).Take(minutesSafe).ToArray();
            var lastRecord = records.First();
            var fromTime = DateTime.Today;


            var oldest = default(PowerMeasureRead);

            foreach (var item in records)
            {
                if (item.Sampling < fromTime)
                {
                    break;
                }

                oldest = item;
            }

            if (oldest == null)
                return Results.NoContent();

            var deltaTime = lastRecord.Sampling - oldest.Sampling;
            var deltaInHigh = lastRecord.ConsumedHighTarif - oldest.ConsumedHighTarif;
            var deltaInLow = lastRecord.ConsumedLowTarif - oldest.ConsumedLowTarif;
            var deltaOut = lastRecord.InjectedEnergyTotal - oldest.InjectedEnergyTotal;

            return Results.Ok(new
            {
                From = oldest.Sampling,
                To = lastRecord.Sampling,
                Duration = deltaTime,
                InHigh = deltaInHigh,
                InLow = deltaInLow,
                Out = deltaOut,
            });
        }).Produces(200, contentType: "application/json");

        app.MapGet("/api/v1/measures/date/{date}", async (MeasureProvider provider, DateOnly date) =>
        {
            // given the date is today, returns the result of today api
            if (date == DateOnly.FromDateTime(DateTime.Now))
                return Results.Redirect("/api/v1/measures/today");

            var fromDate = date.ToDateTime(TimeOnly.MinValue);
            var toDate = date.AddDays(1).ToDateTime(TimeOnly.MinValue);
            var data = await provider.GetMeasuresDayRangeAsync(fromDate, toDate);
            return data == null ? Results.NoContent() : Results.Ok(data);
        }).Produces(200, contentType: "application/json");

        app.MapGet("/api/v1/measures/days/last/{days}", async (MeasureProvider provider, int days) =>
        {
            var now = DateTime.Now;
            var startDay = DateOnly.FromDateTime(now.Subtract(TimeSpan.FromDays(days))).ToDateTime(TimeOnly.MinValue);
            var stopDay = now;
            var data = await provider.GetMeasuresDayRangeAsync(startDay, stopDay);
            return data == null ? Results.NoContent() : Results.Ok(data);
        }).Produces(200, contentType: "application/json");

        app.MapGet("/api/v1/measures/last", (MeasureProvider provider, int? minutes) =>
        {
            //var data = dbContext.GetMeasures(minutes ?? 30);
            var data = provider.GetMeasures(minutes ?? 30);
            return data == null ? Results.NoContent() : Results.Ok(data);
        });

        app.MapGet("/api/v1/measures/range/{date}/{from}/{to}",
            async (MeasureProvider provider, DateOnly date, TimeOnly from, TimeOnly to) =>
            {
                var fromDate = date.ToDateTime(from);
                var toDate = date.ToDateTime(to);
                var data = await provider.GetMeasuresDayRangeAsync(fromDate, toDate);
                return data == null ? Results.NoContent() : Results.Ok(data);
            });

        app.MapGet("/api/v1/measures/range/{date}/{from}/{to}/details",
            async (MeasureProvider provider, DateOnly date, TimeOnly from, TimeOnly to) =>
            {
                var fromDate = date.ToDateTime(from);
                var toDate = date.ToDateTime(to);
                var measures = await provider.GetRange(fromDate, toDate);
                return measures?.Length > 0 ? Results.Ok(measures) : Results.NoContent();
            }).Produces<RawMeasures[]>();

        app.MapGet("/api/v1/measures/range/{date}/{from}/{to}/grouped/{interval}",
            async (MeasureProvider provider, DateOnly date, TimeOnly from, TimeOnly to, TimeSpan interval) =>
            {
                var fromDate = date.ToDateTime(from);
                var toDate = date.ToDateTime(to);
                var measures = await provider.GetRange(fromDate, toDate);

                // group by interval
                var grouped = measures.GroupBy(m => m.Sampling.RoundDown(interval))
                    .Select(g =>
                    {
                        var oldest = g.Last();
                        var newest = g.First();
                        return new EnergyReport
                        {
                            From = oldest.Sampling,
                            To = newest.Sampling,
                            InHigh = newest.ConsumedHighTarif - oldest.ConsumedHighTarif,
                            InLow = newest.ConsumedLowTarif - oldest.ConsumedLowTarif,
                            Out = newest.InjectedEnergyTotal - oldest.InjectedEnergyTotal,
                            AverageCurrentL1 = g.Average(r => r.LiveCurrentL1),
                            AverageCurrentL2 = g.Average(r => r.LiveCurrentL2),
                            AverageCurrentL3 = g.Average(r => r.LiveCurrentL3),
                        };
                    }).ToArray();

                return grouped?.Length > 0 ? Results.Ok(grouped.OrderBy(r => r.From)) : Results.NoContent();
            }).Produces<EnergyReport[]>();

        app.MapGet("/api/v1/measures/summary/days/{day}", async (MeasureProvider provider, int day) =>
            {
                var days = from d in Enumerable.Range(0, day)
                    select new
                    {
                        startDay = DateOnly.FromDateTime(DateTime.Now).AddDays(d * -1).ToDateTime(TimeOnly.MinValue),
                        stopDay = DateOnly.FromDateTime(DateTime.Now).AddDays(d * -1).ToDateTime(TimeOnly.MaxValue)
                    };

                var measures = days.Select(async pair =>
                        await provider.GetMeasuresDayRangeAsync(pair.startDay, pair.stopDay))
                    .Select(r => r.Result)
                    .Where(r => r != null)
                    .ToArray();

                return measures == null ? Results.NoContent() : Results.Ok(measures);
            })
            .Produces(200, contentType: "application/json");

        app.MapPost("/api/v1/measures/mystrom/upload", async (HttpRequest request,
            ILogger<MyStromReport> logger,
            IOptions<MyStromProductionConfig> config,
            IWebHostEnvironment env,
            HttpContext context) =>
        {
            if (config == null && env.EnvironmentName != "Development")
            {
                logger.LogCritical("No config found and not in DEV Environment");
                throw new ApplicationException("No config found and not in DEV Environment");
            }

            var apiKeyHeader = context.Request.Headers["X-Api-Key"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(apiKeyHeader) ||
                !string.Equals(config.Value.UploadKey, apiKeyHeader, StringComparison.OrdinalIgnoreCase)
                && env.EnvironmentName != "Development")
            {
                // No API Key or wrong API Key
                logger.LogCritical("No API Key or wrong API Key");
                throw new ApplicationException("No API Key or wrong API Key");
            }

            using (var reader = new StreamReader(request.Body, System.Text.Encoding.UTF8))
            {
                // Read the raw file as a `string`.
                string fileContent = await reader.ReadToEndAsync();
                var lines = fileContent.Split(Environment.NewLine);
                var mystrom = new List<MyStromMeasure>();
                var orderedList = new SortedList<DateTime, MyStromMeasure>();
                List<string> skippedLines = new();
                List<string> invalidEnergy = new();
                List<string> invalidPower = new();
                List<string> invalidDateTime = new();
                List<string> failedLines = new();

                foreach (var l in lines)
                {
                    if (l.Trim().StartsWith("device")) continue;
                    var line = l.Trim().Split(',');

                    // Line look like: 
                    //    1           2   3        4       5          6     7
                    // device_label,time,mac,power(Watt),energy(Ws) ,cost,temperature
                    //     1          2                    3        4 5 6 7   
                    // Solaire,2022 - 09 - 05 00:00:00,083AF256A96C,0,0,0,14.46

                    if (line.Length < 6)
                    {
                        skippedLines.Add(l);
                        failedLines.Add(l);
                        continue;
                    }

                    if (!decimal.TryParse(line[4], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture,
                            out var energy))
                    {
                        invalidEnergy.Add(line[4]);
                        failedLines.Add(l);
                        continue;
                    }

                    if (!decimal.TryParse(line[3], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture,
                            out var power))
                    {
                        invalidPower.Add(line[3]);
                        failedLines.Add(l);
                        continue;
                    }

                    if (!DateTime.TryParse(line[1], out var dt))
                    {
                        invalidDateTime.Add(line[1]);
                        failedLines.Add(l);
                        continue;
                    }


                    var record = new MyStromMeasure
                    (
                        dt, (uint)(power * 100), (uint)(energy * 100)
                    );

                    mystrom.Add(record);
                    orderedList.Add(record.Sampling, record);
                }

                var objects = new List<Sensor>();
                var counters = new List<CounterDefinition>();

                var energyCounter = new CounterDefinition("12", "energy", "Monitors energy", "W/s", false, true, 0);
                var sensor = new Sensor("1", new List<string> { energyCounter.Id });

                var measures = orderedList.Select(r => new Measure(
                    new Random().Next().ToString(),
                    r.Value.Sampling,
                    energyCounter.Id,
                    sensor.Id,
                    r.Value.Energy));

                var lastMeasure = measures.Last();


                return Results.Ok(new
                {
                    Result = new
                    {
                        RawLines = lines.Length,
                        Skipped = new
                        {
                            Count = skippedLines.Count,
                            Samples = skippedLines.Count > 0
                                ? skippedLines.Take(3).Concat(new List<string> { "..." })
                                    .Concat(skippedLines.TakeLast(3))
                                : Enumerable.Empty<string>()
                        },
                        InvalidDateTime = new
                        {
                            Count = invalidDateTime.Count,
                            Samples = invalidDateTime.Count > 0
                                ? invalidDateTime.Take(3).Concat(new List<string> { "..." })
                                    .Concat(invalidDateTime.TakeLast(3))
                                : Enumerable.Empty<string>()
                        },
                        InvalidEnergy = new
                        {
                            Count = invalidEnergy.Count,
                            Samples = invalidEnergy.Count > 0
                                ? invalidEnergy.Take(3).Concat(new List<string> { "..." })
                                    .Concat(invalidEnergy.TakeLast(3))
                                : Enumerable.Empty<string>()
                        },
                        InvalidPower = new
                        {
                            Count = invalidPower.Count,
                            Samples = invalidPower.Count > 0
                                ? invalidPower.Take(3).Concat(new List<string> { "..." })
                                    .Concat(invalidPower.TakeLast(3))
                                : Enumerable.Empty<string>()
                        },
                        FailedLines = failedLines.Count > 0
                            ? failedLines.Take(3).Concat(new List<string> { "..." }).Concat(failedLines.TakeLast(3))
                            : Enumerable.Empty<string>(),
                        ImportedMeasure = mystrom.Count,
                        Measures = mystrom,
                        SortedList = orderedList
                    }
                });
            }
        }).Accepts<IFormFile>("text/csv");
//.Produces(200);
    }
}