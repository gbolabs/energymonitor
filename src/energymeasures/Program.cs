using ingress_function;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using Container = Microsoft.Azure.Cosmos.Container;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

const string MyAllowSpecificOrigins = "development";

builder.Services.AddCosmos<CosmosDbContext>(builder.Configuration["pr114energymeasures"], builder.Configuration["CosmosDbName"]);
builder.Services.AddSingleton(new CosmosProvider(
    builder.Configuration["pr114energymeasures"],
    builder.Configuration["CosmosDbName"]));

builder.Services.AddTransient<MeasureProvider>();

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

}).RequireCors(MyAllowSpecificOrigins)
.Produces(200, contentType: "application/json");

app.MapGet("/api/v1/measures/date/{date}", async (MeasureProvider provider, DateOnly date) =>
{
    var fromDate = date.ToDateTime(TimeOnly.MinValue);
    var toDate = date.AddDays(1).ToDateTime(TimeOnly.MinValue);
    var data = await provider.GetMeasuresDayRangeAsync(fromDate, toDate);
    return data == null ? Results.NoContent() : Results.Ok(data);

}).RequireCors(MyAllowSpecificOrigins).Produces(200, contentType: "application/json");

app.MapGet("/api/v1/measures/days/last/{days}", async (MeasureProvider provider, int days) =>
{
    var now = DateTime.Now;
    var startDay = DateOnly.FromDateTime(now.Subtract(TimeSpan.FromDays(days))).ToDateTime(TimeOnly.MinValue);
    var stopDay = now;
    var data = await provider.GetMeasuresDayRangeAsync(startDay, stopDay);
    return data == null ? Results.NoContent() : Results.Ok(data);

}).RequireCors(MyAllowSpecificOrigins).Produces(200, contentType: "application/json");

app.MapGet("/api/v1/measures/last", (MeasureProvider provider, int? minutes) =>
{
    //var data = dbContext.GetMeasures(minutes ?? 30);
    var data = provider.GetMeasures(minutes ?? 30);
    return data == null ? Results.NoContent() : Results.Ok(data);
}).RequireCors(MyAllowSpecificOrigins);

app.MapPost("/api/v2/measures/mystrom/upload/{objectId}", (string objectId, [FromBody] MyStromReport report) =>
{
    return Results.Ok(new
    {
        objectId,
        report
    });
});

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

internal class CosmosProvider
{
    public CosmosProvider(string connectionString, string cosmosDbContainer)
    {
        ConnectionString = connectionString;
        CosmosDb = cosmosDbContainer;
        CosmosDbContainer = "RawMeasures";


        CosmosSerializationOptions serializerOptions = new()
        {
            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
        };
        Client = new CosmosClientBuilder(ConnectionString)
            .WithSerializerOptions(serializerOptions)
            .Build();
    }
    public string ConnectionString { get; set; }
    public string CosmosDb { get; private set; }
    public string CosmosDbContainer { get; set; }
    public CosmosClient Client { get; }

    public async Task<Database> GetDatabase()
    {
        return await Client.CreateDatabaseIfNotExistsAsync(CosmosDb);
    }

    public async Task<Container> GetContainerAsync()
    {
        return (await GetDatabase()).GetContainer(CosmosDbContainer);
    }
}

internal class MeasureProvider
{
    private readonly CosmosDbContext _cosmosDbContext;
    private readonly CosmosProvider _cosmosProvider;

    public MeasureProvider(CosmosDbContext cosmosDbContext, CosmosProvider cosmosProvider)
    {
        _cosmosDbContext = cosmosDbContext;
        _cosmosProvider = cosmosProvider;
    }

    public object? GetMeasures(int minutes)
    {

        var minutesSafe = minutes;

        var records = _cosmosDbContext.PowerMeasures.OrderByDescending(p => p._ts).Take(minutesSafe).ToArray();
        var lastRecord = records.First();
        var fromTime = lastRecord.Sampling.Subtract(TimeSpan.FromMinutes(minutesSafe));

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
            return default(object);

        var deltaTime = lastRecord.Sampling - oldest.Sampling;
        var deltaInHigh = lastRecord.ConsumedHighTarif - oldest.ConsumedHighTarif;
        var deltaInLow = lastRecord.ConsumedLowTarif - oldest.ConsumedLowTarif;
        var deltaOut = lastRecord.InjectedEnergyTotal - oldest.InjectedEnergyTotal;

        return new
        {
            From = oldest.Sampling,
            To = lastRecord.Sampling,
            Duration = deltaTime,
            InHigh = deltaInHigh,
            InLow = deltaInLow,
            Out = deltaOut,
        };
    }

    public async Task<object?> GetMeasuresDayRangeAsync(DateTime startDay, DateTime stopDay)
    {
        try
        {
            var container = (await _cosmosProvider.GetContainerAsync());

            var queryStart = "SELECT * FROM RawMeasures c WHERE " +
                //"c.Sampling>= \"2022-09-01T00:00:00.000000\" AND " +
                //"c.Sampling < \"2022-09-02T00:00:00.000000\" ORDER BY c.Sampling DESC";
                $"c.Sampling>= \"{startDay.ToString("s")}\" AND " +
                $"c.Sampling < \"{startDay.AddMinutes(3).ToString("s")}\" ORDER BY c.Sampling DESC";


            var queryStop = "SELECT * FROM RawMeasures c WHERE " +
                //"c.Sampling>= \"2022-09-01T00:00:00.000000\" AND " +
                //"c.Sampling < \"2022-09-02T00:00:00.000000\" ORDER BY c.Sampling DESC";
                $"c.Sampling < \"{stopDay.ToString("s")}\" AND " +
                $"c.Sampling > \"{stopDay.AddMinutes(-3).ToString("s")}\" ORDER BY c.Sampling DESC";


            using FeedIterator<RawMeasures> startFeed = container.GetItemQueryIterator<RawMeasures>(queryStart);
            using FeedIterator<RawMeasures> stopFeed = container.GetItemQueryIterator<RawMeasures>(queryStop);
            if (!startFeed.HasMoreResults || !stopFeed.HasMoreResults)
                return null;


            var oldestRecord = (await startFeed.ReadNextAsync()).FirstOrDefault();
            var newestRecord = (await stopFeed.ReadNextAsync()).LastOrDefault();

            if (oldestRecord != null && newestRecord != null)
            {
                var deltaTime = newestRecord.Sampling - oldestRecord.Sampling;
                var deltaInHigh = newestRecord.ConsumedHighTarif - oldestRecord.ConsumedHighTarif;
                var deltaInLow = newestRecord.ConsumedLowTarif - oldestRecord.ConsumedLowTarif;
                var deltaOut = newestRecord.InjectedEnergyTotal - oldestRecord.InjectedEnergyTotal;

                return new
                {
                    From = oldestRecord.Sampling,
                    To = newestRecord.Sampling,
                    Duration = deltaTime,
                    InHigh = deltaInHigh,
                    InLow = deltaInLow,
                    Out = deltaOut,
                };
            }
            return null;
        }
        catch (CosmosException e)
        {
            return e;
        }
        catch (NotSupportedException e)
        {
            return e;
        }
    }
}

internal record MyStromReport(decimal Power, decimal Ws, bool Relay, decimal Temperature)
{

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
internal record Measure(string Id, DateTime Sampling, string CounterId, string ObjectId, uint Value)
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