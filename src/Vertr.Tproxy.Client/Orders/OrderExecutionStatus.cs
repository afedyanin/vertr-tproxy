namespace Vertr.Tproxy.Client.Orders;
public enum OrderExecutionStatus
{
    Unspecified = 0,
    Fill = 1,
    Rejected = 2,
    Cancelled = 3,
    New = 4,
    PartiallyFill = 5,
}
