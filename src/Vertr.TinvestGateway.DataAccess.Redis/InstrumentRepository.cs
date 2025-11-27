using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts.MarketData;
using Vertr.TinvestGateway.Repositories;

namespace Vertr.TinvestGateway.DataAccess.Redis;

internal class InstrumentRepository : RedisRepositoryBase, IInstrumentRepository
{
    private static readonly string _instrumentsKey = "market.instruments";

    public InstrumentRepository(IConnectionMultiplexer connectionMultiplexer) : base(connectionMultiplexer)
    {
    }

    public async Task Save(Instrument instrument)
    {
        var instrumentEntry = new HashEntry(instrument.Id.ToString(), instrument.ToJson());
        var symbolEntry = new HashEntry(instrument.Ticker, instrument.Id.ToString());
        await GetDatabase().HashSetAsync(_instrumentsKey, [instrumentEntry]);
    }

    public async Task<IEnumerable<Instrument>> GetAll()
    {
        var entries = await GetDatabase().HashGetAllAsync(_instrumentsKey);
        var res = new List<Instrument>();

        if (entries == null)
        {
            return [];
        }

        foreach (var entry in entries)
        {
            if (entry.Value.IsNullOrEmpty)
            {
                continue;
            }

            var item = Instrument.FromJson(entry.Value.ToString());

            if (item == null)
            {
                continue;
            }

            res.Add(item);
        }

        return res;
    }

    public async Task<Instrument?> Get(Guid id)
    {
        var entry = await GetDatabase().HashGetAsync(_instrumentsKey, id.ToString());

        if (entry.IsNullOrEmpty)
        {
            return null;
        }

        var restored = Instrument.FromJson(entry.ToString());
        return restored;
    }

    public Task Clear()
        => GetDatabase().KeyDeleteAsync(_instrumentsKey);
}
