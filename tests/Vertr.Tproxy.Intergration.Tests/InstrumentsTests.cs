using Tinkoff.InvestApi.V1;

namespace Vertr.Tproxy.Intergration.Tests;

[TestFixture(Category = "System", Explicit = true)]
public class InstrumentsTests : InvestApiTestBase
{

    [Test]
    public async Task CanGetShares()
    {
        var req = new InstrumentsRequest()
        {
            InstrumentExchange = InstrumentExchangeType.InstrumentExchangeUnspecified,
            InstrumentStatus = InstrumentStatus.Base,
        };

        var response = await Client.Instruments.SharesAsync(req);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Instruments, Is.Not.Null);

        foreach (var share in response.Instruments)
        {
            Console.WriteLine($"Figi={share.Figi} Ticker={share.Ticker} ClassCode={share.ClassCode} Isin={share.Isin} Lot={share.Lot} Currency={share.Currency} Name={share.Name} Exchange={share.Exchange} TradingStatus={share.TradingStatus} ShareType={share.ShareType} Uid={share.Uid}  AssetUid={share.AssetUid} PositionUid={share.PositionUid} RealExchange={share.RealExchange} InstrumentExchange={share.InstrumentExchange}");
        }
    }

    [Test]
    public async Task CanGetShareBy()
    {
        var req = new InstrumentRequest()
        {
            ClassCode = "TQBR",
            IdType = InstrumentIdType.Ticker,
            Id = "SBER"
        };

        var response = await Client.Instruments.ShareByAsync(req);
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Instrument, Is.Not.Null);

        var share = response.Instrument;
        Console.WriteLine($"Figi={share.Figi} Ticker={share.Ticker} ClassCode={share.ClassCode} Isin={share.Isin} Lot={share.Lot} Currency={share.Currency} Name={share.Name} Exchange={share.Exchange} TradingStatus={share.TradingStatus} ShareType={share.ShareType} Uid={share.Uid}  AssetUid={share.AssetUid} PositionUid={share.PositionUid} RealExchange={share.RealExchange} InstrumentExchange={share.InstrumentExchange}");
    }
}
