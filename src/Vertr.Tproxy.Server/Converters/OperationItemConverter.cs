using Tinkoff.InvestApi.V1;

namespace Vertr.Tproxy.Server.Converters;

internal static class OperationItemConverter
{
    public static IEnumerable<Client.Operations.OperationItem> Convert(
        this IEnumerable<OperationItem> items)
        => items.Select(Convert);

    public static Client.Operations.OperationItem Convert(
        this OperationItem item)
        => new Client.Operations.OperationItem(
            item.Id,
            item.ParentOperationId,
            item.Name,
            item.Date.ToDateTime(),
            (Client.Operations.OperationType)item.Type,
            item.InstrumentUid,
            item.Description,
            item.InstrumentType,
            (Client.Operations.InstrumentType)item.InstrumentKind,
            (Client.Operations.OperationState)item.State,
            item.Payment.Convert(),
            item.Price.Convert(),
            item.Commission.Convert(),
            item.Yield.Convert(),
            item.YieldRelative,
            item.AccruedInt.Convert(),
            item.Quantity,
            item.QuantityRest,
            item.QuantityDone,
            item.CancelDateTime.ToDateTime(),
            item.CancelReason,
            item.TradesInfo.Trades.Convert()
            );
}
