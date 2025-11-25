using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts.Orders;

namespace Vertr.TinvestGateway.DataAccess.Redis;

internal class OrderRequestRepository : RedisRepositoryBase
{
    private static readonly string _requestsKey = "orders.requests";
    private static readonly string _requestsByPortfolioKey = "orders.requests.by-portfolio";
    private static readonly string _requestsAllPortfoliosKey = "orders.requests.all-portfolios";

    public OrderRequestRepository(IConnectionMultiplexer connectionMultiplexer) : base(connectionMultiplexer)
    {
    }

    public async Task Save(PostOrderRequest orderRequest, Guid portfolioId)
    {
        var db = GetDatabase();
        var requestEntry = new HashEntry(orderRequest.RequestId.ToString(), orderRequest.ToJson());
        var portfolioKey = GetPortfolioRequestKey(portfolioId.ToString());

        await Task.WhenAll(
            db.HashSetAsync(_requestsKey, [requestEntry]),
            db.ListRightPushAsync(portfolioKey, orderRequest.RequestId.ToString()),
            db.SetAddAsync(_requestsAllPortfoliosKey, portfolioId.ToString()));
    }

    public async Task<PostOrderRequest?> Get(Guid id)
    {
        var entry = await GetDatabase().HashGetAsync(_requestsKey, id.ToString());

        if (entry.IsNullOrEmpty)
        {
            return null;
        }

        var restored = PostOrderRequest.FromJson(entry.ToString());
        return restored;
    }

    public async Task<IEnumerable<PostOrderRequest>> GetByPortfolioId(Guid portfolioId)
    {
        var db = GetDatabase();
        var portfolioKey = GetPortfolioRequestKey(portfolioId.ToString());
        var requestIds = await db.ListRangeAsync(portfolioKey);

        if (requestIds == null || requestIds.Length <=0)
        {
            return [];
        }

        var res = new List<PostOrderRequest>();

        foreach (var requestId in requestIds)
        {
            var entry = await db.HashGetAsync(_requestsKey, requestId.ToString());

            if (entry.IsNullOrEmpty)
            { 
                continue;
            }

            var item = PostOrderRequest.FromJson(entry.ToString());

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

        var allPortfolioKeys = db.SetMembers(_requestsAllPortfoliosKey);

        foreach (var key in allPortfolioKeys)
        {
            if (key.IsNullOrEmpty)
            { 
                continue; 
            }

            var portfolioKey = GetPortfolioRequestKey(key.ToString());
            await db.KeyDeleteAsync(portfolioKey);
        }

        await Task.WhenAll(
            db.KeyDeleteAsync(_requestsKey),
            db.KeyDeleteAsync(_requestsAllPortfoliosKey));
    }

    private static RedisKey GetPortfolioRequestKey(string portfolioId) => new($"{_requestsByPortfolioKey}.{portfolioId}");
}
