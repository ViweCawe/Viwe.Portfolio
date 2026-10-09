using MyPortfolio.Core.Models;
namespace MyPortfolio.Core.Interfaces;
public interface IPortfolioRepository
{
    Task<IReadOnlyList<PortfolioEntry>> ListAsync(PortfolioSection section, bool includeDrafts, CancellationToken ct);
    Task<PortfolioEntry?> GetAsync(int id, bool includeDrafts, CancellationToken ct);
    Task<int> CreateAsync(PortfolioEntry entry, CancellationToken ct);
    Task<bool> UpdateAsync(int id, PortfolioEntry entry, CancellationToken ct);
    Task<bool> ArchiveAsync(int id, CancellationToken ct);
}
