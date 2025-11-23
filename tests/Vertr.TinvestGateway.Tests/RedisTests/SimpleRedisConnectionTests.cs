using StackExchange.Redis;

namespace Vertr.TinvestGateway.Tests.RedisTests;

// https://stackexchange.github.io/StackExchange.Redis/Basics
public class SimpleRedisConnectionTests
{
    [Test]
    public async Task CanConnectToRedis()
    {
        var redis = ConnectionMultiplexer.Connect("localhost");
        var db = redis.GetDatabase();

        var value = "abcdefg";
        await db.StringSetAsync("mykey", value);

        var saved = await db.StringGetAsync("mykey");
        Console.WriteLine(saved);

        Assert.Pass();
    }

    [Test]
    public async Task CanUsePubSub()
    {
        var redis = ConnectionMultiplexer.Connect("localhost");
        var sub = redis.GetSubscriber();

        sub.Subscribe("messages").OnMessage(async channelMessage => 
        {
            await Task.Delay(1000);
            Console.WriteLine(channelMessage.Message);
        });

        sub.Publish("messages", "hello");

        await Task.Delay(5000);

        Assert.Pass();
    }
}
