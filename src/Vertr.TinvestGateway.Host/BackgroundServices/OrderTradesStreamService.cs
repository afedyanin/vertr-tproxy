using Grpc.Core;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Vertr.TinvestGateway.BackgroundServices;
using Vertr.TinvestGateway.Contracts.Repositories;
using Vertr.TinvestGateway.Converters;

namespace Vertr.TinvestGateway.Host.BackgroundServices;

public class OrderTradesStreamService : StreamServiceBase
{
    protected override bool IsEnabled => TinvestSettings.OrderTradesStreamEnabled;

    public OrderTradesStreamService(
        IServiceProvider serviceProvider,
        IOptions<TinvestSettings> tinvestOptions,
        ILogger<OrderTradesStreamService> logger) :
            base(serviceProvider, tinvestOptions, logger)
    {
    }

    protected override async Task Subscribe(
        ILogger logger,
        DateTime? deadline = null,
        CancellationToken stoppingToken = default)
    {

        using var scope = ServiceProvider.CreateScope();
        var investApiClient = scope.ServiceProvider.GetRequiredService<InvestApiClient>();
        var orderTradeRepository = scope.ServiceProvider.GetRequiredService<IOrderTradeRepository>();

        var request = new Tinkoff.InvestApi.V1.TradesStreamRequest();
        request.Accounts.Add(TinvestSettings.AccountId);

        using var stream = investApiClient.OrdersStream.TradesStream(request, headers: null, deadline, stoppingToken);

        await foreach (var response in stream.ResponseStream.ReadAllAsync(stoppingToken))
        {
            if (response.PayloadCase == Tinkoff.InvestApi.V1.TradesStreamResponse.PayloadOneofCase.OrderTrades)
            {
                var instrumentId = Guid.Parse(response.OrderTrades.InstrumentUid);

                // var currency = await currencyRepository.GetInstrumentCurrency(instrumentId);
                var orderTrades = response.OrderTrades.Convert();
                await orderTradeRepository.Save(orderTrades);

                logger.LogInformation($"New order trades received for OrderId={response.OrderTrades.OrderId} InstrumentId={instrumentId}");
            }
            else if (response.PayloadCase == Tinkoff.InvestApi.V1.TradesStreamResponse.PayloadOneofCase.Ping)
            {
                logger.LogDebug($"Trades ping received: {response.Ping}");
            }
            else if (response.PayloadCase == Tinkoff.InvestApi.V1.TradesStreamResponse.PayloadOneofCase.Subscription)
            {
                logger.LogInformation($"Trades subscription received: {response.Subscription}");
            }
        }
    }
}
