using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOAN_LAPTRINHWEB.Models.Entities;

public enum UploadFileType
{
    Image = 0,
    Pdf = 1,
    SourceCode = 2,
    Log = 3,
    Other = 4
}

public class UserUpload
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string OriginalFileName { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string StoredFileName { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string FileUrl { get; set; } = string.Empty;

    [StringLength(100)]
    public string ContentType { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }

    public UploadFileType FileType { get; set; } = UploadFileType.Other;

    /// <summary>Gắn với bài post cụ thể nếu có</summary>
    public int? PostId { get; set; }

    [ForeignKey(nameof(PostId))]
    public virtual Post? Post { get; set; }

    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; } = false;
}
