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

app.MapPost("/api/v1/measures/mystrom/", (String csvText) =>
{
    if(string.IsNullOrWhiteSpace(csvText))
    {
        return Results.NoContent();
    }
    
    return Results.Ok(csvText);
});

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