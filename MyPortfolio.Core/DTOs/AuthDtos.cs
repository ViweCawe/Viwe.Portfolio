using System.ComponentModel.DataAnnotations;
namespace MyPortfolio.Core.DTOs;
public sealed class LoginRequest
{
    [Required, StringLength(100)] public string UserName { get; set; } = "";
    [Required, StringLength(200)] public string Password { get; set; } = "";
}
public sealed record TokenResponse(string AccessToken, long ExpiresIn);
public sealed record AdminSession(string UserName);
