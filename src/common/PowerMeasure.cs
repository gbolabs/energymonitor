using Newtonsoft.Json;

namespace common
{
    public class PowerMeasure
    {
        public DateTime Sampling { get; set; }
        public decimal ConsumedHighTarif { get; set; }
        public decimal ConsumedLowTarif { get; set; }
        public decimal InjectedEnergyTotal { get; set; }

        public decimal LiveCurrentL1 { get; set; }
        public decimal LiveCurrentL2 { get; set; }
        public decimal LiveCurrentL3 { get; set; }

    }

    public class CosmosDbPowerMeasure : PowerMeasure
    {
        [JsonProperty("samplingdate")]
        public string SamplingDateString => Sampling.ToString();
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }
}
