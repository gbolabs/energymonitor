using Microsoft.EntityFrameworkCore;
using System.Globalization;
using energymeasures;
using energymeasures.Api;
using energymeasures.Config;
using energymeasures.Db.CosmosDb;
using energymeasures.Security;
using energymeasures.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddDatabases();
builder.AddConfiguration();
builder.AddBusinessLogic();
builder.AddLogging();
builder.SetupCors();
builder.SetupApiDocumentation();

var app = builder.Build();
app.SetupApiDocumentation();
app.SetupCors();

// Add the path mappings
app.MapGet("/", () => "Hello!");
app.RegisterMeasuresApis();
app.RegisterProductionApis();

// Execute the application
app.Run();