using StackExchange.Redis;
using System.Diagnostics;
using Vertr.TinvestGateway.Contracts.MarketData;
using Vertr.TinvestGateway.Contracts.Repositories;

namespace Vertr.TinvestGateway.DataAccess.Redis;

internal class CandlestickRepository : RedisRepositoryBase, ICandlestickRepository
{
    private static readonly string _prefixKey = "market.candles";

    public CandlestickRepository(IConnectionMultiplexer connectionMultiplexer) : base(connectionMultiplexer)
    {
    }

    public async Task<long> Save(Guid instrumentId, Candlestick[] candles, int maxCount = 0)
    {
        if (maxCount > 0)
        {
            return await SaveWithOverride(instrumentId, candles, maxCount);
        }

        var entries = candles.Select(c => new SortedSetEntry(c.ToJson(), c.Time)).ToArray();
        var db = GetDatabase();
        var added = await db.SortedSetAddAsync(GetKey(instrumentId), entries);

        return added;
    }

    public async Task<IEnumerable<Candlestick?>> GetLast(Guid instrumentId, long maxItems = -1)
    {
        var items = await GetDatabase().SortedSetRangeByRankAsync(GetKey(instrumentId), 0, maxItems, Order.Descending);
        return items.Select(c => Candlestick.FromJson(c.ToString()));
    }

    public Task<bool> Clear(Guid instrumentId)
        => GetDatabase().KeyDeleteAsync(GetKey(instrumentId));

    internal async Task<long> RemoveLast(Guid instrumentId, long stopIndex)
    {
        var removed = await GetDatabase().SortedSetRemoveRangeByRankAsync(GetKey(instrumentId), 0, stopIndex);
        return removed;
    }

    internal async Task<long> SaveWithOverride(Guid instrumentId, Candlestick[] candles, int maxCount)
    {
        Debug.Assert(maxCount > 0);

        var key = GetKey(instrumentId);
        var db = GetDatabase();

        var candlesTrimmed = candles.OrderBy(c => c.Time).TakeLast(maxCount).ToArray();
        var currentItemsCount = await db.SortedSetLengthAsync(key);
        var toRemove = (currentItemsCount + candlesTrimmed.Length) - maxCount;

        if (toRemove > 0)
        {
            await RemoveLast(instrumentId, toRemove - 1);
        }

        var entries = candlesTrimmed.Select(c => new SortedSetEntry(c.ToJson(), c.Time)).ToArray();
        var added = await db.SortedSetAddAsync(key, entries);

        return added;
    }

    private static RedisKey GetKey(Guid instrumentId) => new($"{_prefixKey}.{instrumentId}");
}
