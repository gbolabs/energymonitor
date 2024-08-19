namespace energymeasures.Db.CosmosDb;

public static class EnergyReportExtensions
{
    public static DateTime RoundDown(this DateTime dt, TimeSpan d)
    {
        return new DateTime((dt.Ticks / d.Ticks) * d.Ticks);
    }
}