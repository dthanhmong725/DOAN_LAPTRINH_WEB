using Microsoft.EntityFrameworkCore;
using DOAN_LAPTRINHWEB.Data;
using DOAN_LAPTRINHWEB.Interfaces;
using DOAN_LAPTRINHWEB.Models.DTOs;
using DOAN_LAPTRINHWEB.Models.Entities;

namespace DOAN_LAPTRINHWEB.Services;

public class BadgeService : IBadgeService
{
    private readonly AppDbContext _context;

    public BadgeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<List<BadgeDto>>> GetAllBadgesAsync(int? userId)
    {
        var badges = await _context.Badges.ToListAsync();
        List<UserBadge> userBadges = new();

        if (userId.HasValue)
        {
            userBadges = await _context.UserBadges
                .Where(ub => ub.UserId == userId.Value)
                .ToListAsync();
        }

        var result = badges.Select(b =>
        {
            var userBadge = userBadges.FirstOrDefault(ub => ub.BadgeId == b.Id);
            return new BadgeDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                Icon = b.Icon,
                Color = b.Color,
                Type = b.Type,
                ReputationRequired = b.ReputationRequired,
                IsEarned = userBadge != null,
                EarnedAt = userBadge?.EarnedAt
            };
        }).ToList();

        return ApiResponse<List<BadgeDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<List<BadgeDto>>> GetUserBadgesAsync(int userId)
    {
        var userBadges = await _context.UserBadges
            .Include(ub => ub.Badge)
            .Where(ub => ub.UserId == userId)
            .ToListAsync();

        var result = userBadges.Select(ub => new BadgeDto
        {
            Id = ub.Badge.Id,
            Name = ub.Badge.Name,
            Description = ub.Badge.Description,
            Icon = ub.Badge.Icon,
            Color = ub.Badge.Color,
            Type = ub.Badge.Type,
            ReputationRequired = ub.Badge.ReputationRequired,
            IsEarned = true,
            EarnedAt = ub.EarnedAt
        }).ToList();

        return ApiResponse<List<BadgeDto>>.SuccessResponse(result);
    }

    public async Task CheckAndAwardBadgesAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Badges)
            .Include(u => u.Posts)
            .Include(u => u.Comments)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return;

        var earnedBadgeIds = user.Badges.Select(ub => ub.BadgeId).ToHashSet();
        var allBadges = await _context.Badges.ToListAsync();

        var postCount = user.Posts.Count(p => !p.IsDeleted);
        var totalUpvotes = user.Posts.Where(p => !p.IsDeleted).Sum(p => p.UpvoteCount) +
                          user.Comments.Where(c => !c.IsDeleted).Sum(c => c.UpvoteCount);
        var bookmarkCount = await _context.Bookmarks.CountAsync(b => b.UserId == userId);
        var isNightOwl = DateTime.UtcNow.Hour >= 0 && DateTime.UtcNow.Hour < 5;

        foreach (var badge in allBadges)
        {
            if (earnedBadgeIds.Contains(badge.Id)) continue;

            bool shouldAward = badge.Type switch
            {
                "milestone" => badge.Id switch
                {
                    1 => postCount >= 1,
                    2 => postCount >= 10,
                    3 => postCount >= 50,
                    4 => postCount >= 100,
                    _ => false
                },
                "reputation" => totalUpvotes >= (badge.Id switch
                {
                    5 => 10,
                    6 => 50,
                    7 => 100,
                    _ => 0
                }),
                "special" => badge.Id switch
                {
                    8 => bookmarkCount >= 10,
                    9 => isNightOwl,
                    _ => false
                },
                "rank" => badge.Id == 10 && user.ReputationPoints >= 100,
                _ => false
            };

            if (shouldAward)
            {
                _context.UserBadges.Add(new UserBadge
                {
                    UserId = userId,
                    BadgeId = badge.Id,
                    EarnedAt = DateTime.UtcNow
                });
            }
        }

        await _context.SaveChangesAsync();
    }
}
