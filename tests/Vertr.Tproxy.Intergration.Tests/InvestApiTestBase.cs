using Tinkoff.InvestApi;

namespace Vertr.Tproxy.Intergration.Tests;

public abstract class InvestApiTestBase
{
    // TODO: Clear token before comit!
    private const string _token = "";

    protected InvestApiClient Client { get; private set; }

    protected InvestApiTestBase()
    {
        Client = InvestApiClientFactory.Create(_token, sandbox: true);
    }
}
