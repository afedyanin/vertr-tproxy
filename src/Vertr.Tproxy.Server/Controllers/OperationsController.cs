using System.ComponentModel.DataAnnotations;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;
using Vertr.Tproxy.Server.Configuration;
using Vertr.Tproxy.Server.Converters;

namespace Vertr.Tproxy.Server.Controllers;
[Route("api/operations")]
[ApiController]
public class OperationsController : InvestApiControllerBase
{
    private readonly TproxySettings _settings;

    public OperationsController(
        InvestApiClient investApi,
        IOptions<TproxySettings> settings
        ) : base(investApi)
    {
        _settings = settings.Value;
    }

    [HttpGet()]
    public async Task<ActionResult<Client.Operations.GetOperationsResponse>> GetOperations([Required] DateTime from, [Required] DateTime to, int limit = 100, string cursor = "")
    {
        if (string.IsNullOrEmpty(_settings.AccountId))
        {
            return BadRequest("Invalid accountId.");
        }

        var request = new GetOperationsByCursorRequest()
        {
            AccountId = _settings.AccountId,
            Limit = limit,
            Cursor = cursor,
            From = Timestamp.FromDateTime(from),
            To = Timestamp.FromDateTime(to)
        };

        var response = await InvestApi.Operations.GetOperationsByCursorAsync(request);
        return Ok(response.Convert());
    }
}
