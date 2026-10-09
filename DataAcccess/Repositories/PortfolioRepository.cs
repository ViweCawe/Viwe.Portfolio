using System.Data;
using Dapper;
using MyPortfolio.Core.Interfaces;
using MyPortfolio.Core.Models;
using MyPortfolio.DataAccess.Connections;
namespace MyPortfolio.DataAccess.Repositories;
public sealed class PortfolioRepository(SqlConnectionFactory factory) : IPortfolioRepository
{
    static CommandDefinition Command(string name, object args, CancellationToken ct) => new(name, args, commandType: CommandType.StoredProcedure, cancellationToken: ct);
    static object Values(PortfolioEntry e) => new { e.Section, e.Title, e.Summary, e.Body, e.Category, Url = string.IsNullOrWhiteSpace(e.Url) ? null : e.Url, e.DisplayOrder, e.IsPublished };
    public async Task<IReadOnlyList<PortfolioEntry>> ListAsync(PortfolioSection section, bool includeDrafts, CancellationToken ct)
    {
        await using var db = factory.Create();
        return (await db.QueryAsync<PortfolioEntry>(Command("dbo.Portfolio_List", new { Section = section, IncludeDrafts = includeDrafts }, ct))).AsList();
    }
    public async Task<PortfolioEntry?> GetAsync(int id, bool includeDrafts, CancellationToken ct)
    {
        await using var db = factory.Create();
        return await db.QuerySingleOrDefaultAsync<PortfolioEntry>(Command("dbo.Portfolio_Get", new { Id = id, IncludeDrafts = includeDrafts }, ct));
    }
    public async Task<int> CreateAsync(PortfolioEntry e, CancellationToken ct)
    {
        await using var db = factory.Create();
        return await db.QuerySingleAsync<int>(Command("dbo.Portfolio_Create", Values(e), ct));
    }
    public async Task<bool> UpdateAsync(int id, PortfolioEntry e, CancellationToken ct)
    {
        await using var db = factory.Create();
        var args = new DynamicParameters(Values(e)); args.Add("Id", id);
        return await db.QuerySingleAsync<int>(Command("dbo.Portfolio_Update", args, ct)) == 1;
    }
    public async Task<bool> ArchiveAsync(int id, CancellationToken ct)
    {
        await using var db = factory.Create();
        return await db.QuerySingleAsync<int>(Command("dbo.Portfolio_Archive", new { Id = id }, ct)) == 1;
    }
}
