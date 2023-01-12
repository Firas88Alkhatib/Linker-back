
namespace Linker.Models;

public class ShortLink
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public required string OriginalUrl { get; set; }
    public required string ShortUrl { get; set; }
    public List<Click> Clicks { get; set; } = new();
}

