using Tinkoff.InvestApi.V1;

namespace Vertr.Tproxy.Server.Converters;

internal static class OperationItemTradeConverter
{
    public static IEnumerable<Client.Operations.OperationItemTrade> Convert(this IEnumerable<OperationItemTrade> trades)
        => trades.Select(Convert);

    public static Client.Operations.OperationItemTrade Convert(this OperationItemTrade trade)
        => new Client.Operations.OperationItemTrade(
            trade.Num,
            trade.Date.ToDateTime(),
            trade.Quantity,
            trade.Price.Convert(),
            trade.Yield.Convert(),
            trade.YieldRelative
            );
}
