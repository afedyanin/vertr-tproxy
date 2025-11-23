using Vertr.TinvestGateway.Host.BackgroundServices;

namespace Vertr.TinvestGateway.Host;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configuration = builder.Configuration;
        //var connectionString = configuration.GetConnectionString(_connStringName);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add modules
        builder.Services.AddTinvestGateways(configuration);

        builder.Services.AddHostedService<MarketDataStreamService>();
        builder.Services.AddHostedService<OrderTradesStreamService>();
        builder.Services.AddHostedService<OrderStateStreamService>();


        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
