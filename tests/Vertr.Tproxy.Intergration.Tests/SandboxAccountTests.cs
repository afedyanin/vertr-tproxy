using Tinkoff.InvestApi.V1;

namespace Vertr.Tproxy.Intergration.Tests;

[TestFixture(Category = "System", Explicit = true)]
public class SandboxAccountTests : InvestApiTestBase
{
    [Test]
    public async Task CanCreateSandboxAccount()
    {
        var request = new OpenSandboxAccountRequest()
        {
            Name = "SBTest001"
        };

        var response = await Client.Sandbox.OpenSandboxAccountAsync(request);

        Assert.That(response, Is.Not.Null);

        var accountId = response.AccountId;
        Assert.That(accountId, Is.Not.Null);

        Console.WriteLine($"Account created: accountId={accountId}");
    }

    [Test]
    public async Task CanGetAccounts()
    {
        var response = await Client.Users.GetAccountsAsync();

        Assert.That(response, Is.Not.Null);
        Assert.That(response.Accounts, Is.Not.Null);

        foreach (var account in response.Accounts)
        {
            Console.WriteLine($"Account: Id={account.Id} Name={account.Name} Type={account.Type} Status={account.Status} AccessLevel={account.AccessLevel} OpenedDate={account.OpenedDate} ClosedDate={account.ClosedDate}");
        }
    }

    [Test]
    public async Task CanGetWithdrawLimits()
    {
        var req = new WithdrawLimitsRequest()
        {
            AccountId = Mother.AccountId,
        };

        var response = await Client.Sandbox.GetSandboxWithdrawLimitsAsync(req);

        Assert.That(response, Is.Not.Null);

        foreach (var pos in response.Money)
        {
            Console.WriteLine($"Currency={pos.Currency} Units={pos.Units} Nano={pos.Nano}");
        }
    }

    [Test]
    public async Task CanDepositAmount()
    {
        var req = new SandboxPayInRequest()
        {
            AccountId = Mother.AccountId,
            Amount = new MoneyValue()
            {
                Currency = "RUB",
                Units = 500_000,
                Nano = 0,
            }

        };

        var response = await Client.Sandbox.SandboxPayInAsync(req);

        Assert.That(response, Is.Not.Null);

        Console.WriteLine($"Balance={response.Balance}");
    }
}
