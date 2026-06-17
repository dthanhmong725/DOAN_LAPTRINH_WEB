using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DOAN_LAPTRINHWEB.Interfaces;

namespace DOAN_LAPTRINHWEB.Controllers;

[ApiController]
[Route("api/reputation")]
[Authorize]
public class ReputationController : ControllerBase
{
    private readonly IReputationService _reputationService;

    public ReputationController(IReputationService reputationService)
    {
        _reputationService = reputationService;
    }

    /// <summary>Lấy tổng quan điểm uy tín của user</summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserReputation(int userId)
    {
        var result = await _reputationService.GetReputationAsync(userId);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    /// <summary>Lấy điểm uy tín của user hiện tại (đang đăng nhập)</summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMyReputation()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _reputationService.GetReputationAsync(userId);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    /// <summary>Lấy lịch sử điểm uy tín của user hiện tại</summary>
    [HttpGet("me/history")]
    public async Task<IActionResult> GetMyHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _reputationService.GetHistoryAsync(userId, page, pageSize);
        return Ok(result);
    }

    /// <summary>Lấy lịch sử điểm uy tín của 1 user cụ thể</summary>
    [HttpGet("user/{userId}/history")]
    public async Task<IActionResult> GetUserHistory(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _reputationService.GetHistoryAsync(userId, page, pageSize);
        return Ok(result);
    }

    /// <summary>Top user theo điểm uy tín (leaderboard)</summary>
    [HttpGet("leaderboard")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLeaderboard([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _reputationService.GetLeaderboardAsync(page, pageSize);
        return Ok(result);
    }

    /// <summary>Quy tắc cộng/trừ điểm - public để user biết cách tích điểm</summary>
    [HttpGet("rules")]
    [AllowAnonymous]
    public IActionResult GetRules()
    {
        return Ok(new
        {
            success = true,
            data = _reputationService.GetRules()
        });
    }
}
