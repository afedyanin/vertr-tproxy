using Google.Protobuf.WellKnownTypes;
using Tinkoff.InvestApi.V1;

namespace Vertr.Tproxy.Intergration.Tests;


[TestFixture(Category = "System", Explicit = true)]
public class OperationsTests : InvestApiTestBase
{
    [Test]
    public async Task CanGetOperations()
    {
        var req = new OperationsRequest()
        {
            AccountId = Mother.AccountId,
            State = OperationState.Unspecified,
            From = Timestamp.FromDateTime(new DateTime(2024, 07, 01, 0, 0, 0, DateTimeKind.Utc)),
            To = Timestamp.FromDateTime(new DateTime(2024, 07, 15, 0, 0, 0, DateTimeKind.Utc))
        };

        var response = await Client.Operations.GetOperationsAsync(req);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Operations, Is.Not.Null);

        foreach (var oper in response.Operations)
        {
            Console.WriteLine(oper.ToString());
        }
    }

    [Test]
    public async Task CanGetPortfolio()
    {
        var req = new PortfolioRequest()
        {
            AccountId = Mother.AccountId,
            Currency = PortfolioRequest.Types.CurrencyRequest.Rub,
        };

        var response = await Client.Operations.GetPortfolioAsync(req);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Positions, Is.Not.Null);

        foreach (var pos in response.Positions)
        {
            Console.WriteLine(pos.ToString());
        }
    }

    [Test]
    public async Task CanGetPositions()
    {
        var req = new PositionsRequest()
        {
            AccountId = Mother.AccountId,
        };

        var response = await Client.Operations.GetPositionsAsync(req);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Securities, Is.Not.Null);

        foreach (var pos in response.Securities)
        {
            Console.WriteLine(pos.ToString());
        }
    }
}
