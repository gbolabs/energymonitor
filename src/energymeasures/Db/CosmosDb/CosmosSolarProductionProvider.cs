using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;

namespace energymeasures;

internal class CosmosSolarProductionProvider
{
    private readonly string _connectionString;
    private const string _cosmosDb = "solar-production";
    private const string _lastContainer = "last";
    private const string _dailyAverage = "daily-average";
    private readonly CosmosClient _client;

    public CosmosSolarProductionProvider(string connectionString)
    {
        _connectionString = connectionString;
        CosmosSerializationOptions serializerOptions = new()
        {
            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
        };
        _client = new CosmosClientBuilder(_connectionString)
            .WithSerializerOptions(serializerOptions)
            .Build();
    }


    private async Task<Database> GetDatabase()
    {
        return await _client.CreateDatabaseIfNotExistsAsync(_cosmosDb);
    }

    public async Task<Container> GetLastContainerAsync()
    {
        return (await GetDatabase()).GetContainer(_lastContainer);
    }

    public async Task<Container> GetDailyAverageAsync()
    {
        return (await GetDatabase()).GetContainer(_dailyAverage);
    }
}