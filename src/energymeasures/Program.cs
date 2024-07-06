using energymeasures.Api;
using energymeasures.Security;
using energymeasures.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddDatabases();
builder.AddConfiguration();
builder.AddBusinessLogic();
builder.AddLogging();
builder.SetupCors();

var app = builder.Build();
app.SetupCors();

// Add the path mappings
app.MapGet("/", () => "Hello!");
app.RegisterMeasuresApis();
app.RegisterProductionApis();
app.RegisterEnergyMeterApis();

// Execute the application
app.Run();
