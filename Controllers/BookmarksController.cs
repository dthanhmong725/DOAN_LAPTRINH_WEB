using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DOAN_LAPTRINHWEB.Interfaces;
using DOAN_LAPTRINHWEB.Models.DTOs;

namespace DOAN_LAPTRINHWEB.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookmarksController : ControllerBase
{
    private readonly IBookmarkService _bookmarkService;

    public BookmarksController(IBookmarkService bookmarkService)
    {
        _bookmarkService = bookmarkService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserBookmarks([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _bookmarkService.GetUserBookmarksAsync(userId, page, pageSize);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleBookmark([FromBody] ToggleBookmarkDto dto)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _bookmarkService.ToggleBookmarkAsync(userId, dto.PostId);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("{postId}")]
    public async Task<IActionResult> AddBookmark(int postId)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _bookmarkService.AddBookmarkAsync(userId, postId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{postId}")]
    public async Task<IActionResult> RemoveBookmark(int postId)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _bookmarkService.RemoveBookmarkAsync(userId, postId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("check/{postId}")]
    public async Task<IActionResult> IsBookmarked(int postId)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _bookmarkService.IsBookmarkedAsync(userId, postId);
        return Ok(result);
    }
}
