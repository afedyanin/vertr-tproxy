using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts.Portfolio;
using Vertr.TinvestGateway.Repositories;

namespace Vertr.TinvestGateway.DataAccess.Redis;

internal class PortfolioRepository : RedisRepositoryBase, IPortfolioRepository
{
    private static readonly string _portfoliosKey = "portfolios";
    private static readonly string _orderToPortfolioKey = "portfolios.orders";

    public PortfolioRepository(IConnectionMultiplexer connectionMultiplexer) : base(connectionMultiplexer)
    {
    }

    public async Task Save(Portfolio portfolio)
    {
        var db = GetDatabase();
        var portfolioEntry = new HashEntry(portfolio.Id.ToString(), portfolio.ToJson());
        await db.HashSetAsync(_portfoliosKey, [portfolioEntry]);
    }

    public async Task<Portfolio?> GetById(Guid portfolioId)
    {
        var entry = await GetDatabase().HashGetAsync(_portfoliosKey, portfolioId.ToString());

        if (entry.IsNullOrEmpty)
        {
            return null;
        }

        var restored = Portfolio.FromJson(entry.ToString());
        return restored;
    }

    public async Task BindOrderToPortfolio(string orderId, Guid portfolioId)
    {
        var entry = new HashEntry(orderId, portfolioId.ToString());
        await GetDatabase().HashSetAsync(_orderToPortfolioKey, [entry]);
    }

    public async Task<Guid?> GetPortfolioByOrderId(string orderId)
    {
        var entry = await GetDatabase().HashGetAsync(_orderToPortfolioKey, orderId);

        if (entry.IsNullOrEmpty)
        {
            return null;
        }

        return new Guid(entry.ToString());
    }
}
