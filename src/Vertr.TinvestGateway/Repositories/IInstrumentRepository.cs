using Vertr.TinvestGateway.Contracts.MarketData;

namespace Vertr.TinvestGateway.Repositories;

public interface IInstrumentRepository
{
    public Task Clear();
    public Task<IEnumerable<Instrument>> GetAll();
    public Task<Instrument?> Get(Guid id);
    public Task<Instrument?> GetByTicker(string ticker);
    public Task Save(Instrument instrument);
}
