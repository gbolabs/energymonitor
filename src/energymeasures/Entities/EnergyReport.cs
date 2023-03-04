namespace energymeasures.Db.CosmosDb;

public class EnergyReport
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public TimeSpan Duration => To - From;
    public decimal InHigh { get; set; }
    public decimal InLow { get; set; }
    public decimal? Out { get; set; }
}