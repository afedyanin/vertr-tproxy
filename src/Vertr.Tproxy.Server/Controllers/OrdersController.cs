using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;
using Vertr.Tproxy.Server.Configuration;
using Vertr.Tproxy.Server.Converters;

namespace Vertr.Tproxy.Server.Controllers;

[Route("api/orders")]
[ApiController]
public class OrdersController : InvestApiControllerBase
{
    private readonly TproxySettings _settings;

    public OrdersController(
        InvestApiClient investApi,
        IOptions<TproxySettings> settings
        ) : base(investApi)
    {
        _settings = settings.Value;
    }

    [HttpGet()]
    public async Task<ActionResult<Client.Orders.OrderStateResponse[]>> GetOrders()
    {
        if (string.IsNullOrEmpty(_settings.AccountId))
        {
            return BadRequest("Invalid accountId.");
        }

        var request = new GetOrdersRequest()
        {
            AccountId = _settings.AccountId,
        };

        var response = await InvestApi.Orders.GetOrdersAsync(request);

        var orderStates = response.Orders.Convert();

        return Ok(orderStates);
    }

    [HttpGet("state/{orderId}")]
    public async Task<ActionResult<Client.Orders.OrderStateResponse>> GetOrderState(string orderId)
    {
        if (string.IsNullOrEmpty(_settings.AccountId))
        {
            return BadRequest("Invalid accountId.");
        }

        var request = new GetOrderStateRequest()
        {
            AccountId = _settings.AccountId,
            OrderId = orderId,
            PriceType = PriceType.Unspecified,
        };

        var response = await InvestApi.Orders.GetOrderStateAsync(request);

        var orderState = response.Convert();

        return Ok(orderState);
    }

    [HttpPost("post")]
    public async Task<ActionResult<Client.Orders.PostOrderResponse>> PostOrder([FromBody] Client.Orders.PostOrderRequest orderRequest)
    {
        if (!orderRequest.IsValid())
        {
            return BadRequest("Invalid order request.");
        }

        var request = new PostOrderRequest
        {
            AccountId = _settings.AccountId,
            InstrumentId = orderRequest.InstrumentId,
            OrderId = orderRequest.OrderId.HasValue ? orderRequest.OrderId.ToString() : Guid.NewGuid().ToString(),
            Direction = orderRequest.QtyLots > 0 ? OrderDirection.Buy : OrderDirection.Sell,
            Quantity = Math.Abs(orderRequest.QtyLots),
            Price = orderRequest.Price,
            OrderType = orderRequest.Price == decimal.Zero ? OrderType.Market : OrderType.Limit,
            PriceType = PriceType.Unspecified,
            TimeInForce = TimeInForceType.TimeInForceUnspecified,
        };

        var response = await InvestApi.Orders.PostOrderAsync(request);

        var postOrderResponse = response.Convert();

        return Ok(postOrderResponse);
    }

    [HttpPost("cancel")]
    public async Task<ActionResult<DateTime?>> CancelOrder([FromBody] Client.Orders.CancelOrderRequest orderRequest)
    {
        if (!orderRequest.IsValid())
        {
            return BadRequest("Invalid order request.");
        }

        var request = new CancelOrderRequest()
        {
            AccountId = _settings.AccountId,
            OrderId = orderRequest.OrderId,
        };

        var response = await InvestApi.Orders.CancelOrderAsync(request);
        var cancelledTime = response.Time.Convert();

        return Ok(cancelledTime);
    }
}
