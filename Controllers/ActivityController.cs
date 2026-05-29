using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DOAN_LAPTRINHWEB.Interfaces;
using DOAN_LAPTRINHWEB.Models.DTOs;

namespace DOAN_LAPTRINHWEB.Controllers;

[ApiController]
[Route("api/activity")]
[Authorize]
public class ActivityController : ControllerBase
{
    private readonly IActivityLogService _activityLogService;

    public ActivityController(IActivityLogService activityLogService)
    {
        _activityLogService = activityLogService;
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyActivity([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _activityLogService.GetUserActivityAsync(userId, page, pageSize);
        return Ok(result);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> GetAllActivity([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] int? userId = null, [FromQuery] string? type = null)
    {
        Models.Entities.ActivityType? activityType = null;
        if (!string.IsNullOrEmpty(type) && Enum.TryParse<Models.Entities.ActivityType>(type, true, out var parsed))
        {
            activityType = parsed;
        }

        var result = await _activityLogService.GetAllActivityAsync(page, pageSize, userId, activityType);
        return Ok(result);
    }
}
