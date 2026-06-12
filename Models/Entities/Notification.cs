using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOAN_LAPTRINHWEB.Models.Entities;

public enum NotificationType
{
    PostUpvote,
    PostDownvote,
    Comment,
    CommentUpvote,
    CommentDownvote,
    Mention
}

public class Notification
{
    [Key]
    public int Id { get; set; }

    /// <summary>Người nhận thông báo</summary>
    public int RecipientId { get; set; }

    [ForeignKey(nameof(RecipientId))]
    public virtual User Recipient { get; set; } = null!;

    /// <summary>Người thực hiện hành động (có thể null nếu hệ thống)</summary>
    public int? ActorId { get; set; }

    [ForeignKey(nameof(ActorId))]
    public virtual User? Actor { get; set; }

    public NotificationType Type { get; set; }

    /// <summary>Bài viết liên quan</summary>
    public int? PostId { get; set; }

    [ForeignKey(nameof(PostId))]
    public virtual Post? Post { get; set; }

    /// <summary>Bình luận liên quan</summary>
    public int? CommentId { get; set; }

    [ForeignKey(nameof(CommentId))]
    public virtual Comment? Comment { get; set; }

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
