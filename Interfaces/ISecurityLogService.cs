using DOAN_LAPTRINHWEB.Models.Entities;
using DOAN_LAPTRINHWEB.Models.DTOs;

namespace DOAN_LAPTRINHWEB.Interfaces;

public interface ISecurityLogService
{
    Task LogAsync(int userId, SecurityAction action, string? ipAddress, string? userAgent, string? description, bool isSuccess = true);
    Task<PaginatedResponse<SecurityLogDto>> GetUserLogsAsync(int userId, int page, int pageSize, DateTime? dateFrom, DateTime? dateTo, SecurityAction? action, bool? isSuccess);
    Task<PaginatedResponse<SecurityLogDto>> GetAllLogsAsync(int page, int pageSize, int? userId, DateTime? dateFrom, DateTime? dateTo, SecurityAction? action, bool? isSuccess);
}
