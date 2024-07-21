using Google.Protobuf.WellKnownTypes;
using Tinkoff.InvestApi.V1;

namespace Vertr.Tproxy.Intergration.Tests;

[TestFixture(Category = "System", Explicit = true)]
public class MarketDataTests : InvestApiTestBase
{
    [Test]
    public async Task CanGetCandles()
    {
        var from = new DateTime(2024, 07, 10, 10, 0, 0, DateTimeKind.Utc);
        var to = from.AddHours(1);

        var req = new GetCandlesRequest()
        {
            CandleSourceType = GetCandlesRequest.Types.CandleSource.Unspecified,
            InstrumentId = Mother.SberUuid.ToString(),
            Interval = CandleInterval._1Min,
            From = Timestamp.FromDateTime(from),
            To = Timestamp.FromDateTime(to)
        };

        var response = await Client.MarketData.GetCandlesAsync(req);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Candles, Is.Not.Null);

        foreach (var candle in response.Candles)
        {
            Console.WriteLine(candle.ToString());
        }
    }

    [Test]
    public async Task CanGetLastPrices()
    {
        var req = new GetLastPricesRequest();
        req.InstrumentId.Add(Mother.SberUuid.ToString());

        var response = await Client.MarketData.GetLastPricesAsync(req);
        Assert.That(response, Is.Not.Null);

        Assert.That(response.LastPrices, Is.Not.Null);

        foreach (var price in response.LastPrices)
        {
            Console.WriteLine(price.ToString());
        }
    }
}
