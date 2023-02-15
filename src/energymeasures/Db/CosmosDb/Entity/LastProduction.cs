namespace energymeasures;

internal class LastProduction : CosmosDbEntityBase
{
    public override string PartitionKey
    {
        get => Sampling.Ticks.ToString();
        set => Sampling = new DateTime(long.Parse(value));
    }

    public DateTime Sampling { get; set; }
    public decimal ProductionAverage { get; set; }
    public decimal CurrentPower { get; set; }

    public decimal? Temperature { get; set; }
}