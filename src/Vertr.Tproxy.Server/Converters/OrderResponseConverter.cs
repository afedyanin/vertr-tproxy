namespace Vertr.Tproxy.Server.Converters;

internal static class OrderResponseConverter
{
    public static Client.Orders.PostOrderResponse Convert(this Tinkoff.InvestApi.V1.PostOrderResponse response)
    {
        var res = new Client.Orders.PostOrderResponse(
            response.OrderId,
            response.OrderRequestId,
            response.InstrumentUid,
            response.ExecutionReportStatus.Convert(),
            response.LotsRequested,
            response.LotsExecuted,
            response.ExecutedCommission.Convert(),
            response.Message,
            response.ResponseMetadata.TrackingId,
            response.ResponseMetadata.ServerTime.ToDateTime()
            );

        return res;
    }

    public static Client.Orders.OrderStateResponse[] Convert(this IEnumerable<Tinkoff.InvestApi.V1.OrderState> states)
        => states.Select(Convert).ToArray();

    public static Client.Orders.OrderStateResponse Convert(this Tinkoff.InvestApi.V1.OrderState response)
    {
        var res = new Client.Orders.OrderStateResponse(
            response.OrderId,
            response.OrderRequestId,
            response.InstrumentUid,
            response.ExecutionReportStatus.Convert(),
            response.LotsRequested,
            response.LotsExecuted,
            response.ExecutedCommission.Convert(),
            response.ServiceCommission.Convert(),
            response.OrderDate.Convert(),
            response.Currency
            );

        return res;
    }


    private static Client.Orders.OrderExecutionStatus Convert(this Tinkoff.InvestApi.V1.OrderExecutionReportStatus status)
        => status switch
        {
            Tinkoff.InvestApi.V1.OrderExecutionReportStatus.ExecutionReportStatusUnspecified => Client.Orders.OrderExecutionStatus.Unspecified,
            Tinkoff.InvestApi.V1.OrderExecutionReportStatus.ExecutionReportStatusRejected => Client.Orders.OrderExecutionStatus.Rejected,
            Tinkoff.InvestApi.V1.OrderExecutionReportStatus.ExecutionReportStatusNew => Client.Orders.OrderExecutionStatus.New,
            Tinkoff.InvestApi.V1.OrderExecutionReportStatus.ExecutionReportStatusFill => Client.Orders.OrderExecutionStatus.Fill,
            Tinkoff.InvestApi.V1.OrderExecutionReportStatus.ExecutionReportStatusCancelled => Client.Orders.OrderExecutionStatus.Cancelled,
            Tinkoff.InvestApi.V1.OrderExecutionReportStatus.ExecutionReportStatusPartiallyfill => Client.Orders.OrderExecutionStatus.PartiallyFill,
            _ => throw new InvalidOperationException($"Unknown OrderExecutionReportStatus:{status}")
        };
}
