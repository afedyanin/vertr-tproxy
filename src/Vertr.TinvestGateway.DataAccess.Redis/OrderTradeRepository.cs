using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts.Orders;
using Vertr.TinvestGateway.Repositories;

namespace Vertr.TinvestGateway.DataAccess.Redis;

internal class OrderTradeRepository : RedisRepositoryBase, IOrderTradeRepository
{
    private static readonly string _tradesKey = "orders.trades";
    private static readonly string _tradesByOrderKey = "orders.trades.by-order";
    private static readonly string _tradesAllOrdersKey = "orders.trades.all-orders";

    public OrderTradeRepository(IConnectionMultiplexer connectionMultiplexer) : base(connectionMultiplexer)
    {
    }

    public async Task Save(OrderTrades orderTrades)
    {
        var db = GetDatabase();
        var tradesEntry = new HashEntry(orderTrades.Id.ToString(), orderTrades.ToJson());
        var orderKey = GetOrderTradesKey(orderTrades.OrderId);

        await Task.WhenAll(
            db.HashSetAsync(_tradesKey, [tradesEntry]),
            db.ListRightPushAsync(orderKey, orderTrades.Id.ToString()),
            db.SetAddAsync(_tradesAllOrdersKey, orderTrades.OrderId));
    }

    public async Task<OrderTrades?> Get(Guid id)
    {
        var entry = await GetDatabase().HashGetAsync(_tradesKey, id.ToString());

        if (entry.IsNullOrEmpty)
        {
            return null;
        }

        var restored = OrderTrades.FromJson(entry.ToString());
        return restored;
    }

    public async Task<IEnumerable<OrderTrades>> GetByOrderId(string orderId)
    {
        var db = GetDatabase();
        var orderKey = GetOrderTradesKey(orderId);
        var orderTradesIds = await db.ListRangeAsync(orderKey);

        if (orderTradesIds == null || orderTradesIds.Length <= 0)
        {
            return [];
        }

        var res = new List<OrderTrades>();

        foreach (var orderTradeId in orderTradesIds)
        {
            var entry = await db.HashGetAsync(_tradesKey, orderTradeId);

            if (entry.IsNullOrEmpty)
            {
                continue;
            }

            var item = OrderTrades.FromJson(entry.ToString());

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

        var allOrdersKeys = db.SetMembers(_tradesAllOrdersKey);

        foreach (var key in allOrdersKeys)
        {
            if (key.IsNullOrEmpty)
            {
                continue;
            }

            var orderKey = GetOrderTradesKey(key.ToString());
            await db.KeyDeleteAsync(orderKey);
        }

        await Task.WhenAll(
            db.KeyDeleteAsync(_tradesKey),
            db.KeyDeleteAsync(_tradesAllOrdersKey));
    }

    private static RedisKey GetOrderTradesKey(string orderId) => new($"{_tradesByOrderKey}.{orderId}");
}
