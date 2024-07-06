using common;

namespace energymeasures.Helpers;

public static class Mappers
{
    internal static PowerMeasureRead ToCosmosPowerMeasure(this PowerMeasure measure) =>
        new()
        {
            Sampling = measure.Sampling,
            ConsumedHighTarif = measure.ConsumedHighTarif,
            ConsumedLowTarif = measure.ConsumedLowTarif,
            InjectedEnergyTotal = measure.InjectedEnergyTotal,
            LiveCurrentL1 = measure.LiveCurrentL1,
            LiveCurrentL2 = measure.LiveCurrentL2,
            LiveCurrentL3 = measure.LiveCurrentL3
        };
}
