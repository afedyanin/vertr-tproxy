namespace Vertr.Tproxy.Client.Orders;
public record class OrderTrade(
    string OrderId,
    string TradeId,
    DateTime Timestamp,
    string InstrumentId,
    decimal Price,
    long Qty);
