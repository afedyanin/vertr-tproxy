using System.Text;
using Tinkoff.InvestApi.V1;

namespace Vertr.Tproxy.Server.Formatters;

internal sealed class OperationsFormatter
{
    private readonly OperationsResponse _operations;
    private readonly PortfolioResponse _portfolio;
    private readonly PositionsResponse _positions;
    private readonly WithdrawLimitsResponse _withdrawLimits;

    public OperationsFormatter(PortfolioResponse portfolio, PositionsResponse positions,
        OperationsResponse operations, WithdrawLimitsResponse withdrawLimits)
    {
        _portfolio = portfolio;
        _positions = positions;
        _operations = operations;
        _withdrawLimits = withdrawLimits;
    }

    public string Format()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Portfolio:")
            .AppendFormat("- Shares: {0} {1}", (decimal)_portfolio.TotalAmountShares,
                _portfolio.TotalAmountShares.Currency)
            .AppendLine()
            .AppendFormat("- Bonds: {0} {1}", (decimal)_portfolio.TotalAmountBonds,
                _portfolio.TotalAmountBonds.Currency)
            .AppendLine()
            .AppendFormat("- Etf: {0} {1}", (decimal)_portfolio.TotalAmountEtf,
                _portfolio.TotalAmountEtf.Currency)
            .AppendLine()
            .AppendFormat("- Currencies: {0} {1}", (decimal)_portfolio.TotalAmountCurrencies,
                _portfolio.TotalAmountCurrencies.Currency)
            .AppendLine()
            .AppendFormat("- Futures: {0} {1}", (decimal)_portfolio.TotalAmountFutures,
                _portfolio.TotalAmountFutures.Currency)
            .AppendLine()
            .AppendFormat("- Expected yield: {0}", (decimal)_portfolio.ExpectedYield)
            .AppendLine()
            .AppendLine();

        if (_withdrawLimits.Money.Count != 0)
        {
            stringBuilder.AppendLine().AppendLine("Withdraw limits:");
            foreach (var value in _withdrawLimits.Money)
            {
                stringBuilder.AppendFormat("- {0} {1}", (decimal)value, value.Currency)
                    .AppendLine();
            }
        }

        if (_positions.Securities.Count != 0)
        {
            stringBuilder.AppendLine().AppendLine("Positions:");
            foreach (var security in _positions.Securities)
            {
                stringBuilder.AppendFormat("- [{0}] {1}", security.Figi, security.Balance)
                    .AppendLine();
            }
        }

        if (_operations.Operations.Count != 0)
        {
            stringBuilder.AppendLine().AppendLine("Operations:");
            foreach (var operation in _operations.Operations)
            {
                stringBuilder.AppendFormat("- [{0}] {1} {2} {3}", operation.Figi, operation.Date,
                        (decimal)operation.Payment, operation.Currency)
                    .AppendLine();
            }
        }

        return stringBuilder.ToString();
    }
}
