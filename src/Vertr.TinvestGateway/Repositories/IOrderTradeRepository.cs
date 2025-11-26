using Vertr.TinvestGateway.Contracts.Orders;

namespace Vertr.TinvestGateway.Repositories;

public interface IOrderTradeRepository
{
    public Task Clear();
    public Task<OrderTrades?> Get(Guid id);
    public Task<IEnumerable<OrderTrades>> GetByOrderId(string orderId);
    public Task Save(OrderTrades orderTrades);
}
