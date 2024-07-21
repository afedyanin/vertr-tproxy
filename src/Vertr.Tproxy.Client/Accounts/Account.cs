namespace Vertr.Tproxy.Client.Accounts;

public record class Account(string Id, string Name, string Type, string Status, DateTime OpenedTime, DateTime? ClosedTime, string AccessLevel);
