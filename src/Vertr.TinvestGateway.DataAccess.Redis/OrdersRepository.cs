using StackExchange.Redis;

namespace Vertr.TinvestGateway.DataAccess.Redis;

internal class OrdersRepository : RedisRepositoryBase
{
    private static readonly string _instrumentsKey = "market.instruments";
    private static readonly string _symbolsKey = "market.symbols";

    public OrdersRepository(IConnectionMultiplexer connectionMultiplexer) : base(connectionMultiplexer)
    {
    }


}
