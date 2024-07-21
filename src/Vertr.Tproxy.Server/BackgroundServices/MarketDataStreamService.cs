
using Grpc.Core;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi.V1;
using Vertr.Infrastructure.Kafka;
using Vertr.Infrastructure.Kafka.Abstractions;
using Vertr.Tproxy.Server.Application;

namespace Vertr.Tproxy.Server.BackgroundServices;

public class MarketDataStreamService : BackgroundService
{
    private const int _fetchDelay = 10;
    private readonly IServiceProvider _services;
    private readonly ILogger<MarketDataStreamService> _logger;

    private readonly string _tradesTopic;
    private readonly string _candlesTopic;
    private readonly string _orderBooksTopic;
    private readonly string _lastPricesTopic;
    private readonly string _tradingStatusesTopic;

    public MarketDataStreamService(
        IServiceProvider serviceProvider,
        IOptions<KafkaSettings> kafkaSettings,
        ILogger<MarketDataStreamService> logger)
    {
        _services = serviceProvider;
        _logger = logger;

        var topics = kafkaSettings.Value.Topics;

        topics.TryGetValue(Consts.MarketTradesTopicKey, out var tradesTopic);
        _tradesTopic = tradesTopic ?? throw new ArgumentException("Trades topic is not defined.");

        topics.TryGetValue(Consts.CandlesTopicKey, out var candlesTopic);
        _candlesTopic = candlesTopic ?? throw new ArgumentException("Candles topic is not defined.");

        topics.TryGetValue(Consts.OrderBooksTopicKey, out var obTopic);
        _orderBooksTopic = obTopic ?? throw new ArgumentException("OrderBooks topic is not defined.");

        topics.TryGetValue(Consts.LastPricesTopicKey, out var lpTopic);
        _lastPricesTopic = lpTopic ?? throw new ArgumentException("Last prices topic is not defined.");

        topics.TryGetValue(Consts.TradingStatusesTopicKey, out var tsTopic);
        _tradingStatusesTopic = tsTopic ?? throw new ArgumentException("Trading statuses topic is not defined.");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Market data stream is started at {DateTime.UtcNow}");
        var provider = _services.GetRequiredService<MarketDataStreamProvider>();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var tradesProducer = ProduceTrades(stoppingToken);
                var candlesProducer = ProduceCandles(stoppingToken);
                var orderBooksProducer = ProduceOrderBooks(stoppingToken);
                var lastPricesProducer = ProduceLastPrices(stoppingToken);
                var tradingStatusesProducer = ProduceTradingStatuses(stoppingToken);
                var marketDataStreamConsumer = provider.StartReading(stoppingToken);

                await Task.WhenAll(
                    marketDataStreamConsumer,
                    tradesProducer,
                    candlesProducer,
                    orderBooksProducer,
                    lastPricesProducer,
                    tradingStatusesProducer);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (RpcException rpcEx)
            {
                if (rpcEx.StatusCode == StatusCode.Cancelled)
                {
                    break;
                }

                _logger.LogError(rpcEx, $"Market data service exception. Message={rpcEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Critial exception: {ex.Message}");
            }

            _logger.LogWarning("Market data stream restaring...");
            provider.Restart();
        }

        _logger.LogInformation($"Market data stream execution completed at {DateTime.UtcNow}");
    }

    private async Task ProduceTrades(CancellationToken cancellationToken)
    {
        var kafkaProducer = _services.GetRequiredService<IProducerWrapper<string, Trade>>();
        var provider = _services.GetRequiredService<MarketDataStreamProvider>();

        _logger.LogInformation($"Trades producer started at {DateTime.UtcNow}");

        while (!cancellationToken.IsCancellationRequested)
        {
            while (provider.FetchTrade(out var trade))
            {
                if (trade != null)
                {
                    _logger.LogDebug($"Sending to Kafka. Trade={trade}");
                    await kafkaProducer.Produce(_tradesTopic, trade.InstrumentUid, trade, null, cancellationToken);
                }
            }

            await Task.Delay(_fetchDelay);
        }
    }

    private async Task ProduceCandles(CancellationToken cancellationToken)
    {
        var kafkaProducer = _services.GetRequiredService<IProducerWrapper<string, Candle>>();
        var provider = _services.GetRequiredService<MarketDataStreamProvider>();

        _logger.LogInformation($"Candles producer started at {DateTime.UtcNow}");

        while (!cancellationToken.IsCancellationRequested)
        {
            while (provider.FetchCandle(out var candle))
            {
                if (candle != null)
                {
                    _logger.LogDebug($"Sending to Kafka. Candle={candle}");
                    await kafkaProducer.Produce(_candlesTopic, candle.InstrumentUid, candle, null, cancellationToken);
                }
            }

            await Task.Delay(_fetchDelay);
        }
    }

    private async Task ProduceOrderBooks(CancellationToken cancellationToken)
    {
        var kafkaProducer = _services.GetRequiredService<IProducerWrapper<string, OrderBook>>();
        var provider = _services.GetRequiredService<MarketDataStreamProvider>();

        _logger.LogInformation($"Order Books producer started at {DateTime.UtcNow}");

        while (!cancellationToken.IsCancellationRequested)
        {
            while (provider.FetchOrderBook(out var orderBook))
            {
                if (orderBook != null)
                {
                    _logger.LogDebug($"Sending to Kafka. OrderBook={orderBook}");
                    await kafkaProducer.Produce(_orderBooksTopic, orderBook.InstrumentUid, orderBook, null, cancellationToken);
                }
            }

            await Task.Delay(_fetchDelay);
        }
    }

    private async Task ProduceLastPrices(CancellationToken cancellationToken)
    {
        var kafkaProducer = _services.GetRequiredService<IProducerWrapper<string, LastPrice>>();
        var provider = _services.GetRequiredService<MarketDataStreamProvider>();

        _logger.LogInformation($"Last prices producer started at {DateTime.UtcNow}");

        while (!cancellationToken.IsCancellationRequested)
        {
            while (provider.FetchLastPrice(out var lastPrice))
            {
                if (lastPrice != null)
                {
                    _logger.LogDebug($"Sending to Kafka. LastPrice={lastPrice}");
                    await kafkaProducer.Produce(_lastPricesTopic, lastPrice.InstrumentUid, lastPrice, null, cancellationToken);
                }
            }

            await Task.Delay(_fetchDelay);
        }
    }

    private async Task ProduceTradingStatuses(CancellationToken cancellationToken)
    {
        var kafkaProducer = _services.GetRequiredService<IProducerWrapper<string, TradingStatus>>();
        var provider = _services.GetRequiredService<MarketDataStreamProvider>();

        _logger.LogInformation($"Trading statuses producer started at {DateTime.UtcNow}");

        while (!cancellationToken.IsCancellationRequested)
        {
            while (provider.FetchTradingStatus(out var tradingStatus))
            {
                if (tradingStatus != null)
                {
                    _logger.LogDebug($"Sending to Kafka. TradingStatus={tradingStatus}");
                    await kafkaProducer.Produce(_tradingStatusesTopic, tradingStatus.InstrumentUid, tradingStatus, null, cancellationToken);
                }
            }

            await Task.Delay(_fetchDelay);
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogWarning("Market data stream is stopping...");
        await base.StopAsync(stoppingToken);
    }
}
