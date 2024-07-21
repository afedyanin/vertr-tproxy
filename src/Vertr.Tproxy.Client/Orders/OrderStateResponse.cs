namespace Vertr.Tproxy.Client.Orders;

public record class OrderStateResponse(
    string OrderId,
    string OrderRequestId,
    string InstrumentUid,
    OrderExecutionStatus Status,
    long LotsRequested,
    long LotsExecuted,
    Money ExecutedCommission,
    Money ServiceCommission,
    DateTime? orderDate,
    string currency
);

