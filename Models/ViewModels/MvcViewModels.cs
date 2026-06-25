using DOAN_LAPTRINHWEB.Models.DTOs;

namespace DOAN_LAPTRINHWEB.Models.ViewModels;

public class HomeViewModel
{
    public List<CategoryDto> Categories { get; set; } = new();
    public List<PostListDto> Posts { get; set; } = new();
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public int TotalItems { get; set; } = 0;
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
    public string SortBy { get; set; } = "";
}

public class CategoryDetailViewModel
{
    public CategoryDto Category { get; set; } = null!;
    public List<PostListDto> Posts { get; set; } = new();
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public int TotalItems { get; set; } = 0;
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
    public string SortBy { get; set; } = "";
}

public class PostDetailViewModel
{
    public PostDto Post { get; set; } = null!;
    public List<CommentDto> Comments { get; set; } = new();
    public int CurrentUserId { get; set; }
    public bool IsAuthenticated { get; set; }
    public bool IsAdminOrMod { get; set; }
    public bool IsAuthor { get; set; }
}

public class CreatePostViewModel
{
    public List<CategoryDto> Categories { get; set; } = new();
}

public class EditPostViewModel
{
    public PostDto? Post { get; set; }
    public List<CategoryDto> Categories { get; set; } = new();
}

public class ProfileViewModel
{
    public PublicProfileDto? Profile { get; set; }
    public List<PostListDto> RecentPosts { get; set; } = new();
    public List<ActivityLogDto> RecentActivities { get; set; } = new();
    public bool IsOwnProfile { get; set; }
    public bool IsFollowing { get; set; }
}

public class BookmarksViewModel
{
    public List<PostListDto> Bookmarks { get; set; } = new();
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public int TotalItems { get; set; } = 0;
}

public class LeaderboardViewModel
{
    public List<LeaderboardEntryDto> Entries { get; set; } = new();
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
}

public class BadgesViewModel
{
    public List<BadgeDto> AllBadges { get; set; } = new();
    public List<BadgeDto> UserBadges { get; set; } = new();
    public int? UserId { get; set; }
}

public class ReputationViewModel
{
    public ReputationDto? Reputation { get; set; }
    public List<ReputationHistoryDto> History { get; set; } = new();
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
}

public class ChatViewModel
{
    public List<ChatRoomDto> ChatRooms { get; set; } = new();
}

public class AdminDashboardViewModel
{
    public DashboardStatsDto Stats { get; set; } = new();
    public List<UserDto> RecentUsers { get; set; } = new();
    public List<PostListDto> RecentPosts { get; set; } = new();
}

public class ModeratorViewModel
{
    public List<PostListDto> FlaggedPosts { get; set; } = new();
    public List<CommentDto> FlaggedComments { get; set; } = new();
}
