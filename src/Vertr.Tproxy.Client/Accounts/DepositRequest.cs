namespace Vertr.Tproxy.Client.Accounts;
public record class DepositRequest(string AccountId, Money Amount);
