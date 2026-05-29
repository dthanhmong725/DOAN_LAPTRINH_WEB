using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOAN_LAPTRINHWEB.Models.Entities;

public enum UserRole
{
    User = 0,
    Moderator = 1,
    Admin = 2
}

public enum UserRank
{
    Newbie = 0,
    Learner = 1,
    Practitioner = 2,
    Professional = 3,
    Expert = 4,
    Elite = 5
}

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public bool IsEmailVerified { get; set; } = false;

    [StringLength(6)]
    public string? EmailVerificationToken { get; set; }

    public DateTime? EmailVerificationTokenExpiry { get; set; }

    public int FailedLoginAttempts { get; set; } = 0;

    public DateTime? LockoutEnd { get; set; }

    [StringLength(50)]
    public string? DisplayName { get; set; }

    [StringLength(500)]
    public string? AvatarUrl { get; set; }

    [StringLength(1000)]
    public string? Bio { get; set; }

    public UserRole Role { get; set; } = UserRole.User;

    public UserRank Rank { get; set; } = UserRank.Newbie;

    public int ReputationPoints { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    public bool IsBanned { get; set; } = false;

    [StringLength(100)]
    public string? BanReason { get; set; }

    public DateTime? LastLoginAt { get; set; }

    [StringLength(45)]
    public string? LastLoginIp { get; set; }

    [StringLength(255)]
    public string? ResetToken { get; set; }

    public DateTime? ResetTokenExpiry { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [NotMapped]
    public bool IsLockedOut => LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public virtual ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
    public virtual ICollection<UserBadge> Badges { get; set; } = new List<UserBadge>();
    public virtual ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
    public virtual ICollection<SecurityLog> SecurityLogs { get; set; } = new List<SecurityLog>();
    public virtual ICollection<ChatMessage> SentMessages { get; set; } = new List<ChatMessage>();
    public virtual ICollection<ChatRoomMember> ChatRooms { get; set; } = new List<ChatRoomMember>();
}

public class RefreshToken
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(45)]
    public string? CreatedByIp { get; set; }

    public bool IsRevoked { get; set; } = false;

    [StringLength(45)]
    public string? RevokedByIp { get; set; }

    public DateTime? RevokedAt { get; set; }

    [StringLength(500)]
    public string? ReplacedByToken { get; set; }

    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
}
