namespace Linker.Models;

public class Click
{
    public int Id { get; set; }
    public int ShortLinkId { get; set; }
    public required ShortLink ShortLink { get; set; }
    public required DateTime ClickedAt { get; set; }
}
