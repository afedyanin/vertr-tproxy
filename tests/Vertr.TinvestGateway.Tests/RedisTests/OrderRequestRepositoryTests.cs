using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts.Orders;
using Vertr.TinvestGateway.Contracts.Orders.Enums;
using Vertr.TinvestGateway.DataAccess.Redis;

namespace Vertr.TinvestGateway.Tests.RedisTests;

public class OrderRequestRepositoryTests
{
    private IConnectionMultiplexer _connectionMultiplexer;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _connectionMultiplexer = ConnectionMultiplexer.Connect("localhost");
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _connectionMultiplexer.Close();
    }

    [TearDown]
    public async Task TearDown()
    {
        var repo = new OrderRequestRepository(_connectionMultiplexer);
        await repo.Clear();
    }

    [Test]
    public async Task CanSaveOrderRequest()
    {
        var repo = new OrderRequestRepository(_connectionMultiplexer);
        var orderRequest = CreateOrderRequest();

        var portfolioId = Guid.NewGuid();
        await repo.Save(orderRequest, portfolioId);
        var saved = await repo.Get(orderRequest.RequestId);
        Assert.That(saved, Is.Not.Null);

        Console.WriteLine(saved);
    }

    [Test]
    public async Task CanGetOrderRequestsByPortfolio()
    {
        var repo = new OrderRequestRepository(_connectionMultiplexer);
        var or1 = CreateOrderRequest();
        var or2 = CreateOrderRequest();

        var portfolioId = Guid.NewGuid();
        await repo.Save(or1, portfolioId);
        await repo.Save(or2, portfolioId);

        var items = await repo.GetByPortfolioId(portfolioId);

        Assert.That(items.Count, Is.EqualTo(2));

        foreach (var item in items)
        {
            Console.WriteLine(item);
        }
    }

    private static PostOrderRequest CreateOrderRequest()
        => new PostOrderRequest
        {
             AccountId = Guid.NewGuid().ToString(),
             InstrumentId = Guid.NewGuid(),
             RequestId = Guid.NewGuid(),
             CreatedAt = DateTime.UtcNow,
             OrderDirection = OrderDirection.Buy,
             OrderType = OrderType.Market,
             Price = decimal.Zero,
             QuantityLots = 10,
             PriceType = PriceType.Unspecified,
             TimeInForceType = TimeInForceType.Unspecified,
        };
}
