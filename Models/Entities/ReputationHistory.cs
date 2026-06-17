using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOAN_LAPTRINHWEB.Models.Entities;

/// <summary>
/// Loại hành động tạo ra thay đổi điểm uy tín.
/// Dùng để audit, hiển thị lịch sử và áp dụng quy tắc cộng/trừ điểm.
/// </summary>
public enum ReputationAction
{
    PostCreated = 1,         // +5 khi đăng bài
    PostUpvoted = 2,         // +10 mỗi upvote nhận được
    PostDownvoted = 3,       // -2 mỗi downvote nhận được
    PostDeleted = 4,         // -15 nếu bài bị xóa
    CommentCreated = 5,      // +2 khi bình luận
    CommentUpvoted = 6,      // +5 mỗi upvote nhận được
    CommentDownvoted = 7,    // -1 mỗi downvote nhận được
    CommentDeleted = 8,      // -5 nếu bình luận bị xóa
    UpvoteRemoved = 9,       // Hủy upvote: trừ lại điểm
    DownvoteRemoved = 10,    // Hủy downvote: cộng lại điểm
    VoteChanged = 11,        // Đổi chiều vote
    AdminAdjustment = 12     // Admin tinh chỉnh thủ công
}

/// <summary>
/// Lịch sử thay đổi điểm uy tín của user.
/// Mỗi lần reputation thay đổi sẽ tạo một bản ghi.
/// </summary>
public class ReputationHistory
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;

    public ReputationAction Action { get; set; }

    /// <summary>Số điểm thay đổi (dương = cộng, âm = trừ)</summary>
    public int PointsChange { get; set; }

    /// <summary>Tổng điểm sau khi thay đổi (snapshot)</summary>
    public int TotalPointsAfter { get; set; }

    /// <summary>Rank mới sau khi thay đổi</summary>
    public UserRank RankAfter { get; set; }

    /// <summary>ID bài viết liên quan (nếu có)</summary>
    public int? PostId { get; set; }

    [ForeignKey(nameof(PostId))]
    public virtual Post? Post { get; set; }

    /// <summary>ID bình luận liên quan (nếu có)</summary>
    public int? CommentId { get; set; }

    [ForeignKey(nameof(CommentId))]
    public virtual Comment? Comment { get; set; }

    /// <summary>User gây ra thay đổi (actor) - null nếu là hệ thống</summary>
    public int? ActorId { get; set; }

    [ForeignKey(nameof(ActorId))]
    public virtual User? Actor { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
