using System.ComponentModel.DataAnnotations.Schema;

namespace energymeasures;

[Table("Measures")]
internal record Measure(string Id, DateTime Sampling, string CounterId, string SensorId, uint Value)
{
}