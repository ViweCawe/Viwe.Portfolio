using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Core.Interfaces;
using MyPortfolio.Core.Models;
namespace MyPortfolio.Api.Controllers;
[ApiController, Route("api/portfolio")]
public sealed class PortfolioController(IPortfolioRepository repository) : ControllerBase
{
    [HttpGet("{section}")]
    public async Task<IActionResult> List(PortfolioSection section, CancellationToken ct) => Enum.IsDefined(section) ? Ok(await repository.ListAsync(section, false, ct)) : BadRequest();
}
[ApiController, Route("api/admin/entries"), Authorize(Policy="Admin")]
public sealed class AdminEntriesController(IPortfolioRepository repository) : ControllerBase
{
    [HttpGet("section/{section}")]
    public async Task<IActionResult> List(PortfolioSection section, CancellationToken ct) => Enum.IsDefined(section) ? Ok(await repository.ListAsync(section, true, ct)) : BadRequest();
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct) => await repository.GetAsync(id, true, ct) is { } item ? Ok(item) : NotFound();
    [HttpPost]
    public async Task<IActionResult> Create(PortfolioEntry item, CancellationToken ct) {
        item.Id = await repository.CreateAsync(item, ct);
        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, PortfolioEntry item, CancellationToken ct) => await repository.UpdateAsync(id, item, ct) ? NoContent() : NotFound();
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Archive(int id, CancellationToken ct) => await repository.ArchiveAsync(id, ct) ? NoContent() : NotFound();
}
