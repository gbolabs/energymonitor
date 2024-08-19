namespace energymeasures;

internal record RawMeasures(Guid Id, DateTime Sampling, decimal ConsumedHighTarif,
    decimal ConsumedLowTarif, decimal LiveCurrentL1,
    decimal LiveCurrentL2, decimal LiveCurrentL3,
    decimal? InjectedEnergyTotal)
{
}