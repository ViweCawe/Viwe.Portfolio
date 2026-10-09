using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using MyPortfolio.Core.Interfaces;
using MyPortfolio.DataAccess.Connections;
using MyPortfolio.DataAccess.Repositories;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton(new SqlConnectionFactory(builder.Configuration.GetConnectionString("Portfolio") ?? ""));
builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();
builder.Services.AddSingleton<IPasswordHasher<string>, PasswordHasher<string>>();
builder.Services.AddAuthentication(BearerTokenDefaults.AuthenticationScheme).AddBearerToken(o => o.BearerTokenExpiration = TimeSpan.FromHours(1));
builder.Services.AddAuthorization(o => o.AddPolicy("Admin", p => p.RequireAuthenticatedUser().RequireRole("Admin").RequireAssertion(ctx => ctx.User.FindFirst("credential_version")?.Value == Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(builder.Configuration["Admin:PasswordHash"] ?? ""))))));
builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = 429;
    o.AddPolicy("login", context => RateLimitPartition.GetFixedWindowLimiter(context.Connection.RemoteIpAddress?.ToString() ?? "unknown", _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
});
var app = builder.Build();
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/health", () => Results.Ok(new { status = "alive" }));
// One-time local utility. Run with --hash-password and enter the password without putting it in shell history.
if (args.Contains("--hash-password"))
{
    Console.Write("Password (12+ characters): ");
    var password = "";
    while (true) { var key = Console.ReadKey(true); if (key.Key == ConsoleKey.Enter) break; if (key.Key == ConsoleKey.Backspace) { if (password.Length > 0) password = password[..^1]; } else if (!char.IsControl(key.KeyChar)) password += key.KeyChar; }
    if (password.Length < 12) throw new InvalidOperationException("Use at least 12 characters.");
    Console.WriteLine(); Console.WriteLine(new PasswordHasher<string>().HashPassword("admin", password)); return;
}
app.MapControllers();
app.Run();
public partial class Program { }
