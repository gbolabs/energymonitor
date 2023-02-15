using System.Globalization;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos.Linq;

namespace energymeasures;

internal class DailyAverageProduction : CosmosDbEntityBase
{
    // The partition key represent an universal format of the DateOnly.
    // The format is YYYY-MM-DD.
    // The format is used to be able to query the data by date.
    public override string PartitionKey
    {
        get => $"{Date.DayNumber}";
        set => Date = DateOnly.FromDayNumber(int.Parse(value));
    }

    public DateOnly Date { get; set; }

    /// <summary>
    /// This property is not serialized to the CosmosDb.
    /// </summary>
    [JsonIgnore]
    public decimal AverageProduction => TotalEnergy / Records;

    public decimal TotalEnergy { get; set; }

    public int Records { get; set; }
    
    public DateTime StartOfSun { get; set; }

    public DateTime LastSampling { get; set; }
}