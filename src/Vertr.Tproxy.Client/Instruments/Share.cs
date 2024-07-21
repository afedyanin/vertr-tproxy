namespace Vertr.Tproxy.Client.Instruments;
public record class Share(
    string Ticker,
    string ClassCode,
    string Isin,
    string Uid,
    string Name,
    string Sector,
    int Lot,
    string Currency);
