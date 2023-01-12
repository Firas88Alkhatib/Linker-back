using Linker.Data;
using Linker.DTOs;
using Linker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Linker.Controllers;

[Authorize]
public class LinksController : ODataController
{
    private readonly ApplicationDb _db;
    private readonly RandomService _randomService;
    public LinksController(ApplicationDb db, RandomService randomService)
    {
        _db = db;
        _randomService = randomService;
    }

    [EnableQuery]
    public IQueryable<ShortLink> Get()
    {
        var userId = User.FindFirst("user_id").Value;
        return _db.ShortLinks.Where(s=>s.UserId == userId);
    }

    [EnableQuery]
    public SingleResult<ShortLink> Get([FromODataUri] int key)
    {
        var userId = User.FindFirst("user_id").Value;
        var result = _db.ShortLinks.Where(l => l.Id == key && l.UserId==userId);
        return SingleResult.Create(result);
    }

    public async Task<ActionResult<ShortLink>> Post([FromBody] NewLinkDTO newLinkDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var userId = User.FindFirst("user_id").Value;
        var newLink = new ShortLink
        {
            UserId= userId,
            OriginalUrl = newLinkDto.OriginalUrl,
            ShortUrl = _randomService.GetRandomUrl()
        };
        await _db.ShortLinks.AddAsync(newLink);
        await _db.SaveChangesAsync();

        return Created(newLink);
    }

    public async Task<ActionResult> Delete(int key)
    {
        var userId = User.FindFirst("user_id").Value;
        var shortlink = _db.ShortLinks.FirstOrDefault(l => l.Id == key && l.UserId == userId);
        if (shortlink == null)
        {
            return NotFound();
        }

        _db.ShortLinks.Remove(shortlink);
        await _db.SaveChangesAsync();

        return NoContent();
    }

}
