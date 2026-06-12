using DOAN_LAPTRINHWEB.Models.DTOs;

namespace DOAN_LAPTRINHWEB.Interfaces;

public interface IUploadService
{
    /// <summary>Upload và cập nhật avatar cho user</summary>
    Task<ApiResponse<UploadAvatarResponseDto>> UploadAvatarAsync(int userId, IFormFile file);

    /// <summary>Upload và cập nhật ảnh bìa hồ sơ</summary>
    Task<ApiResponse<UploadCoverResponseDto>> UploadCoverPhotoAsync(int userId, IFormFile file);

    /// <summary>Upload tài liệu (PDF, ảnh, source code, log). Có thể gắn vào post.</summary>
    Task<ApiResponse<UploadResultDto>> UploadDocumentAsync(int userId, IFormFile file, int? postId = null);

    /// <summary>Lấy danh sách file đã upload của user</summary>
    Task<PaginatedResponse<UploadResultDto>> GetUserUploadsAsync(int userId, int page, int pageSize, string? fileType = null);

    /// <summary>Xóa file upload (chỉ chủ sở hữu hoặc admin)</summary>
    Task<ApiResponse<bool>> DeleteUploadAsync(int userId, int uploadId, bool isAdmin = false);
}
