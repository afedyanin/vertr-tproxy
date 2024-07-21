using System.Text;
using Tinkoff.InvestApi.V1;

namespace Vertr.Tproxy.Server.Formatters;

internal sealed class InstrumentsFormatter
{
    private readonly IReadOnlyList<GetAccruedInterestsResponse> _accruedInterests;
    private readonly IReadOnlyList<Bond> _bonds;
    private readonly IReadOnlyList<GetDividendsResponse> _dividends;
    private readonly IReadOnlyList<Etf> _etfs;
    private readonly IReadOnlyList<Future> _futures;
    private readonly IReadOnlyList<GetFuturesMarginResponse> _futuresMargin;
    private readonly IReadOnlyList<Share> _shares;
    private readonly TradingSchedulesResponse _tradingSchedulesResponse;

    public InstrumentsFormatter(IReadOnlyList<Share> shares, IReadOnlyList<Etf> etfs,
        IReadOnlyList<Bond> bonds, IReadOnlyList<Future> futures,
        IReadOnlyList<GetDividendsResponse> dividends,
        IReadOnlyList<GetAccruedInterestsResponse> accruedInterests,
        IReadOnlyList<GetFuturesMarginResponse> futuresMargin, TradingSchedulesResponse tradingSchedulesResponse)
    {
        _shares = shares;
        _etfs = etfs;
        _bonds = bonds;
        _futures = futures;
        _dividends = dividends;
        _accruedInterests = accruedInterests;
        _futuresMargin = futuresMargin;
        _tradingSchedulesResponse = tradingSchedulesResponse;
    }

    public string Format()
    {
        var stringBuilder = new StringBuilder();

        foreach (var tradingSchedule in _tradingSchedulesResponse.Exchanges)
        {
            stringBuilder.AppendFormat("Trading schedule for exchange {0}: ", tradingSchedule.Exchange)
                .AppendLine();
            foreach (var tradingDay in tradingSchedule.Days)
            {
                stringBuilder.AppendFormat("- {0} {1:working;0;non-working} {2} {3}", tradingDay.Date,
                        tradingDay.IsTradingDay.GetHashCode(), tradingDay.StartTime, tradingDay.EndTime)
                    .AppendLine();
            }
        }

        stringBuilder.AppendFormat("Loaded {0} shares", _shares.Count)
            .AppendLine();

        for (var i = 0; i < 10; i++)
        {
            var share = _shares[i];
            stringBuilder.AppendFormat("- [{0}] {1}", share.Figi, share.Name)
                .AppendLine();
            if (i < _dividends.Count)
            {
                var dividendsCount = Math.Min(10, _dividends[i].Dividends.Count);

                if (dividendsCount == 0)
                {
                    continue;
                }

                stringBuilder.AppendFormat("  Dividends:").AppendLine();
                for (var j = 0; j < dividendsCount; j++)
                {
                    var dividend = _dividends[i].Dividends[j];
                    stringBuilder.AppendFormat("  - {0} {1} {2}", (decimal)dividend.DividendNet,
                            dividend.DividendType, dividend.DeclaredDate)
                        .AppendLine();
                }
            }
        }

        stringBuilder.AppendLine("...").AppendLine();

        stringBuilder.AppendFormat("Loaded {0} etfs", _etfs.Count)
            .AppendLine();
        for (var i = 0; i < 10; i++)
        {
            var etf = _etfs[i];
            stringBuilder.AppendFormat("- [{0}] {1}", etf.Figi, etf.Name)
                .AppendLine();
        }

        stringBuilder.AppendLine("...").AppendLine();

        stringBuilder.AppendFormat("Loaded {0} bonds", _bonds.Count)
            .AppendLine();
        for (var i = 0; i < 10; i++)
        {
            var bond = _bonds[i];
            stringBuilder.AppendFormat("- [{0}] {1}", bond.Figi, bond.Name)
                .AppendLine();

            if (i < _accruedInterests.Count)
            {
                stringBuilder.AppendFormat("  Accrued Interest:").AppendLine();
                var accruedInterestsCount = Math.Min(_accruedInterests[i].AccruedInterests.Count, 10);
                for (var j = 0; j < accruedInterestsCount; j++)
                {
                    var accruedInterest = _accruedInterests[i].AccruedInterests[j];
                    stringBuilder.AppendFormat("  - {0} {1}", accruedInterest.Date,
                            (decimal)accruedInterest.Nominal)
                        .AppendLine();
                }
            }
        }

        stringBuilder.AppendLine("...").AppendLine();

        stringBuilder.AppendFormat("Loaded {0} futures", _futures.Count)
            .AppendLine();
        for (var i = 0; i < 10; i++)
        {
            var future = _futures[i];
            stringBuilder.AppendFormat("- [{0}] {1}", future.Figi, future.Name)
                .AppendLine();

            if (i < _futuresMargin.Count)
            {
                stringBuilder.AppendFormat("  Initial Margin On Buy: {0}",
                    (decimal)_futuresMargin[i].InitialMarginOnBuy).AppendLine();
                stringBuilder.AppendFormat("  Initial Margin On Sell: {0}",
                    (decimal)_futuresMargin[i].InitialMarginOnSell).AppendLine();
            }
        }

        stringBuilder.AppendLine("...");

        return stringBuilder.ToString();
    }
}
