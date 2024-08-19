using System.ComponentModel.DataAnnotations.Schema;

namespace energymeasures;

[Table("LastMeasureByCounter")]
internal record LastMeasure(string SensorId, string CounterId, string MeasureId)
{
}