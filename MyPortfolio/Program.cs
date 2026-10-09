using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using MyPortfolio.Components;
using MyPortfolio.Core.DTOs;
using MyPortfolio.Services;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, AdminAuthenticationStateProvider>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o =>
{
    o.LoginPath = "/account/login"; o.AccessDeniedPath = "/account/login";
    o.Cookie.Name = "ViwePortfolio.Admin"; o.Cookie.HttpOnly = true; o.Cookie.SameSite = SameSiteMode.Strict;
    o.Cookie.SecurePolicy = CookieSecurePolicy.Always; o.SlidingExpiration = false;
});
builder.Services.AddAuthorization(o => o.AddPolicy("Admin", p => p.RequireAuthenticatedUser().RequireRole("Admin")));
builder.Services.AddHttpClient("PortfolioApi", client => { client.BaseAddress = new Uri(builder.Configuration["PortfolioApi:BaseUrl"] ?? "https://localhost:7280/"); client.Timeout = TimeSpan.FromSeconds(15); });
builder.Services.AddScoped<PortfolioApiClient>();
var app = builder.Build();
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Error", createScopeForErrors: true); app.UseHsts(); }
app.UseHttpsRedirection();
app.UseAuthentication(); app.UseAuthorization(); app.UseAntiforgery();
app.MapPost("/account/sign-in", async (HttpContext context, IAntiforgery antiforgery, IHttpClientFactory factory) =>
{
    try { await antiforgery.ValidateRequestAsync(context); } catch (AntiforgeryValidationException) { return Results.BadRequest(); }
    var form = await context.Request.ReadFormAsync();
    var name = form["username"].ToString(); var password = form["password"].ToString();
    if (name.Length is < 1 or > 100 || password.Length is < 1 or > 200) return Results.LocalRedirect("/account/login?error=1");
    try
    {
        using var client = factory.CreateClient("PortfolioApi");
        using var response = await client.PostAsJsonAsync("api/auth/login", new LoginRequest { UserName = name, Password = password });
        if (!response.IsSuccessStatusCode) return Results.LocalRedirect("/account/login?error=1");
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var token = json.RootElement.GetProperty("accessToken").GetString()!;
        var seconds = json.RootElement.GetProperty("expiresIn").GetInt64();
        client.DefaultRequestHeaders.Authorization = new("Bearer", token);
        var verified = await client.GetFromJsonAsync<AdminSession>("api/auth/me");
        if (verified is null) return Results.LocalRedirect("/account/login?error=1");
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Name, verified.UserName), new Claim(ClaimTypes.Role, "Admin"), new Claim("api_token", token), new Claim("expires_at", DateTimeOffset.UtcNow.AddSeconds(seconds).ToUnixTimeSeconds().ToString())], CookieAuthenticationDefaults.AuthenticationScheme);
        await context.SignInAsync(new ClaimsPrincipal(identity), new AuthenticationProperties { ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(seconds), IsPersistent = false });
        return Results.LocalRedirect("/admin");
    }
    catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException) { return Results.LocalRedirect("/account/login?error=1"); }
});
app.MapPost("/account/logout", async (HttpContext context, IAntiforgery antiforgery) =>
{
    try { await antiforgery.ValidateRequestAsync(context); } catch (AntiforgeryValidationException) { return Results.BadRequest(); }
    await context.SignOutAsync(); return Results.LocalRedirect("/");
});
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();
