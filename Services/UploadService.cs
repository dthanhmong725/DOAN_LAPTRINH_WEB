using Microsoft.EntityFrameworkCore;
using DOAN_LAPTRINHWEB.Data;
using DOAN_LAPTRINHWEB.Interfaces;
using DOAN_LAPTRINHWEB.Models.DTOs;
using DOAN_LAPTRINHWEB.Models.Entities;

namespace DOAN_LAPTRINHWEB.Services;

public class UploadService : IUploadService
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<UploadService> _logger;

    // Các loại file được phép upload
    private static readonly HashSet<string> AllowedImageTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".svg"
    };

    private static readonly HashSet<string> AllowedDocumentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf"
    };

    private static readonly HashSet<string> AllowedCodeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ".cs", ".js", ".ts", ".py", ".java", ".cpp", ".c", ".h", ".go", ".rs",
        ".html", ".css", ".json", ".xml", ".yaml", ".yml", ".sh", ".bat", ".ps1",
        ".md", ".sql", ".php", ".rb", ".kt", ".swift", ".vue", ".jsx", ".tsx"
    };

    private static readonly HashSet<string> AllowedLogTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ".log", ".txt"
    };

    private const long MaxImageSize = 5 * 1024 * 1024;   // 5 MB
    private const long MaxDocumentSize = 20 * 1024 * 1024; // 20 MB

    public UploadService(AppDbContext context, IWebHostEnvironment env, ILogger<UploadService> logger)
    {
        _context = context;
        _env = env;
        _logger = logger;
    }

    public async Task<ApiResponse<UploadAvatarResponseDto>> UploadAvatarAsync(int userId, IFormFile file)
    {
        var validation = ValidateImageFile(file);
        if (!validation.IsValid)
            return ApiResponse<UploadAvatarResponseDto>.ErrorResponse(validation.Error!);

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return ApiResponse<UploadAvatarResponseDto>.ErrorResponse("Không tìm thấy người dùng");

        // Xóa avatar cũ nếu là file local
        DeleteLocalFile(user.AvatarUrl);

        var (fileUrl, storedName) = await SaveFileAsync(file, userId, "avatars");

        // Lưu upload record
        var upload = new UserUpload
        {
            OriginalFileName = file.FileName,
            StoredFileName = storedName,
            FileUrl = fileUrl,
            ContentType = file.ContentType,
            FileSizeBytes = file.Length,
            FileType = UploadFileType.Image,
            UserId = userId
        };
        _context.UserUploads.Add(upload);

        // Cập nhật avatar user
        user.AvatarUrl = fileUrl;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return ApiResponse<UploadAvatarResponseDto>.SuccessResponse(new UploadAvatarResponseDto
        {
            AvatarUrl = fileUrl,
            User = MapUserDto(user)
        }, "Cập nhật avatar thành công!");
    }

    public async Task<ApiResponse<UploadCoverResponseDto>> UploadCoverPhotoAsync(int userId, IFormFile file)
    {
        var validation = ValidateImageFile(file);
        if (!validation.IsValid)
            return ApiResponse<UploadCoverResponseDto>.ErrorResponse(validation.Error!);

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return ApiResponse<UploadCoverResponseDto>.ErrorResponse("Không tìm thấy người dùng");

        DeleteLocalFile(user.CoverPhotoUrl);

        var (fileUrl, storedName) = await SaveFileAsync(file, userId, "covers");

        var upload = new UserUpload
        {
            OriginalFileName = file.FileName,
            StoredFileName = storedName,
            FileUrl = fileUrl,
            ContentType = file.ContentType,
            FileSizeBytes = file.Length,
            FileType = UploadFileType.Image,
            UserId = userId
        };
        _context.UserUploads.Add(upload);

        user.CoverPhotoUrl = fileUrl;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return ApiResponse<UploadCoverResponseDto>.SuccessResponse(new UploadCoverResponseDto
        {
            CoverPhotoUrl = fileUrl,
            User = MapUserDto(user)
        }, "Cập nhật ảnh bìa thành công!");
    }

    public async Task<ApiResponse<UploadResultDto>> UploadDocumentAsync(int userId, IFormFile file, int? postId = null)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        UploadFileType fileType;
        string subfolder;

        if (AllowedImageTypes.Contains(ext))
        {
            if (file.Length > MaxImageSize)
                return ApiResponse<UploadResultDto>.ErrorResponse("Ảnh không được vượt quá 5MB");
            fileType = UploadFileType.Image;
            subfolder = "images";
        }
        else if (AllowedDocumentTypes.Contains(ext))
        {
            if (file.Length > MaxDocumentSize)
                return ApiResponse<UploadResultDto>.ErrorResponse("File PDF không được vượt quá 20MB");
            fileType = UploadFileType.Pdf;
            subfolder = "documents";
        }
        else if (AllowedCodeTypes.Contains(ext))
        {
            if (file.Length > MaxDocumentSize)
                return ApiResponse<UploadResultDto>.ErrorResponse("File source code không được vượt quá 20MB");
            fileType = UploadFileType.SourceCode;
            subfolder = "code";
        }
        else if (AllowedLogTypes.Contains(ext))
        {
            if (file.Length > MaxDocumentSize)
                return ApiResponse<UploadResultDto>.ErrorResponse("File log không được vượt quá 20MB");
            fileType = UploadFileType.Log;
            subfolder = "logs";
        }
        else
        {
            return ApiResponse<UploadResultDto>.ErrorResponse(
                "Định dạng file không được hỗ trợ. Chấp nhận: ảnh (jpg/png/gif/webp), PDF, source code (.cs/.js/.py/...), log (.log/.txt)");
        }

        // Kiểm tra post có tồn tại không
        if (postId.HasValue)
        {
            var postExists = await _context.Posts.AnyAsync(p => p.Id == postId.Value && !p.IsDeleted);
            if (!postExists)
                return ApiResponse<UploadResultDto>.ErrorResponse("Bài viết không tồn tại");
        }

        var (fileUrl, storedName) = await SaveFileAsync(file, userId, subfolder);

        var upload = new UserUpload
        {
            OriginalFileName = file.FileName,
            StoredFileName = storedName,
            FileUrl = fileUrl,
            ContentType = file.ContentType,
            FileSizeBytes = file.Length,
            FileType = fileType,
            PostId = postId,
            UserId = userId
        };
        _context.UserUploads.Add(upload);
        await _context.SaveChangesAsync();

        return ApiResponse<UploadResultDto>.SuccessResponse(MapToDto(upload), "Upload tài liệu thành công!");
    }

    public async Task<PaginatedResponse<UploadResultDto>> GetUserUploadsAsync(int userId, int page, int pageSize, string? fileType = null)
    {
        var query = _context.UserUploads
            .Where(u => u.UserId == userId && !u.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(fileType) && Enum.TryParse<UploadFileType>(fileType, true, out var ft))
            query = query.Where(u => u.FileType == ft);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(u => u.UploadedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResponse<UploadResultDto>
        {
            Success = true,
            Data = items.Select(MapToDto).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalItems = total
        };
    }

    public async Task<ApiResponse<bool>> DeleteUploadAsync(int userId, int uploadId, bool isAdmin = false)
    {
        var upload = await _context.UserUploads.FindAsync(uploadId);
        if (upload == null || upload.IsDeleted)
            return ApiResponse<bool>.ErrorResponse("Không tìm thấy file");

        if (!isAdmin && upload.UserId != userId)
            return ApiResponse<bool>.ErrorResponse("Bạn không có quyền xóa file này");

        // Xóa file vật lý
        DeleteLocalFile(upload.FileUrl);

        upload.IsDeleted = true;
        await _context.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Xóa file thành công");
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private async Task<(string fileUrl, string storedName)> SaveFileAsync(IFormFile file, int userId, string subfolder)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var storedName = $"{Guid.NewGuid():N}{ext}";
        var uploadDir = Path.Combine(_env.WebRootPath, "uploads", userId.ToString(), subfolder);
        Directory.CreateDirectory(uploadDir);

        var filePath = Path.Combine(uploadDir, storedName);
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var fileUrl = $"/uploads/{userId}/{subfolder}/{storedName}";
        return (fileUrl, storedName);
    }

    private void DeleteLocalFile(string? fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl) || !fileUrl.StartsWith("/uploads/")) return;
        try
        {
            var fullPath = Path.Combine(_env.WebRootPath, fileUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Không thể xóa file: {FileUrl}", fileUrl);
        }
    }

    private static (bool IsValid, string? Error) ValidateImageFile(IFormFile file)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedImageTypes.Contains(ext))
            return (false, "Chỉ chấp nhận file ảnh (JPG, PNG, GIF, WebP, BMP, SVG)");
        if (file.Length > MaxImageSize)
            return (false, "Ảnh không được vượt quá 5MB");
        if (file.Length == 0)
            return (false, "File không được rỗng");
        return (true, null);
    }

    private static string FormatFileSize(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
        return $"{bytes / (1024.0 * 1024):F1} MB";
    }

    private static UploadResultDto MapToDto(UserUpload u) => new()
    {
        Id = u.Id,
        OriginalFileName = u.OriginalFileName,
        FileUrl = u.FileUrl,
        ContentType = u.ContentType,
        FileSizeBytes = u.FileSizeBytes,
        FileSizeFormatted = FormatFileSize(u.FileSizeBytes),
        FileType = u.FileType.ToString(),
        PostId = u.PostId,
        UploadedAt = u.UploadedAt
    };

    private static UserDto MapUserDto(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        DisplayName = user.DisplayName,
        AvatarUrl = user.AvatarUrl,
        CoverPhotoUrl = user.CoverPhotoUrl,
        Bio = user.Bio,
        Role = user.Role.ToString(),
        Rank = user.Rank.ToString(),
        ReputationPoints = user.ReputationPoints,
        CreatedAt = user.CreatedAt
    };
}
