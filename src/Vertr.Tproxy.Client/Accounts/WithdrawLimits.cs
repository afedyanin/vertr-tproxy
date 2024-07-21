namespace Vertr.Tproxy.Client.Accounts;
public record class WithdrawLimits(Money[] Cash, Money[] Blocked, Money[] BlockedGuarantee);
