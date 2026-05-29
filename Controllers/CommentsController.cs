using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DOAN_LAPTRINHWEB.Interfaces;
using DOAN_LAPTRINHWEB.Models.DTOs;
using DOAN_LAPTRINHWEB.Models.Entities;

namespace DOAN_LAPTRINHWEB.Controllers;

[ApiController]
[Route("api/posts/{postId}/comments")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetByPost(int postId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        int? userId = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        }

        var result = await _commentService.GetByPostAsync(postId, page, pageSize, userId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(int postId, [FromBody] CreateCommentDto dto)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _commentService.CreateAsync(postId, dto, userId);

        if (!result.Success)
            return BadRequest(result);

        return Created($"/api/posts/{postId}/comments/{result.Data?.Id}", result);
    }

    [HttpPut("{commentId}")]
    public async Task<IActionResult> Update(int commentId, [FromBody] UpdateCommentDto dto)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _commentService.UpdateAsync(commentId, dto, userId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{commentId}")]
    public async Task<IActionResult> Delete(int commentId)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var role = Enum.Parse<UserRole>(User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "User");
        var result = await _commentService.DeleteAsync(commentId, userId, role);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{commentId}/vote")]
    public async Task<IActionResult> Vote(int commentId, [FromBody] VoteDto dto)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _commentService.VoteAsync(commentId, userId, dto.IsUpvote);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{commentId}/vote")]
    public async Task<IActionResult> RemoveVote(int commentId)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _commentService.RemoveVoteAsync(commentId, userId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
