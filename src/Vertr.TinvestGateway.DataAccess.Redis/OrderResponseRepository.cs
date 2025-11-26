using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts.Orders;
using Vertr.TinvestGateway.Repositories;

namespace Vertr.TinvestGateway.DataAccess.Redis;

internal class OrderResponseRepository : RedisRepositoryBase, IOrderResponseRepository
{
    private static readonly string _responsesKey = "orders.responses";
    private static readonly string _responsesByPortfolioKey = "orders.responses.by-portfolio";
    private static readonly string _responsesAllPortfoliosKey = "orders.responses.all-portfolios";

    public OrderResponseRepository(IConnectionMultiplexer connectionMultiplexer) : base(connectionMultiplexer)
    {
    }

    public async Task Save(PostOrderResponse orderResponse, Guid portfolioId)
    {
        var responseEntry = new HashEntry(orderResponse.OrderId.ToString(), orderResponse.ToJson());
        var portfolioKey = GetPortfolioResponseKey(portfolioId.ToString());
        var db = GetDatabase();

        await Task.WhenAll(
            db.HashSetAsync(_responsesKey, [responseEntry]),
            db.ListRightPushAsync(portfolioKey, orderResponse.OrderId.ToString()),
            db.SetAddAsync(_responsesAllPortfoliosKey, portfolioId.ToString()));
    }

    public async Task<PostOrderResponse?> Get(Guid id)
    {
        var entry = await GetDatabase().HashGetAsync(_responsesKey, id.ToString());

        if (entry.IsNullOrEmpty)
        {
            return null;
        }

        var restored = PostOrderResponse.FromJson(entry.ToString());
        return restored;
    }

    public async Task<IEnumerable<PostOrderResponse>> GetByPortfolioId(Guid portfolioId)
    {
        var db = GetDatabase();
        var portfolioKey = GetPortfolioResponseKey(portfolioId.ToString());
        var orderIds = await db.ListRangeAsync(portfolioKey);

        if (orderIds == null || orderIds.Length <= 0)
        {
            return [];
        }

        var res = new List<PostOrderResponse>();

        foreach (var orderId in orderIds)
        {
            var entry = await db.HashGetAsync(_responsesKey, orderId.ToString());

            if (entry.IsNullOrEmpty)
            {
                continue;
            }

            var item = PostOrderResponse.FromJson(entry.ToString());

            if (item == null)
            {
                continue;
            }

            res.Add(item);
        }

        return res;
    }

    public async Task Clear()
    {
        var db = GetDatabase();

        var allPortfolioKeys = db.SetMembers(_responsesAllPortfoliosKey);

        foreach (var key in allPortfolioKeys)
        {
            if (key.IsNullOrEmpty)
            {
                continue;
            }

            var portfolioKey = GetPortfolioResponseKey(key.ToString());
            await db.KeyDeleteAsync(portfolioKey);
        }

        await Task.WhenAll(
            db.KeyDeleteAsync(_responsesKey),
            db.KeyDeleteAsync(_responsesAllPortfoliosKey));
    }

    private static RedisKey GetPortfolioResponseKey(string portfolioId) => new($"{_responsesByPortfolioKey}.{portfolioId}");
}
