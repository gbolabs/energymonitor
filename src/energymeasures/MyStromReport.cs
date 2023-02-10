namespace energymeasures;

internal record MyStromReport(DateTime Sampling, decimal Power, decimal Ws, bool Relay, decimal Temperature)
{
}