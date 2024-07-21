using Tinkoff.InvestApi.V1;
using Vertr.Tproxy.Client.Accounts;

namespace Vertr.Tproxy.Server.Converters;

internal static class WithdrawLimitsConverter
{
    public static WithdrawLimits Convert(this WithdrawLimitsResponse response)
    {
        var cash = response.Money.Convert();
        var blocked = response.Blocked.Convert();
        var blokedGarant = response.BlockedGuarantee.Convert();
        return new WithdrawLimits(cash, blocked, blokedGarant);
    }
}
