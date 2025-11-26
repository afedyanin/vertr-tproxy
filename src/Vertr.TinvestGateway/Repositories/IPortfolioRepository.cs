using Vertr.TinvestGateway.Contracts.Portfolio;

namespace Vertr.TinvestGateway.Repositories;

public interface IPortfolioRepository
{
    public Task<Portfolio?> Get(Guid id);

    public Task Save(Portfolio portfolio);
}
