using System.ComponentModel.DataAnnotations;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;
using Vertr.Tproxy.Server.Formatters;

namespace Vertr.Tproxy.Server.Controllers;
[Route("api/demo")]
[ApiController]

public class DemoController : ControllerBase
{
    private readonly InvestApiClient _investApi;
    private readonly ILogger<DemoController> _logger;

    public DemoController(
        InvestApiClient investApi,
        ILogger<DemoController> logger)
    {
        _investApi = investApi;
        _logger = logger;
    }

    [HttpGet("user-info")]
    public async Task<ActionResult> GetUserInfo(CancellationToken cancellationToken)
    {
        var service = _investApi.Users;

        var accountsResponse = await service.GetAccountsAsync(cancellationToken);
        var infoResponse = await service.GetInfoAsync(cancellationToken);
        var userTariffResponse = await service.GetUserTariffAsync(cancellationToken);

        GetMarginAttributesResponse? marginAttributesResponse = null;
        try
        {
            marginAttributesResponse = await service.GetMarginAttributesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured.");
        }

        var res = new UserInfoFormatter(
            infoResponse,
            userTariffResponse,
            marginAttributesResponse,
            accountsResponse.Accounts)
            .Format();


        return Ok(res);
    }

    [HttpGet("operations-info")]
    public async Task<ActionResult> GetOperationsInfo(CancellationToken cancellationToken)
    {
        var accounts = await _investApi.Users.GetAccountsAsync();
        var accountId = accounts.Accounts.First().Id;

        var operations = _investApi.Operations;
        var portfolio = await operations.GetPortfolioAsync(new PortfolioRequest { AccountId = accountId },
            cancellationToken: cancellationToken);

        var positions = await operations.GetPositionsAsync(new PositionsRequest { AccountId = accountId },
            cancellationToken: cancellationToken);

        var withdrawLimits =
            await operations.GetWithdrawLimitsAsync(new WithdrawLimitsRequest { AccountId = accountId },
                cancellationToken: cancellationToken);

        var operationsResponse = await operations.GetOperationsAsync(new OperationsRequest
        {
            AccountId = accountId,
            From = Timestamp.FromDateTime(DateTime.UtcNow.AddMonths(-1)),
            To = Timestamp.FromDateTime(DateTime.UtcNow)
        }, cancellationToken: cancellationToken);

        var res = new OperationsFormatter(portfolio, positions, operationsResponse, withdrawLimits).Format();


        return Ok(res);
    }

    [HttpGet("trading-statuses")]
    public async Task<ActionResult> GetTradingStatuses([Required] string instrumentUid, CancellationToken cancellationToken)
    {
        var request = new GetTradingStatusesRequest();

        request.InstrumentId.Add(instrumentUid);
        //или 
        //request.InstrumentId.AddRange(new List<string>() {instrumentUid});

        var tradingStatuses = await _investApi.MarketData.GetTradingStatusesAsync(request: request, cancellationToken: cancellationToken);

        var res = new TradingStatusesFormatter(tradingStatuses).Format();

        return Ok(res);
    }


    [HttpGet("instruments")]
    public async Task<ActionResult> GetInstruments(CancellationToken cancellationToken)
    {
        var service = _investApi.Instruments;

        var shares = await service.SharesAsync(cancellationToken);
        var etfs = await service.EtfsAsync(cancellationToken);
        var bonds = await service.BondsAsync(cancellationToken);
        var futures = await service.FuturesAsync(cancellationToken);

        var dividends = new List<GetDividendsResponse>(3);
        foreach (var share in shares.Instruments.Take(dividends.Capacity))
        {
            var dividendsResponse = await service.GetDividendsAsync(new GetDividendsRequest
            {
                Figi = share.Figi,
                From = share.IpoDate,
                To = Timestamp.FromDateTime(DateTime.UtcNow)
            }, cancellationToken: cancellationToken);
            dividends.Add(dividendsResponse);
        }

        var accruedInterests = new List<GetAccruedInterestsResponse>(3);
        foreach (var bond in bonds.Instruments.Take(accruedInterests.Capacity))
        {
            var accruedInterestsResponse = await service.GetAccruedInterestsAsync(new GetAccruedInterestsRequest
            { Figi = bond.Figi, From = bond.PlacementDate, To = Timestamp.FromDateTime(DateTime.UtcNow) },
                cancellationToken: cancellationToken);
            accruedInterests.Add(accruedInterestsResponse);
        }

        var futuresMargin = new List<GetFuturesMarginResponse>(3);
        foreach (var future in futures.Instruments.Take(accruedInterests.Capacity))
        {
            var futureMargin = await service.GetFuturesMarginAsync(new GetFuturesMarginRequest { Figi = future.Figi },
                cancellationToken: cancellationToken);
            futuresMargin.Add(futureMargin);
        }

        var tradingSchedulesResponse = await service.TradingSchedulesAsync(new TradingSchedulesRequest
        {
            Exchange = "SPB",
            From = Timestamp.FromDateTime(DateTime.UtcNow.Date.AddDays(1)),
            To = Timestamp.FromDateTime(DateTime.UtcNow.Date.AddDays(3))
        }, cancellationToken: cancellationToken);


        var res = new InstrumentsFormatter(shares.Instruments, etfs.Instruments, bonds.Instruments, futures.Instruments,
            dividends, accruedInterests, futuresMargin, tradingSchedulesResponse).Format();

        return Ok(res);
    }
}
