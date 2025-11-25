using Vertr.TinvestGateway.Contracts.Orders;

namespace Vertr.TinvestGateway.Contracts.Repositories;

public interface IOrderRequestRepository
{
    public Task Clear();
    public Task<PostOrderRequest?> Get(Guid id);
    public Task<IEnumerable<PostOrderRequest>> GetByPortfolioId(Guid portfolioId);
    public Task Save(PostOrderRequest orderRequest, Guid portfolioId);
}
