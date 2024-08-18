using System.Globalization;
using common;

namespace energymeasures.Helpers;

public static class Mappers
{
    internal static PowerMeasureRead ToCosmosPowerMeasure(this PowerMeasure measure) =>
        new()
        {
            Sampling = measure.Sampling,
            ConsumedEnergyTotal = measure.ConsumedEnergyTotal,
            ConsumedHighTarif = measure.ConsumedHighTarif,
            ConsumedLowTarif = measure.ConsumedLowTarif,
            InjectedEnergyTotal = measure.InjectedEnergyTotal,
            InjectedEnergyLowTarif = measure.InjectedEnergyLowTarif,
            InjectedEnergyHighTarif = measure.InjectedEnergyHighTarif,
            LiveCurrentL1 = measure.LiveCurrentL1,
            LiveCurrentL2 = measure.LiveCurrentL2,
            LiveCurrentL3 = measure.LiveCurrentL3,
            Id = Guid.NewGuid().ToString(),
            samplingdate = measure.Sampling.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
        };
}
