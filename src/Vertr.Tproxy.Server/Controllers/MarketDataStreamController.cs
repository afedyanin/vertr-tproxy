using Microsoft.AspNetCore.Mvc;
using Tinkoff.InvestApi.V1;
using Vertr.Tproxy.Server.Application;

namespace Vertr.Tproxy.Server.Controllers;

[Route("api/market-data")]
[ApiController]
public class MarketDataStreamController : ControllerBase
{
    private readonly MarketDataStreamProvider _marketDataStreamProvider;

    public MarketDataStreamController(MarketDataStreamProvider marketDataStreamProvider)
    {
        _marketDataStreamProvider = marketDataStreamProvider;
    }

    [HttpGet("trades/status")]
    public async Task<ActionResult<SubscribeTradesResponse>> GetTradesSubscriptionStatus()
    {
        await _marketDataStreamProvider.UpdateSubscriptionsStatus();
        await Task.Delay(100);
        var tradesSub = _marketDataStreamProvider.SubscribeTradesResponse;
        return Ok(tradesSub);
    }

    [HttpPost("trades/subscribe")]
    public async Task<ActionResult> SubscribeTrades([FromBody] string[] instrumentIds)
    {
        await _marketDataStreamProvider.SubscribeToTrades(instrumentIds);
        await Task.Delay(100);
        var tradesSub = _marketDataStreamProvider.SubscribeTradesResponse;
        return Ok(tradesSub);
    }

    [HttpPost("trades/unsubscribe")]
    public async Task<ActionResult<SubscribeTradesResponse>> UnsubscribeTrades([FromBody] string[] instrumentIds)
    {
        await _marketDataStreamProvider.UnsubscribeFromTrades(instrumentIds);
        await Task.Delay(100);
        var tradesSub = _marketDataStreamProvider.SubscribeTradesResponse;
        return Ok(tradesSub);
    }
}
