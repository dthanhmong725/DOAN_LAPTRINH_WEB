using DOAN_LAPTRINHWEB.Models.DTOs;
using DOAN_LAPTRINHWEB.Models.Entities;

namespace DOAN_LAPTRINHWEB.Interfaces;

public interface INotificationService
{
    /// <summary>Tạo thông báo mới, tự động bỏ qua nếu actor == recipient</summary>
    Task CreateAsync(int recipientId, int actorId, NotificationType type, int? postId = null, int? commentId = null);

    /// <summary>Lấy số thông báo chưa đọc của user</summary>
    Task<int> GetUnreadCountAsync(int userId);

    /// <summary>Lấy danh sách thông báo (phân trang)</summary>
    Task<ApiResponse<List<NotificationDto>>> GetAllAsync(int userId, int page = 1, int pageSize = 20);

    /// <summary>Đánh dấu một thông báo đã đọc</summary>
    Task<ApiResponse<bool>> MarkAsReadAsync(int notificationId, int userId);

    /// <summary>Đánh dấu tất cả thông báo đã đọc</summary>
    Task<ApiResponse<bool>> MarkAllAsReadAsync(int userId);
}
