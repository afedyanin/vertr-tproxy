using Microsoft.AspNetCore.Mvc;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;
using Vertr.Tproxy.Server.Converters;

namespace Vertr.Tproxy.Server.Controllers;
[Route("api/instruments")]
[ApiController]

public class InstrumentsController : InvestApiControllerBase
{
    public InstrumentsController(InvestApiClient investApi) : base(investApi)
    {
    }

    [HttpGet("shares")]
    public async Task<ActionResult<Share[]>> GetShares()
    {
        var request = new InstrumentsRequest
        {
            InstrumentExchange = InstrumentExchangeType.InstrumentExchangeUnspecified,
            InstrumentStatus = InstrumentStatus.Base,
        };

        var response = await InvestApi.Instruments.SharesAsync(request);
        var shares = response.Instruments.Convert();
        return Ok(shares);
    }

    [HttpGet("shares/{classcode}/{ticker}")]
    public async Task<ActionResult<Share>> GetShare(string classcode, string ticker)
    {
        if (string.IsNullOrEmpty(ticker))
        {
            return BadRequest();
        }

        if (string.IsNullOrEmpty(classcode))
        {
            return BadRequest();
        }

        var request = new InstrumentRequest
        {
            IdType = InstrumentIdType.Ticker,
            ClassCode = classcode,
            Id = ticker
        };

        var response = await InvestApi.Instruments.ShareByAsync(request);
        var share = response.Instrument.Convert();
        return Ok(share);
    }
}
