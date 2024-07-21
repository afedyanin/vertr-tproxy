namespace Vertr.Tproxy.Client.Orders;
public record class PostOrderRequest(
    string InstrumentId,
    Guid? OrderId,
    long QtyLots,
    decimal Price)
{
    public bool IsValid()
    {
        if (string.IsNullOrEmpty(InstrumentId))
        {
            return false;
        }

        if (QtyLots == 0)
        {
            return false;
        }

        return true;
    }
}
