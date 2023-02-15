public class LastProductionResponse
{
    public TimeSpan? Duration { get; set; }
    public DateTime Sampling { get; set; }
    public decimal CurrentPowerW { get; set; }
    public decimal ProductionTotalKwh { get; set; }
}