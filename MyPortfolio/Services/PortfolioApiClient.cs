using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Authorization;
using MyPortfolio.Core.Models;
namespace MyPortfolio.Services;
public sealed class PortfolioApiClient(IHttpClientFactory factory, AuthenticationStateProvider auth)
{
    public async Task<List<PortfolioEntry>> ListAsync(PortfolioSection section, bool admin = false) {
        using var client = await Client(admin);
        return await client.GetFromJsonAsync<List<PortfolioEntry>>(admin ? $"api/admin/entries/section/{section}" : $"api/portfolio/{section}") ?? [];
    }
    public async Task<PortfolioEntry?> GetAsync(int id) { using var client = await Client(true); return await client.GetFromJsonAsync<PortfolioEntry>($"api/admin/entries/{id}"); }
    public async Task SaveAsync(PortfolioEntry entry) {
        using var client = await Client(true);
        using var response = entry.Id == 0 ? await client.PostAsJsonAsync("api/admin/entries", entry) : await client.PutAsJsonAsync($"api/admin/entries/{entry.Id}", entry);
        response.EnsureSuccessStatusCode();
    }
    public async Task ArchiveAsync(int id) { using var client = await Client(true); using var response = await client.DeleteAsync($"api/admin/entries/{id}"); response.EnsureSuccessStatusCode(); }
    async Task<HttpClient> Client(bool admin) {
        var client = factory.CreateClient("PortfolioApi");
        if (admin) {
            var token = (await auth.GetAuthenticationStateAsync()).User.FindFirst("api_token")?.Value;
            if (string.IsNullOrWhiteSpace(token)) { client.Dispose(); throw new UnauthorizedAccessException(); }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return client;
    }
}
