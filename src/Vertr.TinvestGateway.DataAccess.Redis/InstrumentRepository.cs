using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts.MarketData;
using Vertr.TinvestGateway.Contracts.Repositories;

namespace Vertr.TinvestGateway.DataAccess.Redis
{
    internal class InstrumentRepository : RedisRepositoryBase, IInstrumentRepository
    {
        private static readonly string _instrumentsKey = "market.instruments";
        private static readonly string _symbolsKey = "market.symbols";

        public InstrumentRepository(IConnectionMultiplexer connectionMultiplexer) : base(connectionMultiplexer)
        {
        }

        public async Task Save(Instrument instrument)
        {
            var instrumentEntry = new HashEntry(instrument.Id.ToString(), instrument.ToJson());
            var symbolEntry = new HashEntry(instrument.Ticker, instrument.Id.ToString());

            var db = GetDatabase();

            await Task.WhenAll(
                db.HashSetAsync(_instrumentsKey, [instrumentEntry]),
                db.HashSetAsync(_symbolsKey, [symbolEntry]));
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

        public async Task<Instrument?> GetByTicker(string ticker)
        {
            var entry = await GetDatabase().HashGetAsync(_symbolsKey, ticker);

            if (entry.IsNullOrEmpty)
            {
                return null;
            }

            if (!Guid.TryParse(entry.ToString(), out var instrumentId))
            {
                return null;
            }

            return await Get(instrumentId);
        }

        public async Task Clear()
        {
            var db = GetDatabase();

            await Task.WhenAll(
                db.KeyDeleteAsync(_instrumentsKey),
                db.KeyDeleteAsync(_symbolsKey));
        }
    }
}
