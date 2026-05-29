using System.ComponentModel.DataAnnotations;

namespace DOAN_LAPTRINHWEB.Models.DTOs;

// Bookmark DTOs
public class BookmarkDto
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public PostListDto Post { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

public class ToggleBookmarkDto
{
    [Required]
    public int PostId { get; set; }
}

// Badge DTOs
public class BadgeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Icon { get; set; } = "award";
    public string Color { get; set; } = "#ffc107";
    public string Type { get; set; } = "achievement";
    public int ReputationRequired { get; set; }
    public bool IsEarned { get; set; }
    public DateTime? EarnedAt { get; set; }
}

// Reputation DTOs
public class ReputationDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public string Rank { get; set; } = "Newbie";
    public int PostsThisMonth { get; set; }
    public int CommentsThisMonth { get; set; }
    public int UpvotesReceived { get; set; }
    public int DownvotesReceived { get; set; }
}

public class ReputationHistoryDto
{
    public int Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public int PointsChange { get; set; }
    public string? RelatedPostTitle { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Activity Log DTOs
public class ActivityLogDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? Details { get; set; }
    public bool IsSuccess { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Chat DTOs
public class ChatRoomDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = "Direct";
    public int MemberCount { get; set; }
    public ChatMessageDto? LastMessage { get; set; }
    public int UnreadCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<UserDto> Members { get; set; } = new();
}

public class ChatMessageDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? AttachmentUrl { get; set; }
    public string? AttachmentType { get; set; }
    public bool IsEdited { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? EditedAt { get; set; }
    public UserDto Sender { get; set; } = null!;
    public int ChatRoomId { get; set; }
    public int? ReplyToId { get; set; }
    public ChatMessageDto? ReplyTo { get; set; }
}

public class CreateChatRoomDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public List<int>? MemberIds { get; set; }
}

public class SendMessageDto
{
    [Required]
    public string Content { get; set; } = string.Empty;

    public int? AttachmentId { get; set; }

    public int? ReplyToId { get; set; }
}

// Password Strength
public class CheckPasswordDto
{
    [Required]
    public string Password { get; set; } = string.Empty;
}

public class PasswordStrengthDto
{
    public int Score { get; set; }
    public string Level { get; set; } = "Yếu";
    public List<string> Suggestions { get; set; } = new();
    public bool HasMinLength { get; set; }
    public bool HasUppercase { get; set; }
    public bool HasLowercase { get; set; }
    public bool HasDigit { get; set; }
    public bool HasSpecialChar { get; set; }
    public bool HasNoCommonPatterns { get; set; }
}

// Leaderboard DTOs
public class LeaderboardEntryDto
{
    public int Rank { get; set; }
    public UserDto User { get; set; } = null!;
    public int ReputationPoints { get; set; }
    public int PostCount { get; set; }
    public int CommentCount { get; set; }
}
