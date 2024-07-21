using Tinkoff.InvestApi;

namespace Vertr.Tproxy.Server.Configuration;

public class TproxySettings
{
    public InvestApiSettings? InvestApiSettings { get; set; }

    public string AccountId { get; set; } = string.Empty;
}
