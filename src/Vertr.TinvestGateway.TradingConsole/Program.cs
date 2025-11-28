using Refit;
using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts;
using Vertr.TinvestGateway.Contracts.MarketData;
using Vertr.TinvestGateway.Contracts.Orders;
using Vertr.TinvestGateway.Contracts.Orders.Enums;
using static StackExchange.Redis.RedisChannel;

namespace Vertr.TinvestGateway.TradingConsole;

internal class Program
{
    private static string _accountId = "1b6184e4-0b7e-493c-b1c8-26094fbf2940";
    private static Guid _instrumentId = new Guid("e6123145-9665-43e0-8413-cd61b8aa9b13");
    private static Guid _portfolioId = new Guid("73C4E51B-B0C9-4D5F-999E-171140E79C54");

    private static volatile bool _isCancelled = false;
    private static ConnectionMultiplexer? _connection;
    private static ISubscriber? _subscriber;
    private static ITinvestGatewayClient _gatewayClient;

    public static async Task Main(string[] args)
    {
        _connection = ConnectionMultiplexer.Connect("localhost");
        _subscriber = _connection.GetSubscriber();
        _gatewayClient = RestService.For<ITinvestGatewayClient>("https://localhost:7132");

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
            Thread.Sleep(20000); 
        }

        Console.WriteLine("Application exiting gracefully after cleanup.");
    }

    private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs args)
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
            AccountId = _accountId, // Get from settings
            InstrumentId = _instrumentId, // Get from channel name
            PortfolioId = _portfolioId, // Get From Strategy Dictionary
            OrderDirection = GetRandomDirection(),
            OrderType = OrderType.Market,
            TimeInForceType = TimeInForceType.Unspecified,
            PriceType = PriceType.Unspecified,
            Price = 0.0m,
            QuantityLots = 10,
            CreatedAt = DateTime.UtcNow,
        };

        var response = await _gatewayClient.PostOrder(request);
        Console.WriteLine($"Post order response: {response}");
    }

    private static OrderDirection GetRandomDirection()
        => Random.Shared.Next(0, 2) == 0 ? OrderDirection.Buy : OrderDirection.Sell; 
}
