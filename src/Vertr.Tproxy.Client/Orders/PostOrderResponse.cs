namespace Vertr.Tproxy.Client.Orders;

public record class PostOrderResponse(
    string OrderId,
    string OrderRequestId,
    string InstrumentUid,
    OrderExecutionStatus Status,
    long LotsRequested,
    long LotsExecuted,
    Money ExecutedCommission,
    string Message,
    string TrackingId,
    DateTime ServerTime
);

