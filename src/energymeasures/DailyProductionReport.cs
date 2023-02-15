namespace energymeasures;

public class DailyProductionReport
{
    public DateTime Date { get; set; }
    public decimal ProductionKwh { get; set; }
    public TimeSpan? Duration { get; set; }

    public DateTime? LastSampling { get; set; }
}