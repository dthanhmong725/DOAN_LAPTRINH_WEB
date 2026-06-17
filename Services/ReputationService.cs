using Microsoft.EntityFrameworkCore;
using DOAN_LAPTRINHWEB.Data;
using DOAN_LAPTRINHWEB.Interfaces;
using DOAN_LAPTRINHWEB.Models.DTOs;
using DOAN_LAPTRINHWEB.Models.Entities;

namespace DOAN_LAPTRINHWEB.Services;

/// <summary>
/// Quản lý điểm uy tín (reputation) của user.
/// Tự động cập nhật ReputationPoints + Rank và ghi log lịch sử.
/// </summary>
public class ReputationService : IReputationService
{
    private readonly AppDbContext _context;

    // Quy tắc cộng/trừ điểm
    public static class Rules
    {
        public const int PostCreated = 5;
        public const int PostUpvoted = 10;
        public const int PostDownvoted = -2;
        public const int PostDeleted = -15;
        public const int CommentCreated = 2;
        public const int CommentUpvoted = 5;
        public const int CommentDownvoted = -1;
        public const int CommentDeleted = -5;
    }

    public ReputationService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Cộng/trừ điểm cho user dựa trên hành động.
    /// Trả về tổng điểm mới.
    /// </summary>
    public async Task<int> ApplyChangeAsync(int userId, ReputationAction action, int? actorId = null, int? postId = null, int? commentId = null, string? description = null)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return 0;

        int points = GetPointsForAction(action);
        if (points == 0) return user.ReputationPoints;

        // Không cho điểm âm
        int newTotal = Math.Max(0, user.ReputationPoints + points);
        int actualChange = newTotal - user.ReputationPoints;

        user.ReputationPoints = newTotal;
        user.Rank = CalculateRank(newTotal);
        user.UpdatedAt = DateTime.UtcNow;

        var history = new ReputationHistory
        {
            UserId = userId,
            Action = action,
            PointsChange = actualChange,
            TotalPointsAfter = newTotal,
            RankAfter = user.Rank,
            PostId = postId,
            CommentId = commentId,
            ActorId = actorId,
            Description = description ?? GetDefaultDescription(action),
            CreatedAt = DateTime.UtcNow
        };

        _context.ReputationHistories.Add(history);
        await _context.SaveChangesAsync();

        return newTotal;
    }

    public async Task<ApiResponse<ReputationDto>> GetReputationAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Posts.Where(p => !p.IsDeleted))
            .Include(u => u.Comments.Where(c => !c.IsDeleted))
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return ApiResponse<ReputationDto>.ErrorResponse("Không tìm thấy người dùng");

        var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var upvotes = user.Posts.Sum(p => p.UpvoteCount) + user.Comments.Sum(c => c.UpvoteCount);
        var downvotes = user.Posts.Sum(p => p.DownvoteCount) + user.Comments.Sum(c => c.DownvoteCount);

        return ApiResponse<ReputationDto>.SuccessResponse(new ReputationDto
        {
            UserId = user.Id,
            Username = user.DisplayName ?? user.Username,
            TotalPoints = user.ReputationPoints,
            Rank = user.Rank.ToString(),
            PostsThisMonth = user.Posts.Count(p => p.CreatedAt >= startOfMonth),
            CommentsThisMonth = user.Comments.Count(c => c.CreatedAt >= startOfMonth),
            UpvotesReceived = upvotes,
            DownvotesReceived = downvotes
        });
    }

    public async Task<PaginatedResponse<ReputationHistoryDto>> GetHistoryAsync(int userId, int page, int pageSize)
    {
        var query = _context.ReputationHistories
            .Where(h => h.UserId == userId)
            .OrderByDescending(h => h.CreatedAt);

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(h => new
            {
                h.Id,
                h.Action,
                h.PointsChange,
                h.TotalPointsAfter,
                h.RankAfter,
                h.PostId,
                PostTitle = h.Post != null ? h.Post.Title : null,
                h.CommentId,
                h.ActorId,
                ActorName = h.Actor != null ? (h.Actor.DisplayName != null ? h.Actor.DisplayName : h.Actor.Username) : null,
                h.Description,
                h.CreatedAt
            })
            .ToListAsync();

        var dtos = items.Select(i => new ReputationHistoryDto
        {
            Id = i.Id,
            Action = i.Action.ToString(),
            ActionLabel = GetActionLabel(i.Action),
            PointsChange = i.PointsChange,
            TotalPointsAfter = i.TotalPointsAfter,
            RankAfter = i.RankAfter.ToString(),
            PostId = i.PostId,
            PostTitle = i.PostTitle,
            CommentId = i.CommentId,
            ActorId = i.ActorId,
            ActorName = i.ActorName,
            Description = i.Description,
            Icon = GetActionIcon(i.Action),
            Color = GetActionColor(i.Action),
            CreatedAt = i.CreatedAt
        }).ToList();

        return new PaginatedResponse<ReputationHistoryDto>
        {
            Success = true,
            Data = dtos,
            Page = page,
            PageSize = pageSize,
            TotalItems = total
        };
    }

    public async Task<PaginatedResponse<LeaderboardEntryDto>> GetLeaderboardAsync(int page, int pageSize)
    {
        var query = _context.Users
            .Where(u => u.IsActive && !u.IsBanned)
            .OrderByDescending(u => u.ReputationPoints)
            .ThenByDescending(u => u.CreatedAt);

        var total = await query.CountAsync();
        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var entries = new List<LeaderboardEntryDto>();
        int baseRank = (page - 1) * pageSize + 1;

        for (int i = 0; i < users.Count; i++)
        {
            var u = users[i];
            var postCount = await _context.Posts.CountAsync(p => p.AuthorId == u.Id && !p.IsDeleted);
            var commentCount = await _context.Comments.CountAsync(c => c.AuthorId == u.Id && !c.IsDeleted);

            entries.Add(new LeaderboardEntryDto
            {
                Rank = baseRank + i,
                ReputationPoints = u.ReputationPoints,
                PostCount = postCount,
                CommentCount = commentCount,
                User = new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    DisplayName = u.DisplayName,
                    AvatarUrl = u.AvatarUrl,
                    Role = u.Role.ToString(),
                    Rank = u.Rank.ToString(),
                    ReputationPoints = u.ReputationPoints
                }
            });
        }

        return new PaginatedResponse<LeaderboardEntryDto>
        {
            Success = true,
            Data = entries,
            Page = page,
            PageSize = pageSize,
            TotalItems = total
        };
    }

    public ReputationRulesDto GetRules()
    {
        return new ReputationRulesDto
        {
            Rules = new List<ReputationRuleItem>
            {
                new() { Action = "Đăng bài viết", Description = "Được cộng khi bài viết được đăng thành công", Points = Rules.PostCreated, Icon = "ti-article", Color = "#388bfd" },
                new() { Action = "Bài viết được upvote", Description = "Mỗi lượt upvote bài viết", Points = Rules.PostUpvoted, Icon = "ti-arrow-up", Color = "#00e5a0" },
                new() { Action = "Bài viết bị downvote", Description = "Mỗi lượt downvote bài viết", Points = Rules.PostDownvoted, Icon = "ti-arrow-down", Color = "#ff5a5f" },
                new() { Action = "Bình luận", Description = "Được cộng khi bình luận được đăng", Points = Rules.CommentCreated, Icon = "ti-message", Color = "#388bfd" },
                new() { Action = "Bình luận được upvote", Description = "Mỗi lượt upvote bình luận", Points = Rules.CommentUpvoted, Icon = "ti-thumb-up", Color = "#00e5a0" },
                new() { Action = "Bình luận bị downvote", Description = "Mỗi lượt downvote bình luận", Points = Rules.CommentDownvoted, Icon = "ti-thumb-down", Color = "#ff5a5f" },
                new() { Action = "Bài viết bị xóa", Description = "Trừ khi bài viết bị xóa bởi mod/admin", Points = Rules.PostDeleted, Icon = "ti-trash", Color = "#ff5a5f" },
                new() { Action = "Bình luận bị xóa", Description = "Trừ khi bình luận bị xóa bởi mod/admin", Points = Rules.CommentDeleted, Icon = "ti-trash", Color = "#ff5a5f" }
            },
            Ranks = new List<ReputationRankInfo>
            {
                new() { Name = "Newbie",       MinPoints = 0,    Color = "#7d8590", Icon = "ti-user" },
                new() { Name = "Learner",      MinPoints = 25,   Color = "#388bfd", Icon = "ti-book" },
                new() { Name = "Practitioner", MinPoints = 100,  Color = "#0dcaf0", Icon = "ti-tool" },
                new() { Name = "Professional", MinPoints = 500,  Color = "#20c997", Icon = "ti-briefcase" },
                new() { Name = "Expert",       MinPoints = 2000, Color = "#bc8cff", Icon = "ti-award" },
                new() { Name = "Elite",        MinPoints = 5000, Color = "#00e5a0", Icon = "ti-crown" }
            }
        };
    }

    // ============================================================
    // STATIC HELPERS
    // ============================================================
    public static int GetPointsForAction(ReputationAction action) => action switch
    {
        ReputationAction.PostCreated     => Rules.PostCreated,
        ReputationAction.PostUpvoted     => Rules.PostUpvoted,
        ReputationAction.PostDownvoted   => Rules.PostDownvoted,
        ReputationAction.PostDeleted     => Rules.PostDeleted,
        ReputationAction.CommentCreated  => Rules.CommentCreated,
        ReputationAction.CommentUpvoted  => Rules.CommentUpvoted,
        ReputationAction.CommentDownvoted=> Rules.CommentDownvoted,
        ReputationAction.CommentDeleted  => Rules.CommentDeleted,
        _ => 0
    };

    public static UserRank CalculateRank(int points) => points switch
    {
        >= 5000 => UserRank.Elite,
        >= 2000 => UserRank.Expert,
        >= 500 => UserRank.Professional,
        >= 100 => UserRank.Practitioner,
        >= 25 => UserRank.Learner,
        _ => UserRank.Newbie
    };

    public static string GetActionLabel(ReputationAction action) => action switch
    {
        ReputationAction.PostCreated     => "Đăng bài viết",
        ReputationAction.PostUpvoted     => "Bài viết được upvote",
        ReputationAction.PostDownvoted   => "Bài viết bị downvote",
        ReputationAction.PostDeleted     => "Bài viết bị xóa",
        ReputationAction.CommentCreated  => "Đăng bình luận",
        ReputationAction.CommentUpvoted  => "Bình luận được upvote",
        ReputationAction.CommentDownvoted=> "Bình luận bị downvote",
        ReputationAction.CommentDeleted  => "Bình luận bị xóa",
        ReputationAction.UpvoteRemoved   => "Hủy upvote",
        ReputationAction.DownvoteRemoved => "Hủy downvote",
        ReputationAction.VoteChanged     => "Đổi lượt vote",
        ReputationAction.AdminAdjustment => "Tinh chỉnh bởi Admin",
        _ => "Hành động khác"
    };

    public static string GetActionIcon(ReputationAction action) => action switch
    {
        ReputationAction.PostCreated     => "ti-article",
        ReputationAction.PostUpvoted     => "ti-arrow-up",
        ReputationAction.PostDownvoted   => "ti-arrow-down",
        ReputationAction.PostDeleted     => "ti-trash",
        ReputationAction.CommentCreated  => "ti-message",
        ReputationAction.CommentUpvoted  => "ti-thumb-up",
        ReputationAction.CommentDownvoted=> "ti-thumb-down",
        ReputationAction.CommentDeleted  => "ti-trash",
        ReputationAction.UpvoteRemoved   => "ti-arrow-back-up",
        ReputationAction.DownvoteRemoved => "ti-arrow-back-up",
        ReputationAction.VoteChanged     => "ti-switch-horizontal",
        ReputationAction.AdminAdjustment => "ti-shield-cog",
        _ => "ti-star"
    };

    public static string GetActionColor(ReputationAction action)
    {
        int pts = GetPointsForAction(action);
        if (pts > 0) return "#00e5a0";
        if (pts < 0) return "#ff5a5f";
        return "#8b949e";
    }

    public static string GetDefaultDescription(ReputationAction action) => action switch
    {
        ReputationAction.PostCreated     => "Đã tạo bài viết mới",
        ReputationAction.PostUpvoted     => "Bài viết nhận được upvote",
        ReputationAction.PostDownvoted   => "Bài viết nhận downvote",
        ReputationAction.PostDeleted     => "Bài viết đã bị xóa",
        ReputationAction.CommentCreated  => "Đã đăng bình luận",
        ReputationAction.CommentUpvoted  => "Bình luận nhận được upvote",
        ReputationAction.CommentDownvoted=> "Bình luận nhận downvote",
        ReputationAction.CommentDeleted  => "Bình luận đã bị xóa",
        _ => ""
    };
}
