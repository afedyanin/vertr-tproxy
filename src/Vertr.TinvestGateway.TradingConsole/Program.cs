using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts.MarketData;
using Vertr.TinvestGateway.Contracts.Portfolio;
using static StackExchange.Redis.RedisChannel;

namespace Vertr.TinvestGateway.TradingConsole;

internal class Program
{
    private static volatile bool _isCancelled = false;
    private static ConnectionMultiplexer? _connection;
    private static ISubscriber? _subscriber;

    public static async Task Main(string[] args)
    {
        _connection = ConnectionMultiplexer.Connect("localhost");
        _subscriber = _connection.GetSubscriber();

        var candlesChannel = new RedisChannel("market.candles*", PatternMode.Pattern);
        await _subscriber.SubscribeAsync(candlesChannel, (ch, message) =>
        {
            var candle = Candlestick.FromJson(message.ToString());
            Console.WriteLine($"Received candle: {ch} - {candle}");
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

        _isCancelled = true; // Signal to the main loop to exit
    }
}
