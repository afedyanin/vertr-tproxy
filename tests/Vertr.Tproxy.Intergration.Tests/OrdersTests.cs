
using Tinkoff.InvestApi.V1;

namespace Vertr.Tproxy.Intergration.Tests;

public class OrdersTests : InvestApiTestBase
{
    [Test]
    public async Task CanPostMarketOrder()
    {
        var req = new PostOrderRequest()
        {
            AccountId = Mother.AccountId,
            Direction = OrderDirection.Buy,
            InstrumentId = Mother.SberUuid,
            OrderId = Guid.NewGuid().ToString(),
            OrderType = OrderType.Market,
            Quantity = 5,
        };

        var response = await Client.Orders.PostOrderAsync(req);
        Assert.That(response, Is.Not.Null);

        Console.WriteLine($@"
Direction={response.Direction}
ExecutedCommission={response.ExecutedCommission}
ExecutedOrderPrice={response.ExecutedOrderPrice}
ExecutionReportStatus={response.ExecutionReportStatus}
Figi={response.Figi}
InitialCommission={response.InitialCommission}
InitialOrderPrice={response.InitialOrderPrice}
InitialSecurityPrice={response.InitialSecurityPrice}
InstrumentUid={response.InstrumentUid}
LotsExecuted={response.LotsExecuted}
LotsRequested={response.LotsRequested}
Message={response.Message}
OrderId={response.OrderId}
OrderRequestId={response.OrderRequestId}
OrderType={response.OrderType}
ResponseMetadata={response.ResponseMetadata}
TotalOrderAmount={response.TotalOrderAmount}
");

    }

    [Test]
    public async Task CanPostLimitOrder()
    {
        var req = new PostOrderRequest()
        {
            AccountId = Mother.AccountId,
            Direction = OrderDirection.Sell,
            InstrumentId = Mother.SberUuid,
            OrderId = Guid.NewGuid().ToString(),
            OrderType = OrderType.Limit,
            PriceType = PriceType.Currency,
            Price = new Quotation()
            {
                Units = 312,
                Nano = 5,
            },
            Quantity = 10,
            TimeInForce = TimeInForceType.TimeInForceDay,
        };

        var response = await Client.Orders.PostOrderAsync(req);
        Assert.That(response, Is.Not.Null);

        Console.WriteLine($@"
Direction={response.Direction}
ExecutedCommission={response.ExecutedCommission}
ExecutedOrderPrice={response.ExecutedOrderPrice}
ExecutionReportStatus={response.ExecutionReportStatus}
Figi={response.Figi}
InitialCommission={response.InitialCommission}
InitialOrderPrice={response.InitialOrderPrice}
InitialSecurityPrice={response.InitialSecurityPrice}
InstrumentUid={response.InstrumentUid}
LotsExecuted={response.LotsExecuted}
LotsRequested={response.LotsRequested}
Message={response.Message}
OrderId={response.OrderId}
OrderRequestId={response.OrderRequestId}
OrderType={response.OrderType}
ResponseMetadata={response.ResponseMetadata}
TotalOrderAmount={response.TotalOrderAmount}
");

    }

    [Test]
    public async Task CanGetOrders()
    {
        var req = new GetOrdersRequest()
        {
            AccountId = Mother.AccountId,
        };

        var response = await Client.Orders.GetOrdersAsync(req);

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Orders, Is.Not.Null);

        foreach (var order in response.Orders)
        {
            Console.WriteLine($@"
AveragePositionPrice={order.AveragePositionPrice}
Currency={order.Currency}
Direction={order.Direction}
ExecutedCommission={order.ExecutedCommission}
ExecutedOrderPrice={order.ExecutedOrderPrice}
ExecutionReportStatus={order.ExecutionReportStatus}
Figi={order.Figi}
InitialCommission={order.InitialCommission}
InitialOrderPrice={order.InitialOrderPrice}
InitialSecurityPrice={order.InitialSecurityPrice}
InstrumentUid={order.InstrumentUid}
LotsExecuted={order.LotsExecuted}
LotsRequested={order.LotsRequested}
OrderDate={order.OrderDate}
OrderId={order.OrderId}
OrderType={order.OrderType}
OrderRequestId={order.OrderRequestId}
ServiceCommission={order.ServiceCommission}
TotalOrderAmount={order.TotalOrderAmount}
Stages={string.Join(',', order.Stages)}
");
            Console.WriteLine("\n");
        }
    }
}
