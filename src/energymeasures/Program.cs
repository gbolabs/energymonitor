using ingress_function;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

const string MyAllowSpecificOrigins = "development";

builder.Services.AddCosmos<CosmosDbContext>(builder.Configuration["pr114energymeasures"], builder.Configuration["CosmosDbName"]);
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins("https://stopr114emp001.z1.web.core.windows.net",
                              "https://energy.isago.ch",
                                                "http://localhost:4200",
                                              "http://localhost");
                      });
});

var app = builder.Build();
app.UseCors();

// Configure the HTTP request pipeline.

app.MapGet("/", () => "Hello CORS!");

app.MapGet("/api/v1/measures/last", (CosmosDbContext dbContext, int? minutes) =>
{
    var minutesSafe = minutes ?? 30;

    var records = dbContext.PowerMeasures.OrderByDescending(p => p._ts).Take(minutesSafe).ToArray();
    var lastRecord = records.First();
    var fromTime = lastRecord.Sampling.Subtract(TimeSpan.FromMinutes(minutesSafe));
    // 11/26/2021 23:49:56
    var fromTimeString = fromTime.ToString("MM/dd/yyyy HH:mm:ss");

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
        Duration = deltaTime,
        InHigh = deltaInHigh,
        InLow = deltaInLow,
        Out = deltaOut,
    });
}).RequireCors(MyAllowSpecificOrigins);

app.MapPost("/api/v1/measures/mystrom/upload", async (HttpRequest request) =>
{
    using (var reader = new StreamReader(request.Body, System.Text.Encoding.UTF8))
    {

        // Read the raw file as a `string`.
        string fileContent = await reader.ReadToEndAsync();
        var lines = fileContent.Split(Environment.NewLine);
        var mystrom = new List<MyStromMeasure>();
        List<string> skippedLines = new();
        List<string> invalidEnergy = new();
        List<string> invalidPower = new();
        List<string> invalidDateTime = new();
        List<string> failedLines = new();

        foreach (var l in lines)
        {
            if (l.Trim().StartsWith("device")) continue;

            var line = l.Trim().Split(',');

            if (line.Length < 6)
            {
                skippedLines.Add(l);
                failedLines.Add(l);
                continue;
            }
            if (!decimal.TryParse(line[4], out var energy))
            {
                invalidEnergy.Add(line[4]);
                failedLines.Add(l);
                continue;
            }
            if (!decimal.TryParse(line[3], out var power))
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


            mystrom.Add(new MyStromMeasure
            (
                dt, (uint)(energy * 100), (uint)(power * 100)
            ));
        }

        // Do something with `fileContent`...

        return Results.Ok(new
        {
            Result = new
            {
                RawLines = lines.Length,
                Skipped = new
                {
                    Count = skippedLines.Count,
                    Samples = skippedLines.Count > 0 ? skippedLines.Take(3).Concat(new List<string> { "..." }).Concat(skippedLines.TakeLast(3)) : Enumerable.Empty<string>()
                },
                InvalidDateTime = new
                {
                    Count = invalidDateTime.Count,
                    Samples = invalidDateTime.Count > 0 ? invalidDateTime.Take(3).Concat(new List<string> { "..." }).Concat(invalidDateTime.TakeLast(3)) : Enumerable.Empty<string>()
                },
                InvalidEnergy = new
                {
                    Count = invalidEnergy.Count,
                    Samples = invalidEnergy.Count > 0 ? invalidEnergy.Take(3).Concat(new List<string> { "..." }).Concat(invalidEnergy.TakeLast(3)) : Enumerable.Empty<string>()
                },
                InvalidPower = new
                {
                    Count = invalidPower.Count,
                    Samples = invalidPower.Count > 0 ? invalidPower.Take(3).Concat(new List<string> { "..." }).Concat(invalidPower.TakeLast(3)) : Enumerable.Empty<string>()
                },
                FailedLines = failedLines.Count > 0 ? failedLines.Take(3).Concat(new List<string> { "..." }).Concat(failedLines.TakeLast(3)) : Enumerable.Empty<string>(),
                ImportedMeasure = mystrom.Count,
                Measures = mystrom
            }
        }); ;
    }
}).Accepts<IFormFile>("text/csv");
//.Produces(200);

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
internal record RawMeasures(Guid Id, DateTime Sampling, decimal ConsumedHighTarif,
    decimal ConsumedLowTarif, decimal LiveCurrentL1,
    decimal LiveCurrentL2, decimal LiveCurrentL3,
    decimal? InjectedEnergyTotal)
{
}

internal record MyStromMeasure(DateTime Sampling, uint Power, uint Energy)
{

}

[Table("Objects")]
internal record Object(string Id, DateTime Sampling, string CounterId, string ObjectId, uint Value)
{

}

[Table("LastMeasureByCounter")]
internal record LastMeasure(string ObjectId, string CounterId, string MeasureId)
{

}

[Table("Counters")]
internal record CounterDefinition(string Id, string Name, string Description, string Unit,
    bool IsCumulative, bool IsAbsolute, byte exposant)
{

}

[Table("Measures")]
internal record Measure(string Id, DateTime SamplingDate, string CounterId, string ObjectId, uint Value)
{

}

class CosmosDbContext : DbContext
{
    public CosmosDbContext(DbContextOptions<CosmosDbContext> dbContextOptions) : base(dbContextOptions)
    {

    }

    public DbSet<PowerMeasureRead> PowerMeasures { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PowerMeasureRead>()
            .HasNoDiscriminator()
            .ToContainer("RawMeasures")
            .HasPartitionKey(pm => pm.samplingdate)
            .HasKey(pm => pm.Id);
    }
}

class PowerMeasureRead : PowerMeasure
{
    public string samplingdate { get; set; } // 11/26/2021 23:49:56
    public string Id { get; set; }

    public ulong _ts { get; set; }
}