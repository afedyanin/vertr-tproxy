using Tinkoff.InvestApi.V1;
using Vertr.Infrastructure.Kafka;
using Vertr.Tproxy.Server.Application;
using Vertr.Tproxy.Server.BackgroundServices;
using Vertr.Tproxy.Server.Configuration;

namespace Vertr.Tproxy.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOptions<TproxySettings>().BindConfiguration("TproxySettings");
        builder.Services.AddOptions<KafkaSettings>().BindConfiguration("KafkaSettings");

        builder.Services.AddInvestApiClient((_, settings) => builder.Configuration.Bind("TproxySettings:InvestApiSettings", settings));
        builder.Services.AddKafkaSettings(settings => builder.Configuration.Bind("KafkaSettings", settings));
        builder.Services.AddKafkaProducer<string, Client.Orders.OrderTrade>();
        builder.Services.AddKafkaProducer<string, Trade>();
        builder.Services.AddKafkaProducer<string, Candle>();
        builder.Services.AddKafkaProducer<string, OrderBook>();
        builder.Services.AddKafkaProducer<string, LastPrice>();
        builder.Services.AddKafkaProducer<string, TradingStatus>();
        builder.Services.AddSingleton<MarketDataStreamProvider>();

        // builder.Services.AddHostedService<MarketDataStreamService>();
        // builder.Services.AddHostedService<OrderStateStreamService>();
        builder.Services.AddHostedService<TradesStreamService>();
        // builder.Services.AddHostedService<BackgroundServices.MarketDataStreamService>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
