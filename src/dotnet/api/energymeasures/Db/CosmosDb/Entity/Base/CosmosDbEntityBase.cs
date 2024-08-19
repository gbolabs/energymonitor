namespace energymeasures;

internal abstract class CosmosDbEntityBase
{
    public string Id { get; set; }
    public abstract string PartitionKey { get; set; }
}