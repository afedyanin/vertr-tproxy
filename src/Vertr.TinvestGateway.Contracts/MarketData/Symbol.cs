using System.Text.Json.Serialization;

namespace Vertr.TinvestGateway.Contracts.MarketData;

public record class Symbol
{
    public string ClassCode { get; init; } = string.Empty;

    public string Ticker { get; init; } = string.Empty;

    public override string ToString()
        => $"{ClassCode}.{Ticker}";

    [JsonConstructor]
    private Symbol() { }

    public Symbol(
        string classCode,
        string ticker)
    {
        ClassCode = classCode;
        Ticker = ticker;
    }
}
