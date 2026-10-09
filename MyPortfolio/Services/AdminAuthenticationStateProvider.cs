using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
namespace MyPortfolio.Services;
public sealed class AdminAuthenticationStateProvider(ILoggerFactory loggerFactory) : RevalidatingServerAuthenticationStateProvider(loggerFactory)
{
    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(1);
    protected override Task<bool> ValidateAuthenticationStateAsync(AuthenticationState state, CancellationToken ct)
        => Task.FromResult(long.TryParse(state.User.FindFirst("expires_at")?.Value, out var expiry) && expiry > DateTimeOffset.UtcNow.ToUnixTimeSeconds());
}
