using Microsoft.EntityFrameworkCore;
using DOAN_LAPTRINHWEB.Data;
using DOAN_LAPTRINHWEB.Interfaces;
using DOAN_LAPTRINHWEB.Models.DTOs;
using DOAN_LAPTRINHWEB.Models.Entities;

namespace DOAN_LAPTRINHWEB.Services;

public class ActivityLogService : IActivityLogService
{
    private readonly AppDbContext _context;

    public ActivityLogService(AppDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(ActivityType type, int userId, string? ipAddress, string? userAgent, string? details, bool isSuccess = true)
    {
        var log = new ActivityLog
        {
            Type = type,
            UserId = userId,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Details = details,
            IsSuccess = isSuccess,
            CreatedAt = DateTime.UtcNow
        };

        _context.ActivityLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task<PaginatedResponse<ActivityLogDto>> GetUserActivityAsync(int userId, int page, int pageSize)
    {
        var query = _context.ActivityLogs
            .Where(al => al.UserId == userId)
            .OrderByDescending(al => al.CreatedAt);

        var totalItems = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(al => new ActivityLogDto
            {
                Id = al.Id,
                Type = al.Type.ToString(),
                IpAddress = al.IpAddress,
                Details = al.Details,
                IsSuccess = al.IsSuccess,
                CreatedAt = al.CreatedAt
            })
            .ToListAsync();

        return new PaginatedResponse<ActivityLogDto>
        {
            Success = true,
            Data = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    public async Task<PaginatedResponse<ActivityLogDto>> GetAllActivityAsync(int page, int pageSize, int? userId, ActivityType? type)
    {
        var query = _context.ActivityLogs.AsQueryable();

        if (userId.HasValue)
            query = query.Where(al => al.UserId == userId.Value);

        if (type.HasValue)
            query = query.Where(al => al.Type == type.Value);

        query = query.OrderByDescending(al => al.CreatedAt);

        var totalItems = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(al => al.User)
            .Select(al => new ActivityLogDto
            {
                Id = al.Id,
                Type = al.Type.ToString(),
                IpAddress = al.IpAddress,
                Details = al.Details,
                IsSuccess = al.IsSuccess,
                CreatedAt = al.CreatedAt
            })
            .ToListAsync();

        return new PaginatedResponse<ActivityLogDto>
        {
            Success = true,
            Data = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }
}
