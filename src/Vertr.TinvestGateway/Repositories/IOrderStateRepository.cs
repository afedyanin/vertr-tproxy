using Vertr.TinvestGateway.Contracts.Orders;

namespace Vertr.TinvestGateway.Repositories;

public interface IOrderStateRepository
{
    public Task Clear();
    public Task<OrderState?> Get(Guid id);
    public Task<IEnumerable<OrderState>> GetByIds(string? orderId = null, string? requestId = null);
    public Task Save(OrderState orderState);
}
