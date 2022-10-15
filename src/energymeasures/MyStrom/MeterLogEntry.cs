namespace energymeasures.MyStrom
{
    public record MeterLogEntry(string DeviceLabel, DateTime StartPeriodTime, TimeSpan Duration, string Mac, decimal EnergyWs);
}
