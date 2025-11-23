using Vertr.TinvestGateway.Contracts.MarketData;

namespace Vertr.TinvestGateway.Converters;

public static class CandleConverter
{
    public static Candle[] Convert(this Tinkoff.InvestApi.V1.HistoricCandle[] source, Guid instrumentId)
        => [.. source.Select(s => s.Convert(instrumentId))];

    public static Candle Convert(this Tinkoff.InvestApi.V1.HistoricCandle source, Guid instrumentId)
        => new Candle(
            instrumentId,
            source.Time.ToDateTime(),
            source.Open,
            source.Close,
            source.High,
            source.Low,
            source.Volume);

    public static Candle Convert(this Tinkoff.InvestApi.V1.Candle source, Guid instrumentId)
        => new Candle(
            instrumentId,
            source.Time.ToDateTime(),
            source.Open,
            source.Close,
            source.High,
            source.Low,
            source.Volume);
}
