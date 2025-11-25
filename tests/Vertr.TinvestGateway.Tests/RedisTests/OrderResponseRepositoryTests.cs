using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts.Orders;
using Vertr.TinvestGateway.Contracts.Orders.Enums;
using Vertr.TinvestGateway.DataAccess.Redis;

namespace Vertr.TinvestGateway.Tests.RedisTests;

public class OrderResponseRepositoryTests
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
        var repo = new OrderResponseRepository(_connectionMultiplexer);
        await repo.Clear();
    }

    [Test]
    public async Task CanSaveOrderResponse()
    {
        var repo = new OrderResponseRepository(_connectionMultiplexer);
        var orderResponse = CreateOrderResponse();

        var portfolioId = Guid.NewGuid();
        await repo.Save(orderResponse, portfolioId);
        var saved = await repo.Get(new Guid(orderResponse.OrderId));
        Assert.That(saved, Is.Not.Null);

        Console.WriteLine(saved);
    }

    [Test]
    public async Task CanGetOrderRequestsByPortfolio()
    {
        var repo = new OrderResponseRepository(_connectionMultiplexer);
        var or1 = CreateOrderResponse();
        var or2 = CreateOrderResponse();

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

    private static PostOrderResponse CreateOrderResponse()
        => new PostOrderResponse
        {
            OrderId = Guid.NewGuid().ToString(),
            InstrumentId = Guid.NewGuid(),
            OrderDirection = OrderDirection.Buy,
            OrderType = OrderType.Market,
            OrderRequestId = Guid.NewGuid().ToString(),
        };
}
