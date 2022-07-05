using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

app.MapGet("/api/v1/measures", () =>
{
    return new RawMeasures
    (
        Guid.NewGuid(),
        DateTime.Now,
        10,
        20,
        1,
        2.3m,
        2.1m,
        -10
    );
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

internal record Object(string Id, DateTime Sampling, string CounterId, string ObjectId, uint Value)
{

}

internal record LastMeasure(string ObjectId, string CounterId, string MeasureId)
{

}

internal record CounterDefinition(string Id, string Name, string Description, string Unit,
    bool IsCumulative, bool IsAbsolute, byte exposant)
{

}
internal record Measure(string Id, DateTime SamplingDate, string CounterId, string ObjectId, uint Value)
{

}