using StackExchange.Redis;

namespace Vertr.TinvestGateway.DataAccess.Redis;

internal abstract class RedisRepositoryBase : IDisposable
{
    private readonly IConnectionMultiplexer _connection;

    protected IDatabase GetDatabase() => _connection.GetDatabase();

    protected RedisRepositoryBase(IConnectionMultiplexer connectionMultiplexer)
    {
        _connection = connectionMultiplexer;
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}
