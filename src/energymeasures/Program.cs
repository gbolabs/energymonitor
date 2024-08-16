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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.SetupCors();

// Add the path mappings
app.RegisterMeasuresApis();
app.RegisterProductionApis();
app.RegisterEnergyMeterApis();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Execute the application
app.Run();
