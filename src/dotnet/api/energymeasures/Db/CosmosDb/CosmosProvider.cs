using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;

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