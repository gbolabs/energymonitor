using energymeasures.Db.CosmosDb;
using Microsoft.Azure.Cosmos;

namespace energymeasures;

internal class MeasureProvider
{
    private readonly CosmosDbContext _cosmosDbContext;
    private readonly CosmosProvider _cosmosProvider;
    private readonly ILogger<MeasureProvider> _logger;

    public MeasureProvider(CosmosDbContext cosmosDbContext, CosmosProvider cosmosProvider, ILogger<MeasureProvider> logger)
    {
        _cosmosDbContext = cosmosDbContext;
        _cosmosProvider = cosmosProvider;
        _logger = logger;
    }

    public object? GetMeasures(int minutes)
    {
        var minutesSafe = minutes;

        var records = _cosmosDbContext.PowerMeasures.OrderByDescending(p => p._ts).Take(minutesSafe).ToArray();
        var lastRecord = records.First();
        var fromTime = lastRecord.Sampling.Subtract(TimeSpan.FromMinutes(minutesSafe));

        var oldest = default(PowerMeasureRead);

        foreach (var item in records)
        {
            if (item.Sampling < fromTime)
            {
                break;
            }

            oldest = item;
        }

        if (oldest == null)
            return default(object);

        var deltaTime = lastRecord.Sampling - oldest.Sampling;
        var deltaInHigh = lastRecord.ConsumedHighTarif - oldest.ConsumedHighTarif;
        var deltaInLow = lastRecord.ConsumedLowTarif - oldest.ConsumedLowTarif;
        var deltaOut = lastRecord.InjectedEnergyTotal - oldest.InjectedEnergyTotal;

        return new
        {
            From = oldest.Sampling,
            To = lastRecord.Sampling,
            Duration = deltaTime,
            InHigh = deltaInHigh,
            InLow = deltaInLow,
            Out = deltaOut,
        };
    }

    public async Task<EnergyReport> GetMeasuresDateTimeRange(DateTime from, DateTime to)
    {
        try
        {

            var (fromRecord, toRecord) = await GetRangeBoundaries(from, to);

            if (fromRecord == null || toRecord == null)
                return null;

            return new EnergyReport
            {
                From = fromRecord.Sampling,
                To = toRecord.Sampling,
                InHigh = toRecord.ConsumedHighTarif - fromRecord.ConsumedHighTarif,
                InLow = toRecord.ConsumedLowTarif - fromRecord.ConsumedLowTarif,
                Out = toRecord.InjectedEnergyTotal - fromRecord.InjectedEnergyTotal,
            };
        }
        catch (CosmosException e)
        {
            _logger.LogError(e, "CosmosException");
        }
        catch (NotSupportedException e)
        {
            _logger.LogError(e, "NotSupportedException");
        }

        return null;
    }
    
    internal async Task<RawMeasures[]> GetRange(DateTime from, DateTime to)
    {
        var container = (await _cosmosProvider.GetContainerAsync());

        var query = "SELECT * FROM RawMeasures c WHERE " +
                    //"c.Sampling>= \"2022-09-01T00:00:00.000000\" AND " +
                    //"c.Sampling < \"2022-09-02T00:00:00.000000\" ORDER BY c.Sampling DESC";
                    $"c.Sampling>= \"{from.ToString("s")}\" AND " +
                    $"c.Sampling < \"{to.ToString("s")}\" ORDER BY c.Sampling DESC";

        using FeedIterator<RawMeasures> feed = container.GetItemQueryIterator<RawMeasures>(query);
        if (!feed.HasMoreResults)
            return null;
        
        return (await feed.ReadNextAsync()).ToArray();
    }

    private async Task<Tuple<RawMeasures, RawMeasures>> GetRangeBoundaries(DateTime from, DateTime to)
    {
        var container = (await _cosmosProvider.GetContainerAsync());

        var queryStart = "SELECT * FROM RawMeasures c WHERE " +
                         //"c.Sampling>= \"2022-09-01T00:00:00.000000\" AND " +
                         //"c.Sampling < \"2022-09-02T00:00:00.000000\" ORDER BY c.Sampling DESC";
                         $"c.Sampling>= \"{from.ToString("s")}\" AND " +
                         $"c.Sampling < \"{from.AddMinutes(3).ToString("s")}\" ORDER BY c.Sampling DESC";


        var queryStop = "SELECT * FROM RawMeasures c WHERE " +
                        //"c.Sampling>= \"2022-09-01T00:00:00.000000\" AND " +
                        //"c.Sampling < \"2022-09-02T00:00:00.000000\" ORDER BY c.Sampling DESC";
                        $"c.Sampling < \"{to.ToString("s")}\" AND " +
                        $"c.Sampling > \"{to.AddMinutes(-3).ToString("s")}\" ORDER BY c.Sampling DESC";


        using FeedIterator<RawMeasures> startFeed = container.GetItemQueryIterator<RawMeasures>(queryStart);
        using FeedIterator<RawMeasures> stopFeed = container.GetItemQueryIterator<RawMeasures>(queryStop);
        if (!startFeed.HasMoreResults || !stopFeed.HasMoreResults)
            return null;
        
        return new Tuple<RawMeasures, RawMeasures>((await startFeed.ReadNextAsync()).FirstOrDefault(),
            (await stopFeed.ReadNextAsync()).FirstOrDefault());
    }

    public async Task<EnergyReport?> GetMeasuresDayRangeAsync(DateTime startDay, DateTime stopDay)
    {
        return await GetMeasuresDateTimeRange(startDay, stopDay);
    }
}