using System.ComponentModel.DataAnnotations.Schema;

namespace energymeasures;

[Table("Counters")]
internal record CounterDefinition(string Id, string Name, string Description, string Unit,
    bool IsCumulative, bool IsAbsolute, byte exposant)
{
}