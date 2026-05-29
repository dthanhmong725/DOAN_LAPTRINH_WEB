using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOAN_LAPTRINHWEB.Models.Entities;

public enum ActivityType
{
    Login = 1,
    Logout = 2,
    Register = 3,
    PasswordChange = 4,
    PasswordReset = 5,
    EmailChange = 6,
    ProfileUpdate = 7,
    PostCreate = 8,
    PostEdit = 9,
    PostDelete = 10,
    CommentCreate = 11,
    CommentEdit = 12,
    CommentDelete = 13,
    AccountLocked = 14,
    AccountUnlocked = 15,
    RoleChange = 16
}

public class ActivityLog
{
    [Key]
    public int Id { get; set; }

    public ActivityType Type { get; set; }

    [StringLength(45)]
    public string? IpAddress { get; set; }

    [StringLength(500)]
    public string? UserAgent { get; set; }

    [StringLength(500)]
    public string? Details { get; set; }

    public bool IsSuccess { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
}

public class RateLimitRecord
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Endpoint { get; set; } = string.Empty;

    public int RequestCount { get; set; } = 1;

    public DateTime WindowStart { get; set; } = DateTime.UtcNow;

    public DateTime LastRequest { get; set; } = DateTime.UtcNow;
}
