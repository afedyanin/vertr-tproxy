using Tinkoff.InvestApi.V1;

namespace Vertr.Tproxy.Server.Converters;

internal static class GetOperationsResponseConvertor
{
    public static Client.Operations.GetOperationsResponse Convert(
        this GetOperationsByCursorResponse response)
        => new Client.Operations.GetOperationsResponse(
            response.HasNext,
            response.NextCursor,
            response.Items.Convert());
}
