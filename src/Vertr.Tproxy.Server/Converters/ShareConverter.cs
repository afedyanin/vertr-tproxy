using Vertr.Tproxy.Client.Instruments;

namespace Vertr.Tproxy.Server.Converters;

internal static class ShareConverter
{
    public static Share[] Convert(this IEnumerable<Tinkoff.InvestApi.V1.Share> shares)
        => shares.Select(Convert).ToArray();

    public static Share Convert(this Tinkoff.InvestApi.V1.Share share)
        => new Share(
            share.Ticker,
            share.ClassCode,
            share.Isin,
            share.Uid,
            share.Name,
            share.Sector,
            share.Lot,
            share.Currency);
}
