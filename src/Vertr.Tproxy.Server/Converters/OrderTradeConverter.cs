using Tinkoff.InvestApi.V1;

namespace Vertr.Tproxy.Server.Converters;

internal static class OrderTradeConverter
{
    public static Client.Orders.OrderTrade[] Convert(this OrderTrades trades)
        => trades.Trades.Select(
            t => t.Convert(
                trades.OrderId,
                trades.InstrumentUid,
                trades.Direction))
        .ToArray();

    public static Client.Orders.OrderTrade Convert(
        this OrderTrade trade,
        string orderId,
        string instrumentId,
        OrderDirection direction)
        => new Client.Orders.OrderTrade(
            orderId,
            trade.TradeId,
            trade.DateTime.ToDateTime(),
            instrumentId,
            trade.Price,
            trade.Quantity * (direction == OrderDirection.Sell ? -1 : 1));
}
