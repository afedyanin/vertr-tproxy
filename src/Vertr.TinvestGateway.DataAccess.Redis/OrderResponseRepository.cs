using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts.Orders;
using Vertr.TinvestGateway.Repositories;

namespace Vertr.TinvestGateway.DataAccess.Redis;

internal class OrderResponseRepository : RedisRepositoryBase, IOrderResponseRepository
{
    private static readonly string _responsesKey = "order.responses";

    public OrderResponseRepository(IConnectionMultiplexer connectionMultiplexer) : base(connectionMultiplexer)
    {
    }

    public async Task Save(PostOrderResponse orderResponse)
    {
        var responseEntry = new HashEntry(orderResponse.OrderId.ToString(), orderResponse.ToJson());
        await GetDatabase().HashSetAsync(_responsesKey, [responseEntry]);
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

    public async Task<IEnumerable<PostOrderResponse?>> Find(string pattern)
    {
        var res = new List<PostOrderResponse>();

        await foreach (var entry in GetDatabase().HashScanAsync(_responsesKey, pattern))
        {
            if (entry.Value.IsNullOrEmpty)
            {
                continue;
            }

            var restored = PostOrderResponse.FromJson(entry.Value.ToString());
            if (restored == null)
            {
                continue;
            }

            res.Add(restored);
        }

        return res;
    }

    public Task Clear() =>
        GetDatabase().KeyDeleteAsync(_responsesKey);
}
