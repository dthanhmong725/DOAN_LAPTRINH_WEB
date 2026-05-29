using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOAN_LAPTRINHWEB.Models.Entities;

public enum ChatRoomType
{
    Direct = 0,
    Group = 1
}

public class ChatRoom
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public ChatRoomType Type { get; set; } = ChatRoomType.Direct;

    public bool IsActive { get; set; } = true;

    public int MemberCount { get; set; } = 0;

    public DateTime? LastActivityAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int CreatedById { get; set; }

    [ForeignKey(nameof(CreatedById))]
    public virtual User CreatedBy { get; set; } = null!;

    // Navigation properties
    public virtual ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    public virtual ICollection<ChatRoomMember> Members { get; set; } = new List<ChatRoomMember>();
}

public class ChatRoomMember
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;

    public int ChatRoomId { get; set; }

    [ForeignKey(nameof(ChatRoomId))]
    public virtual ChatRoom ChatRoom { get; set; } = null!;

    public bool IsAdmin { get; set; } = false;

    public bool IsMuted { get; set; } = false;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastReadAt { get; set; }
}

public class ChatMessage
{
    [Key]
    public int Id { get; set; }

    public string Content { get; set; } = string.Empty;

    [StringLength(255)]
    public string? AttachmentUrl { get; set; }

    [StringLength(100)]
    public string? AttachmentType { get; set; }

    public bool IsEdited { get; set; } = false;

    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? EditedAt { get; set; }

    public int SenderId { get; set; }

    [ForeignKey(nameof(SenderId))]
    public virtual User Sender { get; set; } = null!;

    public int ChatRoomId { get; set; }

    [ForeignKey(nameof(ChatRoomId))]
    public virtual ChatRoom ChatRoom { get; set; } = null!;

    public int? ReplyToId { get; set; }

    [ForeignKey(nameof(ReplyToId))]
    public virtual ChatMessage? ReplyTo { get; set; }
}
