using DOAN_LAPTRINHWEB.Interfaces;
using DOAN_LAPTRINHWEB.Models.DTOs;
using System.Text.RegularExpressions;

namespace DOAN_LAPTRINHWEB.Services;

public class PasswordStrengthService : IPasswordStrengthService
{
    private static readonly string[] CommonPasswords =
    {
        "password", "123456", "12345678", "qwerty", "abc123", "monkey", "1234567",
        "letmein", "trustno1", "dragon", "baseball", "iloveyou", "master", "sunshine",
        "ashley", "bailey", "shadow", "123123", "654321", "superman", "qazwsx",
        "michael", "football", "password1", "password123", "welcome", "welcome1",
        "admin", "login", "hello", "charlie", "donald", "qwerty123", "password12"
    };

    private static readonly Regex UppercaseRegex = new(@"[A-Z]", RegexOptions.Compiled);
    private static readonly Regex LowercaseRegex = new(@"[a-z]", RegexOptions.Compiled);
    private static readonly Regex DigitRegex = new(@"[0-9]", RegexOptions.Compiled);
    private static readonly Regex SpecialCharRegex = new(@"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]", RegexOptions.Compiled);
    private static readonly Regex RepeatingCharsRegex = new(@"(.)\1{2,}", RegexOptions.Compiled);
    private static readonly Regex SequentialCharsRegex = new(@"(?:abc|bcd|cde|def|efg|fgh|ghi|hij|ijk|jkl|klm|lmn|mno|nop|opq|pqr|qrs|rst|stu|tuv|uvw|vwx|wxy|xyz|012|123|234|345|456|567|678|789)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public PasswordStrengthDto CheckStrength(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return new PasswordStrengthDto
            {
                Score = 0,
                Level = "Không có",
                Suggestions = new List<string> { "Mật khẩu không được để trống" }
            };
        }

        var suggestions = new List<string>();
        int score = 0;

        // Length checks
        bool hasMinLength = password.Length >= 8;
        bool hasGoodLength = password.Length >= 12;
        bool hasGreatLength = password.Length >= 16;

        if (!hasMinLength)
            suggestions.Add("Ít nhất 8 ký tự");
        if (!hasGoodLength)
            suggestions.Add("Nên dùng 12 ký tự trở lên để tăng bảo mật");

        // Character type checks
        bool hasUppercase = UppercaseRegex.IsMatch(password);
        bool hasLowercase = LowercaseRegex.IsMatch(password);
        bool hasDigit = DigitRegex.IsMatch(password);
        bool hasSpecialChar = SpecialCharRegex.IsMatch(password);

        if (!hasUppercase)
            suggestions.Add("Thêm chữ HOA (A-Z)");
        if (!hasLowercase)
            suggestions.Add("Thêm chữ thường (a-z)");
        if (!hasDigit)
            suggestions.Add("Thêm số (0-9)");
        if (!hasSpecialChar)
            suggestions.Add("Thêm ký tự đặc biệt (!@#$%^&*)");

        // Pattern checks
        bool hasRepeating = RepeatingCharsRegex.IsMatch(password);
        bool hasSequential = SequentialCharsRegex.IsMatch(password);
        bool isCommon = CommonPasswords.Contains(password.ToLower());
        bool hasNoCommonPatterns = !hasRepeating && !hasSequential && !isCommon;

        if (hasRepeating)
            suggestions.Add("Tránh lặp ký tự (vd: 'aaa')");
        if (hasSequential)
            suggestions.Add("Tránh chuỗi ký tự liên tiếp (vd: 'abc', '123')");
        if (isCommon)
            suggestions.Add("Tránh mật khẩu phổ biến");

        // Calculate score
        if (hasMinLength) score += 1;
        if (hasGoodLength) score += 1;
        if (hasGreatLength) score += 1;
        if (hasUppercase) score += 1;
        if (hasLowercase) score += 1;
        if (hasDigit) score += 1;
        if (hasSpecialChar) score += 2;
        if (hasNoCommonPatterns) score += 2;

        // Penalties
        if (password.Length < 8) score -= 2;
        if (isCommon) score = Math.Min(score, 2);
        if (hasRepeating) score -= 1;
        if (hasSequential) score -= 1;

        score = Math.Max(0, Math.Min(score, 10));

        string level = score switch
        {
            0 or 1 => "Rất yếu",
            2 or 3 => "Yếu",
            4 or 5 => "Trung bình",
            6 or 7 => "Mạnh",
            8 or 9 => "Rất mạnh",
            10 => "Hoàn hảo",
            _ => "Không xác định"
        };

        return new PasswordStrengthDto
        {
            Score = score,
            Level = level,
            Suggestions = suggestions.Take(5).ToList(),
            HasMinLength = hasMinLength,
            HasUppercase = hasUppercase,
            HasLowercase = hasLowercase,
            HasDigit = hasDigit,
            HasSpecialChar = hasSpecialChar,
            HasNoCommonPatterns = hasNoCommonPatterns
        };
    }
}
