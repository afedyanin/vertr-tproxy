using Vertr.TinvestGateway.Contracts.MarketData;

namespace Vertr.TinvestGateway.Contracts.Repositories;

public interface ICandlestickRepository
{
    public Task<bool> Clear(string ticker);
    public Task<IEnumerable<Candlestick?>> GetLast(string ticker, long maxItems = -1);
    public Task<long> Save(string ticker, Candlestick[] candles, int maxCount = 0);
}
