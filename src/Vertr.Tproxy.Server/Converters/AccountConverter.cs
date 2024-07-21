namespace Vertr.Tproxy.Server.Converters;

internal static class AccountConverter
{
    public static Client.Accounts.Account Convert(this Tinkoff.InvestApi.V1.Account account)
        => new Client.Accounts.Account(
            account.Id,
            account.Name,
            account.Type.ToString(),
            account.Status.ToString(),
            account.OpenedDate.ToDateTime(),
            account.ClosedDate.Convert(),
            account.AccessLevel.ToString());

    public static Client.Accounts.Account[] Convert(this IEnumerable<Tinkoff.InvestApi.V1.Account> accounts)
        => accounts.Select(Convert).ToArray();
}
