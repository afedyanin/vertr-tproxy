using Vertr.TinvestGateway.Contracts.Orders;

namespace Vertr.TinvestGateway.Contracts.Repositories;

public interface IOrderResponseRepository
{
    public Task Clear();
    public Task<PostOrderResponse?> Get(Guid id);
    public Task<IEnumerable<PostOrderResponse>> GetByPortfolioId(Guid portfolioId);
    public Task Save(PostOrderResponse orderResponse, Guid portfolioId);
}
