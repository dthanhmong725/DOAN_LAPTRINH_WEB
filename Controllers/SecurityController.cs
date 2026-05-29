using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DOAN_LAPTRINHWEB.Interfaces;
using DOAN_LAPTRINHWEB.Models.Entities;

namespace DOAN_LAPTRINHWEB.Controllers;

[ApiController]
[Route("api/security")]
[Authorize]
public class SecurityController : ControllerBase
{
    private readonly ISecurityLogService _securityLogService;

    public SecurityController(ISecurityLogService securityLogService)
    {
        _securityLogService = securityLogService;
    }

    [HttpGet("logs")]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> GetAllLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? userId = null,
        [FromQuery] string? dateFrom = null,
        [FromQuery] string? dateTo = null,
        [FromQuery] string? action = null,
        [FromQuery] bool? isSuccess = null)
    {
        DateTime? from = string.IsNullOrEmpty(dateFrom) ? null : DateTime.Parse(dateFrom);
        DateTime? to = string.IsNullOrEmpty(dateTo) ? null : DateTime.Parse(dateTo);
        SecurityAction? secAction = null;
        if (!string.IsNullOrEmpty(action) && Enum.TryParse<SecurityAction>(action, true, out var parsed))
            secAction = parsed;

        var result = await _securityLogService.GetAllLogsAsync(page, pageSize, userId, from, to, secAction, isSuccess);
        return Ok(result);
    }

    [HttpGet("logs/mine")]
    public async Task<IActionResult> GetMyLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _securityLogService.GetUserLogsAsync(userId, page, pageSize, null, null, null, null);
        return Ok(result);
    }
}
