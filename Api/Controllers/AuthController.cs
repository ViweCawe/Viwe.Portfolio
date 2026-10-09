using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MyPortfolio.Core.DTOs;
namespace MyPortfolio.Api.Controllers;
[ApiController, Route("api/auth")]
public sealed class AuthController(IConfiguration config, IPasswordHasher<string> hasher) : ControllerBase
{
    [HttpPost("login"), EnableRateLimiting("login")]
    public IActionResult Login(LoginRequest request)
    {
        var name = config["Admin:UserName"];
        var hash = config["Admin:PasswordHash"];
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(hash)) return Problem("Admin authentication has not been configured.", statusCode:503);
        var result = hasher.VerifyHashedPassword(name, hash, request.Password);
        if (result == PasswordVerificationResult.Failed || !string.Equals(name, request.UserName, StringComparison.Ordinal)) return Unauthorized();
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Name, name), new Claim(ClaimTypes.Role, "Admin"), new Claim("credential_version", Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(hash))))], BearerTokenDefaults.AuthenticationScheme);
        return SignIn(new ClaimsPrincipal(identity), BearerTokenDefaults.AuthenticationScheme);
    }
    [HttpGet("me"), Authorize(Policy="Admin")]
    public AdminSession Me() => new(User.Identity!.Name!);
}
