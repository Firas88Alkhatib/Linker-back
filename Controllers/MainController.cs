using Linker.Data;
using Linker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Linker.Controllers;

[Route("")]
[ApiController]
public class MainController : ControllerBase
{
    private readonly ApplicationDb _db;
    public MainController(ApplicationDb db)
    {
        _db = db;
    }

    [HttpGet("{link}")]
    public async Task<ActionResult> Get(string link = "")
    {
        if (String.IsNullOrEmpty(link))
        {
            return NotFound();
        }

        var shortLink = await _db.ShortLinks.FirstOrDefaultAsync(l => l.ShortUrl == link);

        if (shortLink is null)
        {
            return NotFound();
        }

        var newClick = new Click { 
            ShortLink= shortLink,
            ClickedAt=DateTime.UtcNow
        };
        await _db.AddAsync(newClick);
        await _db.SaveChangesAsync();

        return Redirect(shortLink.OriginalUrl);
    }
}
