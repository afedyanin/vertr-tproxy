
using System.Text.Json;
using Grpc.Core;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;
using Vertr.Tproxy.Server.Configuration;

namespace Vertr.Tproxy.Server.BackgroundServices;

public class OrderStateStreamService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly TproxySettings _tproxySettings;
    private readonly ILogger<OrderStateStreamService> _logger;


    public OrderStateStreamService(
        IServiceProvider serviceProvider,
        IOptions<TproxySettings> tproxySettings,
        ILogger<OrderStateStreamService> logger)
    {
        _services = serviceProvider;
        _tproxySettings = tproxySettings.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Start listening Order State stream...");

        var client = _services.GetRequiredService<InvestApiClient>();

        var request = new OrderStateStreamRequest()
        {
            Accounts =
            {
                _tproxySettings.AccountId,
            }
        };

        var stream = client.OrdersStream.OrderStateStream(request);

        await foreach (var response in stream.ResponseStream.ReadAllAsync())
        {
            if (response.PayloadCase == OrderStateStreamResponse.PayloadOneofCase.OrderState)
            {
                _logger.LogInformation(JsonSerializer.Serialize(response.OrderState));
            }
            if (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogWarning("Order state stream is stopping.");
        await base.StopAsync(stoppingToken);
    }
}
