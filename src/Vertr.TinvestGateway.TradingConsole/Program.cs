using Refit;
using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts;
using Vertr.TinvestGateway.Contracts.MarketData;
using Vertr.TinvestGateway.Contracts.Orders;
using Vertr.TinvestGateway.Contracts.Orders.Enums;
using static StackExchange.Redis.RedisChannel;

namespace Vertr.TinvestGateway.TradingConsole;

internal static class Program
{
    private static readonly Guid InstrumentId = new Guid("e6123145-9665-43e0-8413-cd61b8aa9b13");
    private static readonly Guid PortfolioId = new Guid("73C4E51B-B0C9-4D5F-999E-171140E79C54");

    private static volatile bool _isCancelled;
    private static ConnectionMultiplexer? _connection;
    private static ISubscriber? _subscriber;
    private static ITinvestGatewayClient? _gatewayClient;

    public static async Task Main(string[] args)
    {
        _connection = await ConnectionMultiplexer.ConnectAsync("localhost");
        _subscriber = _connection.GetSubscriber();

        _gatewayClient = RestService.For<ITinvestGatewayClient>(
            "http://localhost:5099",
            new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(JsonOptions.DefaultOptions)
            });

        var candlesChannel = new RedisChannel("market.candles*", PatternMode.Pattern);
        await _subscriber.SubscribeAsync(candlesChannel, async (ch, message) =>
        {
            var candle = Candlestick.FromJson(message.ToString());
            Console.WriteLine($"Received candle: {ch} - {candle}");

            await PostRandomOrder();
        });

        var portfoliosChannel = new RedisChannel("portfolios", PatternMode.Literal);
        await _subscriber.SubscribeAsync(portfoliosChannel, (ch, message) =>
        {
            // var portfolio = Portfolio.FromJson(message.ToString());
            Console.WriteLine($"Received portfolio: {ch} - {message}");
        });

        Console.WriteLine("Press Ctrl+C to interrupt the application.");
        Console.CancelKeyPress += OnCancelKeyPress;
        while (!_isCancelled)
        {
            Console.WriteLine("Working...");
            await Task.Delay(20000);
        }

        Console.WriteLine("Application exiting gracefully after cleanup.");
    }

    private static void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs args)
    {
        Console.WriteLine("\nCtrl+C detected! Performing cleanup...");

        // Prevent immediate termination
        args.Cancel = true;

        _subscriber?.UnsubscribeAll();
        _connection?.Close();

        // TODO: Dump portfolios

        _isCancelled = true; // Signal to the main loop to exit
    }

    private static async Task PostRandomOrder()
    {
        var request = new PostOrderRequest
        {
            RequestId = Guid.NewGuid(),
            InstrumentId = InstrumentId, // Get from channel name
            PortfolioId = PortfolioId, // Get From Strategy Dictionary
            OrderDirection = GetRandomDirection(),
            OrderType = OrderType.Market,
            TimeInForceType = TimeInForceType.Unspecified,
            PriceType = PriceType.Unspecified,
            Price = 0.0m,
            QuantityLots = 10,
            CreatedAt = DateTime.UtcNow,
        };

        if (_gatewayClient != null)
        {
            var response = await _gatewayClient.PostOrder(request);
            Console.WriteLine($"Post order response: {response}");
        }
    }

    private static OrderDirection GetRandomDirection()
        => Random.Shared.Next(0, 2) == 0 ? OrderDirection.Buy : OrderDirection.Sell;
}