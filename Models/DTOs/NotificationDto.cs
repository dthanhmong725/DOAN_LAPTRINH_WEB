using DOAN_LAPTRINHWEB.Models.Entities;

namespace DOAN_LAPTRINHWEB.Models.DTOs;

public class NotificationDto
{
    public int Id { get; set; }
    public NotificationType Type { get; set; }
    public string TypeLabel { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? ActorUsername { get; set; }
    public string? ActorAvatar { get; set; }
    public int? PostId { get; set; }
    public string? PostTitle { get; set; }
    public int? CommentId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
}

public class NotificationCountDto
{
    public int UnreadCount { get; set; }
}
