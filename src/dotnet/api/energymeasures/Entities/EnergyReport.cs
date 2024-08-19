namespace energymeasures.Db.CosmosDb;

public class EnergyReport
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public TimeSpan Duration => To - From;

    /// <summary>
    /// Compute the Average Power In by adding the InHigh and InLow and multiplying by the duration in hours.
    /// </summary>
    public decimal AveragePowerIn => (InHigh + InLow) * (decimal)Duration.TotalHours;
    public decimal AveragePowerOut => (Out ?? 0) * (decimal)Duration.TotalHours;
    public decimal InHigh { get; set; }
    public decimal InLow { get; set; }
    public decimal? Out { get; set; }
    public decimal AverageCurrentL1 { get; set; }
    public decimal AverageCurrentL2 { get; set; }
    public decimal AverageCurrentL3 { get; set; }
}