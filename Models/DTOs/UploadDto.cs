using System.ComponentModel.DataAnnotations;

namespace DOAN_LAPTRINHWEB.Models.DTOs;

public class UploadResultDto
{
    public int Id { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string FileSizeFormatted { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public int? PostId { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class UploadAvatarResponseDto
{
    public string AvatarUrl { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
}

public class UploadCoverResponseDto
{
    public string CoverPhotoUrl { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
}

public class AttachPostDto
{
    public int? PostId { get; set; }
}
