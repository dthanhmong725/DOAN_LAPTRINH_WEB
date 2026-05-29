using DOAN_LAPTRINHWEB.Models.DTOs;
using DOAN_LAPTRINHWEB.Models.Entities;

namespace DOAN_LAPTRINHWEB.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto, string? ipAddress, string? userAgent);
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto, string? ipAddress, string? userAgent);
    Task<ApiResponse<bool>> LogoutAsync(int userId, string? ipAddress, string? userAgent);
    Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordDto dto, string? ipAddress, string? userAgent);
    Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordDto dto, string? ipAddress, string? userAgent);
    Task<ApiResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordDto dto);
    Task<ApiResponse<bool>> VerifyEmailAsync(string token, string? ipAddress);
    Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(string refreshToken, string? ipAddress);
    string GenerateJwtToken(User user);
    string GenerateSecureToken(int length);
}
