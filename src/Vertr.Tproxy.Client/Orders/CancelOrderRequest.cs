namespace Vertr.Tproxy.Client.Orders;
public record class CancelOrderRequest(string OrderId)
{
    public bool IsValid()
    {
        if (string.IsNullOrEmpty(OrderId))
        {
            return false;
        }

        return true;
    }
}
