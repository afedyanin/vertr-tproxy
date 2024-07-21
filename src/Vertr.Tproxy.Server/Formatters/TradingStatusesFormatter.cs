using System.Text;
using Tinkoff.InvestApi.V1;

namespace Vertr.Tproxy.Server.Formatters;

internal sealed class TradingStatusesFormatter
{
    private readonly GetTradingStatusesResponse _tradingStatusesResponse;

    public TradingStatusesFormatter(GetTradingStatusesResponse tradingStatusesResponse)
    {
        _tradingStatusesResponse = tradingStatusesResponse;
    }

    public string Format()
    {
        var builder = new StringBuilder();

        if (_tradingStatusesResponse.TradingStatuses.Count != 0)
        {
            builder.AppendLine().AppendLine("TradingStatuses:");
            foreach (var status in _tradingStatusesResponse.TradingStatuses)
            {
                builder.Append($"[{status}]");
            }
        }
        else
        {
            builder.Append("No trading statuses exists in response");
        }

        return builder.ToString();
    }
}
