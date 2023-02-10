using Microsoft.Azure.Cosmos;

namespace energymeasures;

internal class MeasureProvider
{
    private readonly CosmosDbContext _cosmosDbContext;
    private readonly CosmosProvider _cosmosProvider;

    public MeasureProvider(CosmosDbContext cosmosDbContext, CosmosProvider cosmosProvider)
    {
        _cosmosDbContext = cosmosDbContext;
        _cosmosProvider = cosmosProvider;
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

    public async Task<object?> GetMeasuresDayRangeAsync(DateTime startDay, DateTime stopDay)
    {
        try
        {
            var container = (await _cosmosProvider.GetContainerAsync());

            var queryStart = "SELECT * FROM RawMeasures c WHERE " +
                             //"c.Sampling>= \"2022-09-01T00:00:00.000000\" AND " +
                             //"c.Sampling < \"2022-09-02T00:00:00.000000\" ORDER BY c.Sampling DESC";
                             $"c.Sampling>= \"{startDay.ToString("s")}\" AND " +
                             $"c.Sampling < \"{startDay.AddMinutes(3).ToString("s")}\" ORDER BY c.Sampling DESC";


            var queryStop = "SELECT * FROM RawMeasures c WHERE " +
                            //"c.Sampling>= \"2022-09-01T00:00:00.000000\" AND " +
                            //"c.Sampling < \"2022-09-02T00:00:00.000000\" ORDER BY c.Sampling DESC";
                            $"c.Sampling < \"{stopDay.ToString("s")}\" AND " +
                            $"c.Sampling > \"{stopDay.AddMinutes(-3).ToString("s")}\" ORDER BY c.Sampling DESC";


            using FeedIterator<RawMeasures> startFeed = container.GetItemQueryIterator<RawMeasures>(queryStart);
            using FeedIterator<RawMeasures> stopFeed = container.GetItemQueryIterator<RawMeasures>(queryStop);
            if (!startFeed.HasMoreResults || !stopFeed.HasMoreResults)
                return null;


            var oldestRecord = (await startFeed.ReadNextAsync()).FirstOrDefault();
            var newestRecord = (await stopFeed.ReadNextAsync()).LastOrDefault();

            if (oldestRecord != null && newestRecord != null)
            {
                var deltaTime = newestRecord.Sampling - oldestRecord.Sampling;
                var deltaInHigh = newestRecord.ConsumedHighTarif - oldestRecord.ConsumedHighTarif;
                var deltaInLow = newestRecord.ConsumedLowTarif - oldestRecord.ConsumedLowTarif;
                var deltaOut = newestRecord.InjectedEnergyTotal - oldestRecord.InjectedEnergyTotal;

                return new
                {
                    From = oldestRecord.Sampling,
                    To = newestRecord.Sampling,
                    Duration = deltaTime,
                    InHigh = deltaInHigh,
                    InLow = deltaInLow,
                    Out = deltaOut,
                };
            }

            return null;
        }
        catch (CosmosException e)
        {
            return e;
        }
        catch (NotSupportedException e)
        {
            return e;
        }
    }
}