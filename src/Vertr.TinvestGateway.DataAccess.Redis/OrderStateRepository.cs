using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts.Orders;
using Vertr.TinvestGateway.Repositories;

namespace Vertr.TinvestGateway.DataAccess.Redis;

internal class OrderStateRepository : RedisRepositoryBase, IOrderStateRepository
{
    private static readonly string _statesKey = "orders.states";
    private static readonly string _statesByOrderKey = "orders.states.by-order";
    private static readonly string _statesAllOrdersKey = "orders.states.all-orders";
    private static readonly string _statesByRequestKey = "orders.states.by-request";
    private static readonly string _statesAllRequestsKey = "orders.states.all-requests";

    public OrderStateRepository(IConnectionMultiplexer connectionMultiplexer) : base(connectionMultiplexer)
    {
    }

    public async Task Save(OrderState orderState)
    {
        var db = GetDatabase();
        var stateEntry = new HashEntry(orderState.Id.ToString(), orderState.ToJson());
        await db.HashSetAsync(_statesKey, [stateEntry]);

        if (!string.IsNullOrEmpty(orderState.OrderId))
        {
            var orderKey = GetStatesByOrderKey(orderState.OrderId);
            await db.ListRightPushAsync(orderKey, orderState.Id.ToString());
            await db.SetAddAsync(_statesAllOrdersKey, orderState.OrderId);
        }
        else if (!string.IsNullOrEmpty(orderState.OrderRequestId))
        {
            var requestKey = GetStatesByRequestKey(orderState.OrderRequestId);
            await db.ListRightPushAsync(requestKey, orderState.Id.ToString());
            await db.SetAddAsync(_statesAllRequestsKey, orderState.OrderRequestId);
        }
    }

    public async Task<OrderState?> Get(Guid id)
    {
        var entry = await GetDatabase().HashGetAsync(_statesKey, id.ToString());

        if (entry.IsNullOrEmpty)
        {
            return null;
        }

        var restored = OrderState.FromJson(entry.ToString());
        return restored;
    }

    public async Task<IEnumerable<OrderState>> GetByIds(string? orderId = null, string? requestId = null)
    {
        var db = GetDatabase();

        var stateIds = new List<string>();

        if (!string.IsNullOrEmpty(orderId))
        {
            var orderKey = GetStatesByOrderKey(orderId);
            var ids = await db.ListRangeAsync(orderKey);
            if (ids != null)
            {
                stateIds.AddRange(ids.Select(v => v.ToString()));
            }
        }

        if (!string.IsNullOrEmpty(requestId))
        {
            var requestKey = GetStatesByRequestKey(requestId);
            var ids = await db.ListRangeAsync(requestKey);
            if (ids != null)
            {
                stateIds.AddRange(ids.Select(v => v.ToString()));
            }
        }

        var res = new List<OrderState>();

        foreach (var stateId in stateIds.Distinct())
        {
            var entry = await db.HashGetAsync(_statesKey, stateId);

            if (entry.IsNullOrEmpty)
            {
                continue;
            }

            var item = OrderState.FromJson(entry.ToString());

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

        var allOrdersKeys = db.SetMembers(_statesAllOrdersKey);

        foreach (var key in allOrdersKeys)
        {
            if (key.IsNullOrEmpty)
            {
                continue;
            }

            var orderKey = GetStatesByOrderKey(key.ToString());
            await db.KeyDeleteAsync(orderKey);
        }

        var allRequestsKeys = db.SetMembers(_statesAllRequestsKey);

        foreach (var key in allRequestsKeys)
        {
            if (key.IsNullOrEmpty)
            {
                continue;
            }

            var requestKey = GetStatesByRequestKey(key.ToString());
            await db.KeyDeleteAsync(requestKey);
        }

        await Task.WhenAll(
            db.KeyDeleteAsync(_statesKey),
            db.KeyDeleteAsync(_statesAllOrdersKey),
            db.KeyDeleteAsync(_statesAllRequestsKey));
    }

    private static RedisKey GetStatesByOrderKey(string orderId) => new($"{_statesByOrderKey}.{orderId}");

    private static RedisKey GetStatesByRequestKey(string requestId) => new($"{_statesByRequestKey}.{requestId}");
}
