using System.Collections.Concurrent;
using Grpc.Core;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;
using Vertr.Infrastructure.Kafka;
using Vertr.Infrastructure.Kafka.Abstractions;
using Vertr.Tproxy.Server.Configuration;
using Vertr.Tproxy.Server.Converters;

namespace Vertr.Tproxy.Server.BackgroundServices;

public class TradesStreamService : BackgroundService
{
    private readonly int _streamDeadlineLongSeconds = 51;
    private readonly int _streamDeadlineShortSeconds = 17;

    private readonly IServiceProvider _services;
    private readonly TproxySettings _tproxySettings;
    private readonly ILogger<TradesStreamService> _logger;

    private readonly ConcurrentQueue<Client.Orders.OrderTrade> _trades;

    private readonly string _orderTradesTopic;

    public TradesStreamService(
        IServiceProvider serviceProvider,
        IOptions<TproxySettings> tproxySettings,
        IOptions<KafkaSettings> kafkaSettings,
        ILogger<TradesStreamService> logger)
    {
        _services = serviceProvider;
        _logger = logger;
        _tproxySettings = tproxySettings.Value;

        kafkaSettings.Value.Topics.TryGetValue(Consts.OorderTradesTopicKey, out var topic);
        _orderTradesTopic = topic ?? throw new ArgumentException("Order trades topic is not defined.");

        _trades = new ConcurrentQueue<Client.Orders.OrderTrade>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var producingTask = ProduceTrades(stoppingToken);
        var consumingLongTask = StartConsumingLoop(stoppingToken, _streamDeadlineLongSeconds);
        var consumingShortTask = StartConsumingLoop(stoppingToken, _streamDeadlineShortSeconds);

        await Task.WhenAll(producingTask, consumingLongTask, consumingShortTask);

        _logger.LogInformation($"TradesStreamService execution completed at {DateTime.UtcNow}");
    }

    private async Task StartConsumingLoop(CancellationToken stoppingToken, int deadlineSeconds)
    {
        var client = _services.GetRequiredService<InvestApiClient>();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation($"Trades stream started at {DateTime.UtcNow} whith deadline={deadlineSeconds}");
                var deadline = DateTime.UtcNow.AddSeconds(deadlineSeconds);
                await StartTradesSream(client, deadline, stoppingToken);
            }
            catch (RpcException rpcEx)
            {
                if (rpcEx.StatusCode != StatusCode.DeadlineExceeded)
                {
                    _logger.LogError(rpcEx, $"Trades consuming exception. Message={rpcEx.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Trades consuming exception. Message={ex.Message}");
            }
        }
    }

    private async Task StartTradesSream(
        InvestApiClient client,
        DateTime deadline,
        CancellationToken cancellationToken)
    {
        var request = new TradesStreamRequest
        {
            Accounts =
            {
                _tproxySettings.AccountId,
            }
        };

        using (var stream = client.OrdersStream.TradesStream(request, headers: null, deadline, cancellationToken))
        {
            await foreach (var response in stream.ResponseStream.ReadAllAsync(cancellationToken))
            {
                if (response.PayloadCase == TradesStreamResponse.PayloadOneofCase.OrderTrades)
                {
                    foreach (var trade in response.OrderTrades.Convert())
                    {
                        // _logger.LogDebug($"Enqueue trade. Trade={trade}");
                        _trades.Enqueue(trade);
                    }
                }
            }
        }
    }

    private async Task ProduceTrades(CancellationToken cancellationToken)
    {
        var kafkaProducer = _services.GetRequiredService<IProducerWrapper<string, Client.Orders.OrderTrade>>();

        _logger.LogInformation($"Trades producer started at {DateTime.UtcNow}");

        while (!cancellationToken.IsCancellationRequested)
        {
            while (_trades.TryDequeue(out var trade))
            {
                _logger.LogDebug($"Sending to Kafka. Trade={trade}");
                await kafkaProducer.Produce(_orderTradesTopic, trade.OrderId, trade, null, cancellationToken);
            }

            await Task.Delay(10);
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogWarning("Trades stream is stopping...");
        await base.StopAsync(stoppingToken);
    }
}
