using DOAN_LAPTRINHWEB.Models.DTOs;
using DOAN_LAPTRINHWEB.Models.Entities;

namespace DOAN_LAPTRINHWEB.Interfaces;

public interface IReputationService
{
    /// <summary>Cộng/trừ điểm cho user kèm ghi log lịch sử</summary>
    Task<int> ApplyChangeAsync(int userId, ReputationAction action, int? actorId = null, int? postId = null, int? commentId = null, string? description = null);

    /// <summary>Lấy tổng quan điểm uy tín của 1 user</summary>
    Task<ApiResponse<ReputationDto>> GetReputationAsync(int userId);

    /// <summary>Lấy lịch sử điểm uy tín (phân trang)</summary>
    Task<PaginatedResponse<ReputationHistoryDto>> GetHistoryAsync(int userId, int page, int pageSize);

    /// <summary>Top user theo điểm uy tín</summary>
    Task<PaginatedResponse<LeaderboardEntryDto>> GetLeaderboardAsync(int page, int pageSize);

    /// <summary>Quy tắc cộng/trừ điểm (để hiển thị tooltip cho user)</summary>
    ReputationRulesDto GetRules();
}

public class ReputationRulesDto
{
    public List<ReputationRuleItem> Rules { get; set; } = new();
    public List<ReputationRankInfo> Ranks { get; set; } = new();
}

public class ReputationRuleItem
{
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Points { get; set; }
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = "#00e5a0";
}

public class ReputationRankInfo
{
    public string Name { get; set; } = string.Empty;
    public int MinPoints { get; set; }
    public string Color { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}
