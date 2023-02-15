using System.ComponentModel.DataAnnotations.Schema;

namespace energymeasures;

[Table("Sensors")]
internal record Sensor(string Id, List<string> CounterIds)
{
}