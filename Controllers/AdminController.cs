using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DOAN_LAPTRINHWEB.Interfaces;
using DOAN_LAPTRINHWEB.Models.Entities;
using DOAN_LAPTRINHWEB.Authorization;
using DOAN_LAPTRINHWEB.Data;

namespace DOAN_LAPTRINHWEB.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
public class AdminController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly AppDbContext _context;

    public AdminController(IRoleService roleService, AppDbContext context)
    {
        _roleService = roleService;
        _context = context;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? role = null,
        [FromQuery] bool? isBanned = null)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var kw = search.Trim().ToLower();
            query = query.Where(u => u.Username.ToLower().Contains(kw) || (u.DisplayName != null && u.DisplayName.ToLower().Contains(kw)));
        }

        if (!string.IsNullOrEmpty(role) && Enum.TryParse<UserRole>(role, true, out var r))
        {
            query = query.Where(u => u.Role == r);
        }

        if (isBanned.HasValue)
        {
            query = query.Where(u => u.IsBanned == isBanned.Value);
        }

        var totalItems = await query.CountAsync();

        // Trả về định dạng chuẩn chữ thường (camelCase) để JavaScript đọc được
        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new
            {
                id = u.Id,
                username = u.Username,
                displayName = u.DisplayName ?? u.Username,
                email = u.Email,
                role = u.Role.ToString(),
                reputationPoints = u.ReputationPoints,
                isBanned = u.IsBanned
            })
            .ToListAsync();

        return Ok(new { success = true, data = users, totalItems, page, pageSize });
    }

    [HttpPut("users/{id}/role")]
    public async Task<IActionResult> ChangeRole(int id, [FromQuery] UserRole newRole)
    {
        var adminId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _roleService.ChangeRoleAsync(adminId, id, newRole);

        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("users/{id}/ban")]
    public async Task<IActionResult> BanUser(int id, [FromQuery] string reason = "Vi phạm nội quy")
    {
        var adminId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var result = await _roleService.BanUserAsync(adminId, id, reason);
        if (!result.Success) return BadRequest(result);

        // Đồng bộ tuyệt đối: Tắt IsActive và Bật IsBanned
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            user.IsActive = false;
            user.IsBanned = true;
            await _context.SaveChangesAsync();
        }

        return Ok(new { success = true, message = "Đã khóa tài khoản thành công." });
    }

    [HttpPost("users/{id}/unban")]
    public async Task<IActionResult> UnbanUser(int id)
    {
        var adminId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        var result = await _roleService.UnbanUserAsync(adminId, id);
        if (!result.Success) return BadRequest(result);

        // Đồng bộ tuyệt đối: Bật IsActive, Tắt IsBanned và Xóa sạch lý do cấm
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            user.IsActive = true;
            user.IsBanned = false;
            user.BanReason = null;
            await _context.SaveChangesAsync();
        }

        return Ok(new { success = true, message = "Đã mở khóa tài khoản thành công." });
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard([FromQuery] int days = 14)
    {
        var result = await _roleService.GetDashboardStatsAsync(days);
        return Ok(result);
    }

    [HttpGet("security-logs")]
    public async Task<IActionResult> GetSecurityLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] int? userId = null)
    {
        var result = await _roleService.GetSecurityLogsAsync(page, pageSize, userId);
        return Ok(result);
    }
}