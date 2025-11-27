using Microsoft.AspNetCore.Mvc;
using Vertr.TinvestGateway.Contracts.Orders;
using Vertr.TinvestGateway.Repositories;

namespace Vertr.TinvestGateway.Host.Controllers;

[Route("api/order-storage")]
[ApiController]
public class OrderStorageController : ControllerBase
{
    private readonly IOrderTradeRepository _orderTradeRepository;
    private readonly IOrderStateRepository _orderStateRepository;
    private readonly IOrderRequestRepository _orderRequestRepository;
    private readonly IOrderResponseRepository _orderResponseRepository;
    private readonly IPortfolioRepository _portfolioRepository;

    public OrderStorageController(
        IOrderTradeRepository orderTradeRepository,
        IOrderStateRepository orderStateRepository,
        IOrderRequestRepository orderRequestRepository,
        IOrderResponseRepository orderResponseRepository,
        IPortfolioRepository portfolioRepository)
    {
        _orderTradeRepository  = orderTradeRepository;
        _orderStateRepository = orderStateRepository;
        _orderRequestRepository = orderRequestRepository;
        _orderResponseRepository = orderResponseRepository;
        _portfolioRepository = portfolioRepository;
    }

    [HttpGet("trades")]
    public async Task<IActionResult> FindOrderTrades([FromQuery] string pattern)
    {
        var trades = await _orderTradeRepository.Find(pattern);
        return Ok(trades);
    }

    [HttpGet("states")]
    public async Task<IActionResult> FingOrderStates([FromQuery] string pattern)
    {
        var items = await _orderStateRepository.Find(pattern);
        return Ok(items);
    }

    [HttpGet("requests/{portfolioId:guid}")]
    public async Task<IActionResult> GetOrderRquests(Guid portfolioId)
    {
        var requests = await _orderRequestRepository.GetByPortfolioId(portfolioId);
        return Ok(requests);
    }

    [HttpGet("order-request/{requestId:guid}")]
    public async Task<IActionResult> GetOrderRquest(Guid requestId)
    {
        var request = await _orderRequestRepository.Get(requestId);

        if (request == null)
        {
            return NotFound();
        }

        return Ok(request);
    }

    [HttpGet("responses/{portfolioId:guid}")]
    public async Task<IActionResult> GetOrderResponses(Guid portfolioId)
    {
        var responses = await _orderResponseRepository.GetByPortfolioId(portfolioId);
        return Ok(responses);
    }

    [HttpGet("order-response/{orderId:guid}")]
    public async Task<IActionResult> GetOrderResponse(Guid orderId)
    {
        var response = await _orderResponseRepository.Get(orderId);

        if (response == null)
        {
            return NotFound(); 
        }

        return Ok(response);
    }
    /*
    [HttpGet("portfolio-trades/{portfolioId:guid}")]
    public async Task<IActionResult> GetPortfolioTrades(Guid portfolioId)
    {
        var responses = await _orderResponseRepository.GetByPortfolioId(portfolioId);
        var orderIds = responses.Select(x => x.OrderId);

        var allTrades = new List<OrderTrades>();

        foreach (var orderId in orderIds)
        {
            var trades = await _orderTradeRepository.GetByOrderId(orderId.ToString());
            allTrades.AddRange(trades);
        }

        return Ok(allTrades.ToArray());
    }
    */

    [HttpGet("portfolio/{portfolioId:guid}")]
    public async Task<IActionResult> GetPortfolio(Guid portfolioId)
    {
        var portfolio = await _portfolioRepository.GetById(portfolioId);

        if (portfolio == null)
        {
            return NotFound(); 
        }

        return Ok(portfolio);
    }
}
