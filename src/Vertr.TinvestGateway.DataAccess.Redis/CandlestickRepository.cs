using StackExchange.Redis;
using System.Diagnostics;
using Vertr.TinvestGateway.Contracts.MarketData;

namespace Vertr.TinvestGateway.DataAccess.Redis;

public class CandlestickRepository
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    private static readonly string _prefixKey = "market.candles";

    public CandlestickRepository(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<long> Save(string ticker, Candlestick[] candles, int maxCount = 0)
    {
        if (maxCount > 0)
        {
            return await SaveWithOverride(ticker, candles, maxCount);
        }

        var key = GetKey(ticker);
        var db = _connectionMultiplexer.GetDatabase();

        var entries = candles.Select(c => new SortedSetEntry(c.ToJson(), c.Time)).ToArray();
        var added = await db.SortedSetAddAsync(key, entries);

        return added;
    }

    public async Task<IEnumerable<Candlestick?>> GetLast(string ticker, long maxItems = -1)
    {
        var key = GetKey(ticker);
        var db = _connectionMultiplexer.GetDatabase();

        var items = await db.SortedSetRangeByRankAsync(key, 0, maxItems, Order.Descending);
        return items.Select(c => Candlestick.FromJson(c.ToString()));
    }

    internal async Task<long> RemoveLast(string ticker, long stopIndex)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var key = GetKey(ticker);

        var removed = await db.SortedSetRemoveRangeByRankAsync(key, 0, stopIndex);
        return removed;
    }

    internal async Task<long> SaveWithOverride(string ticker, Candlestick[] candles, int maxCount)
    {
        Debug.Assert(maxCount > 0);

        var key = GetKey(ticker);
        var db = _connectionMultiplexer.GetDatabase();

        var candlesTrimmed = candles.OrderBy(c => c.Time).TakeLast(maxCount).ToArray();
        var currentItemsCount = await db.SortedSetLengthAsync(key);
        var toRemove = (currentItemsCount + candlesTrimmed.Length) - maxCount;

        if (toRemove > 0)
        {
            await RemoveLast(ticker, toRemove - 1);
        }

        var entries = candlesTrimmed.Select(c => new SortedSetEntry(c.ToJson(), c.Time)).ToArray();
        var added = await db.SortedSetAddAsync(key, entries);

        return added;
    }

    private static RedisKey GetKey(string ticker) => new RedisKey($"{_prefixKey}.{ticker}");
}
