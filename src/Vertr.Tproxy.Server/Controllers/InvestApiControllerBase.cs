using Microsoft.AspNetCore.Mvc;
using Tinkoff.InvestApi;

namespace Vertr.Tproxy.Server.Controllers;

public abstract class InvestApiControllerBase : ControllerBase
{
    protected InvestApiClient InvestApi { get; private set; }

    protected InvestApiControllerBase(InvestApiClient investApi)
    {
        InvestApi = investApi;
    }
}
