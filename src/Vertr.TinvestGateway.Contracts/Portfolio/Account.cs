namespace Vertr.TinvestGateway.Contracts.Portfolio;
public record class Account(
    string Id,
    string Name,
    string AccessLevel,
    string Status,
    string AccountType,
    DateTime OpenedDate,
    DateTime? ClosedDate);

