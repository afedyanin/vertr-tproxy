using System.Collections.Concurrent;
using Grpc.Core;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;

namespace Vertr.Tproxy.Server.Application;

public sealed class MarketDataStreamProvider : IDisposable
{
    private readonly InvestApiClient _investApiClient;
    private readonly ILogger<MarketDataStreamProvider> _logger;

    private AsyncDuplexStreamingCall<MarketDataRequest, MarketDataResponse>? _stream;

    private readonly ConcurrentQueue<Trade> _trades;

    private readonly ConcurrentQueue<Candle> _candles;

    private readonly ConcurrentQueue<OrderBook> _orderBooks;

    private readonly ConcurrentQueue<LastPrice> _lastPrices;

    private readonly ConcurrentQueue<TradingStatus> _tradingStatuses;

    public SubscribeTradesResponse? SubscribeTradesResponse { get; private set; }

    public SubscribeCandlesResponse? SubscribeCandlesResponse { get; private set; }

    public SubscribeOrderBookResponse? SubscribeOrderBookResponse { get; private set; }

    public SubscribeLastPriceResponse? SubscribeLastPriceResponse { get; private set; }

    public SubscribeInfoResponse? SubscribeInfoResponse { get; private set; }

    public MarketDataStreamProvider(
        InvestApiClient investApiClient,
        ILogger<MarketDataStreamProvider> logger)
    {
        _investApiClient = investApiClient;
        _logger = logger;

        _trades = new ConcurrentQueue<Trade>();
        _candles = new ConcurrentQueue<Candle>();
        _orderBooks = new ConcurrentQueue<OrderBook>();
        _lastPrices = new ConcurrentQueue<LastPrice>();
        _tradingStatuses = new ConcurrentQueue<TradingStatus>();

        Restart();
    }

    public bool FetchTrade(out Trade? trade)
    {
        return _trades.TryDequeue(out trade);
    }

    public bool FetchCandle(out Candle? candle)
    {
        return _candles.TryDequeue(out candle);
    }

    public bool FetchOrderBook(out OrderBook? orderBook)
    {
        return _orderBooks.TryDequeue(out orderBook);
    }

    public bool FetchLastPrice(out LastPrice? lastPrice)
    {
        return _lastPrices.TryDequeue(out lastPrice);
    }

    public bool FetchTradingStatus(out TradingStatus? tradingStatus)
    {
        return _tradingStatuses.TryDequeue(out tradingStatus);
    }

    public async Task UpdateSubscriptionsStatus()
    {
        var request = new GetMySubscriptions();
        await _stream!.RequestStream.WriteAsync(new MarketDataRequest { GetMySubscriptions = request });
    }

    public async Task SubscribeToTrades(IEnumerable<string> instruments)
    {
        _logger.LogDebug($"Subscribing to Trades");
        var request = new SubscribeTradesRequest
        {
            Instruments =
            {
                instruments.Select(x => new TradeInstrument { InstrumentId = x })
            },
            SubscriptionAction = SubscriptionAction.Subscribe,
            TradeType = TradeSourceType.TradeSourceAll,
        };

        await _stream!.RequestStream.WriteAsync(new MarketDataRequest { SubscribeTradesRequest = request });
    }

    public async Task UnsubscribeFromTrades(IEnumerable<string> instruments)
    {
        _logger.LogWarning($"Unsubscribing from Trades");

        var request = new SubscribeTradesRequest
        {
            Instruments =
            {
                instruments.Select(x => new TradeInstrument { InstrumentId = x })
            },
            SubscriptionAction = SubscriptionAction.Unsubscribe,
            TradeType = TradeSourceType.TradeSourceAll,
        };

        await _stream!.RequestStream.WriteAsync(new MarketDataRequest { SubscribeTradesRequest = request });
    }

    public void Restart()
    {
        _stream?.Dispose();
        _stream = _investApiClient.MarketDataStream.MarketDataStream();
    }

    public async Task StartReading(CancellationToken cancellationToken)
    {
        await foreach (var response in _stream!.ResponseStream.ReadAllAsync(cancellationToken))
        {
            if (response.PayloadCase == MarketDataResponse.PayloadOneofCase.SubscribeTradesResponse)
            {
                SubscribeTradesResponse = response.SubscribeTradesResponse;
                _logger.LogWarning($"SubscribeTradesResponse={SubscribeTradesResponse}");
            }
            if (response.PayloadCase == MarketDataResponse.PayloadOneofCase.SubscribeCandlesResponse)
            {
                SubscribeCandlesResponse = response.SubscribeCandlesResponse;
                _logger.LogInformation($"SubscribeCandlesResponse={SubscribeCandlesResponse}");
            }
            if (response.PayloadCase == MarketDataResponse.PayloadOneofCase.SubscribeOrderBookResponse)
            {
                SubscribeOrderBookResponse = response.SubscribeOrderBookResponse;
                _logger.LogInformation($"SubscribeOrderBookResponse={SubscribeOrderBookResponse}");
            }
            if (response.PayloadCase == MarketDataResponse.PayloadOneofCase.SubscribeLastPriceResponse)
            {
                SubscribeLastPriceResponse = response.SubscribeLastPriceResponse;
                _logger.LogInformation($"SubscribeLastPriceResponse={SubscribeLastPriceResponse}");
            }
            if (response.PayloadCase == MarketDataResponse.PayloadOneofCase.SubscribeInfoResponse)
            {
                SubscribeInfoResponse = response.SubscribeInfoResponse;
                _logger.LogInformation($"SubscribeInfoResponse={SubscribeInfoResponse}");
            }
            if (response.PayloadCase == MarketDataResponse.PayloadOneofCase.Trade)
            {
                _trades.Enqueue(response.Trade);
            }
            if (response.PayloadCase == MarketDataResponse.PayloadOneofCase.Candle)
            {
                _candles.Enqueue(response.Candle);
            }
            if (response.PayloadCase == MarketDataResponse.PayloadOneofCase.Orderbook)
            {
                _orderBooks.Enqueue(response.Orderbook);
            }
            if (response.PayloadCase == MarketDataResponse.PayloadOneofCase.LastPrice)
            {
                _lastPrices.Enqueue(response.LastPrice);
            }
            if (response.PayloadCase == MarketDataResponse.PayloadOneofCase.TradingStatus)
            {
                _tradingStatuses.Enqueue(response.TradingStatus);
            }
            if (response.PayloadCase == MarketDataResponse.PayloadOneofCase.Ping)
            {
                _logger.LogDebug($"Ping={response.Ping}");
            }

            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    public void Dispose()
    {
        _stream?.Dispose();
    }
}
