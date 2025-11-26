using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts.Portfolio;
using Vertr.TinvestGateway.Repositories;

namespace Vertr.TinvestGateway.DataAccess.Redis;

internal class PortfolioRepository : RedisRepositoryBase, IPortfolioRepository
{
    private static readonly string _portfoliosKey = "portfolios";

    public PortfolioRepository(IConnectionMultiplexer connectionMultiplexer) : base(connectionMultiplexer)
    {
    }

    public async Task Save(Portfolio portfolio)
    {
        var db = GetDatabase();
        var portfolioEntry = new HashEntry(portfolio.Id.ToString(), portfolio.ToJson());
        await db.HashSetAsync(_portfoliosKey, [portfolioEntry]);
    }

    public async Task<Portfolio?> Get(Guid id)
    {
        var entry = await GetDatabase().HashGetAsync(_portfoliosKey, id.ToString());

        if (entry.IsNullOrEmpty)
        {
            return null;
        }

        var restored = Portfolio.FromJson(entry.ToString());
        return restored;
    }
}
