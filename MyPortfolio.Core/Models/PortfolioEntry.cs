using System.ComponentModel.DataAnnotations;
namespace MyPortfolio.Core.Models;
public enum PortfolioSection { About, Skills, Projects, Services }
public class PortfolioEntry
{
    public int Id { get; set; }
    [EnumDataType(typeof(PortfolioSection))] public PortfolioSection Section { get; set; }
    [Required, StringLength(120)] public string Title { get; set; } = "";
    [Required, StringLength(500)] public string Summary { get; set; } = "";
    [StringLength(6000)] public string Body { get; set; } = "";
    [StringLength(120)] public string Category { get; set; } = "";
    [HttpUrl, StringLength(500)] public string? Url { get; set; }
    [Range(0, 9999)] public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
public sealed class HttpUrlAttribute : ValidationAttribute
{
    public override bool IsValid(object? value) => value is null || string.IsNullOrWhiteSpace(value.ToString()) ||
        (Uri.TryCreate(value.ToString(), UriKind.Absolute, out var uri) && (uri.Scheme == "https" || uri.Scheme == "http"));
}
