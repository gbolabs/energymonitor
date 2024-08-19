using energymeasures.Api;
using energymeasures.Security;
using energymeasures.Services;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddDatabases();
builder.AddConfiguration();
builder.AddBusinessLogic();
builder.AddLogging();
builder.SetupCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.All;
});

var app = builder.Build();
app.SetupCors();

// Add the path mappings
app.RegisterMeasuresApis();
app.RegisterProductionApis();
app.RegisterEnergyMeterApis();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpLogging();


// Execute the application
app.Run();
