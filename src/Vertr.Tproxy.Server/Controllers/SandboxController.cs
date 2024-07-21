using Microsoft.AspNetCore.Mvc;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;
using Vertr.Tproxy.Client;
using Vertr.Tproxy.Client.Accounts;
using Vertr.Tproxy.Server.Converters;

namespace Vertr.Tproxy.Server.Controllers;
[Route("api/sandbox")]
[ApiController]
public class SandboxController : InvestApiControllerBase
{
    public SandboxController(InvestApiClient investApi) : base(investApi)
    {
    }

    [HttpPost("open-account")]
    public async Task<ActionResult<string>> OpenSandboxAccount([FromBody] string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return BadRequest();
        }

        var request = new OpenSandboxAccountRequest { Name = name };
        var response = await InvestApi.Sandbox.OpenSandboxAccountAsync(request);
        var accountId = response.AccountId;

        return Ok(accountId);
    }

    [HttpPost("close-account")]
    public async Task<ActionResult> CloseSandboxAccount([FromBody] string accountId)
    {
        if (string.IsNullOrEmpty(accountId))
        {
            return BadRequest();
        }

        var request = new CloseSandboxAccountRequest { AccountId = accountId };
        var response = await InvestApi.Sandbox.CloseSandboxAccountAsync(request);

        return NoContent();
    }

    [HttpPost("deposit")]
    public async Task<ActionResult<Money>> DepositSandboxAccount([FromBody] DepositRequest depoRequest)
    {
        if (string.IsNullOrEmpty(depoRequest.AccountId))
        {
            return BadRequest();
        }

        var request = new SandboxPayInRequest
        {
            AccountId = depoRequest.AccountId,
            Amount = depoRequest.Amount.Convert()
        };

        var response = await InvestApi.Sandbox.SandboxPayInAsync(request);
        var balance = response.Balance.Convert();

        return Ok(balance);
    }

    [HttpPost("post-sandbox-order/{accountId}")]
    public async Task<ActionResult<Client.Orders.PostOrderResponse>> PostOrder(
        string accountId,
        [FromBody] Client.Orders.PostOrderRequest orderRequest)
    {
        if (!orderRequest.IsValid())
        {
            return BadRequest("Invalid order request.");
        }

        var request = new PostOrderRequest
        {
            AccountId = accountId,
            InstrumentId = orderRequest.InstrumentId,
            OrderId = orderRequest.OrderId.HasValue ? orderRequest.OrderId.ToString() : Guid.NewGuid().ToString(),
            Direction = orderRequest.QtyLots > 0 ? OrderDirection.Buy : OrderDirection.Sell,
            Quantity = Math.Abs(orderRequest.QtyLots),
            Price = orderRequest.Price,
            OrderType = orderRequest.Price == decimal.Zero ? OrderType.Market : OrderType.Limit,
            PriceType = PriceType.Unspecified,
            TimeInForce = TimeInForceType.TimeInForceUnspecified,
        };

        var response = await InvestApi.Sandbox.PostSandboxOrderAsync(request);

        var postOrderResponse = response.Convert();

        return Ok(postOrderResponse);
    }

    [HttpGet("withdraw-limits")]
    public async Task<ActionResult<WithdrawLimits>> GetWithdrawLimits([FromQuery] string accountId)
    {
        if (string.IsNullOrEmpty(accountId))
        {
            return BadRequest();
        }

        var request = new WithdrawLimitsRequest { AccountId = accountId };
        var response = await InvestApi.Sandbox.GetSandboxWithdrawLimitsAsync(request);

        var res = response.Convert();

        return Ok(res);
    }
}
