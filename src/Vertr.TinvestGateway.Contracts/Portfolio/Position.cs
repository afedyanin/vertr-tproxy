using System.Text.Json;

namespace Vertr.TinvestGateway.Contracts.Portfolio;

public record struct Position
{
    public Guid InstrumentId { get; set; }

    public decimal Balance { get; set; }
}
