using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DOAN_LAPTRINHWEB.Interfaces;
using DOAN_LAPTRINHWEB.Models.DTOs;
using DOAN_LAPTRINHWEB.Models.Entities;
using DOAN_LAPTRINHWEB.Authorization;

namespace DOAN_LAPTRINHWEB.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;

    public UsersController(IUserService userService, IRoleService roleService)
    {
        _userService = userService;
        _roleService = roleService;
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _userService.GetByIdAsync(id);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpGet("profile/{username}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProfile(string username)
    {
        var result = await _userService.GetPublicProfileAsync(username);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>Lấy thông tin user hiện tại (đang đăng nhập) - dùng cho settings page</summary>
    [HttpGet("profile")]
    public async Task<IActionResult> GetCurrentUserProfile()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _userService.GetByIdAsync(userId);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _userService.UpdateProfileAsync(userId, dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.RequireModerator)]
    public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null, [FromQuery] string? role = null)
    {
        var result = await _userService.GetUsersAsync(page, pageSize, search, role);
        return Ok(result);
    }

    [HttpPut("{id}/role")]
    [Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
    public async Task<IActionResult> ChangeRole(int id, [FromQuery] UserRole newRole)
    {
        var adminId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _roleService.ChangeRoleAsync(adminId, id, newRole);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{id}/ban")]
    [Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
    public async Task<IActionResult> BanUser(int id, [FromQuery] string reason = "Vi phạm nội quy")
    {
        var adminId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _roleService.BanUserAsync(adminId, id, reason);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{id}/unban")]
    [Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
    public async Task<IActionResult> UnbanUser(int id)
    {
        var adminId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _roleService.UnbanUserAsync(adminId, id);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{id}/reputation")]
    [AllowAnonymous]
    public async Task<IActionResult> GetReputation(int id)
    {
        var result = await _userService.GetReputationAsync(id);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpGet("leaderboard")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLeaderboard([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _userService.GetLeaderboardAsync(page, pageSize);
        return Ok(result);
    }
}
