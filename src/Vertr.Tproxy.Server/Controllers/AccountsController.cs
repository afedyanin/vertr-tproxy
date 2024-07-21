using Microsoft.AspNetCore.Mvc;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;
using Vertr.Tproxy.Server.Converters;

namespace Vertr.Tproxy.Server.Controllers;

[Route("api/accounts")]
[ApiController]
public class AccountsController : InvestApiControllerBase
{
    public AccountsController(InvestApiClient investApi) : base(investApi)
    {
    }

    [HttpGet()]
    public async Task<ActionResult<Client.Accounts.Account[]>> GetAccounts()
    {
        var request = new GetAccountsRequest();
        var response = await InvestApi.Users.GetAccountsAsync(request);

        var accounts = response.Accounts.Convert();

        return Ok(accounts);
    }
}
