using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DOAN_LAPTRINHWEB.Interfaces;
using DOAN_LAPTRINHWEB.Models.DTOs;
using DOAN_LAPTRINHWEB.Models.ViewModels;

namespace DOAN_LAPTRINHWEB.Controllers.MVC;

/// <summary>MVC Controller - Xác thực (Razor Views)</summary>
[Route("auth")]
public class AuthMvcController : Controller
{
    private readonly IAuthService _authService;
    private readonly IPasswordStrengthService _passwordStrengthService;
    private const string AccessTokenCookie = "access_token";
    private const string RefreshTokenCookie = "refresh_token";

    public AuthMvcController(IAuthService authService, IPasswordStrengthService passwordStrengthService)
    {
        _authService = authService;
        _passwordStrengthService = passwordStrengthService;
    }

    // ─── LOGIN ────────────────────────────────────────────────────────────────

    [HttpGet("login")]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return Redirect(returnUrl ?? "/");
        return View("~/Views/Auth/Login.cshtml", new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View("~/Views/Auth/Login.cshtml", model);

        var dto = new LoginDto { Username = model.Username, Password = model.Password, RememberMe = model.RememberMe };
        var result = await _authService.LoginAsync(dto, GetClientIp(), Request.Headers.UserAgent.ToString());

        if (!result.Success)
        {
            model.ErrorMessage = result.Message;
            return View("~/Views/Auth/Login.cshtml", model);
        }

        SetAuthCookies(result.Data!.AccessToken, result.Data.RefreshToken, model.RememberMe ? 7 : 1);

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);
        return Redirect("/");
    }

    // ─── REGISTER ─────────────────────────────────────────────────────────────

    [HttpGet("register")]
    [AllowAnonymous]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true) return Redirect("/");
        return View("~/Views/Auth/Register.cshtml", new RegisterViewModel());
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View("~/Views/Auth/Register.cshtml", model);

        var dto = new RegisterDto { Username = model.Username, Email = model.Email, Password = model.Password, ConfirmPassword = model.ConfirmPassword };
        var result = await _authService.RegisterAsync(dto, GetClientIp(), Request.Headers.UserAgent.ToString());

        if (!result.Success)
        {
            model.ErrorMessage = result.Message;
            return View("~/Views/Auth/Register.cshtml", model);
        }

        SetAuthCookies(result.Data!.AccessToken, result.Data.RefreshToken, 1);
        model.IsSuccess = true;
        return View("~/Views/Auth/Register.cshtml", model);
    }

    // ─── LOGOUT ───────────────────────────────────────────────────────────────

    [HttpPost("logout")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await TryLogoutAsync();
        ClearAuthCookies();
        return Redirect("/auth/login");
    }

    [HttpGet("signout")]
    public async Task<IActionResult> SignOut()
    {
        await TryLogoutAsync();
        ClearAuthCookies();
        return Redirect("/auth/login");
    }

    // ─── FORGOT PASSWORD ──────────────────────────────────────────────────────

    [HttpGet("forgot-password")]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
        => View("~/Views/Auth/ForgotPassword.cshtml", new ForgotPasswordViewModel());

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View("~/Views/Auth/ForgotPassword.cshtml", model);

        var result = await _authService.ForgotPasswordAsync(
            new ForgotPasswordDto { Email = model.Email }, GetClientIp(), Request.Headers.UserAgent.ToString());

        if (result.Success)
            model.SuccessMessage = result.Message ?? "Email đặt lại mật khẩu đã được gửi.";
        else
            model.ErrorMessage = result.Message;

        return View("~/Views/Auth/ForgotPassword.cshtml", model);
    }

    // ─── RESET PASSWORD ───────────────────────────────────────────────────────

    [HttpGet("reset-password")]
    [AllowAnonymous]
    public IActionResult ResetPassword(string? token = null)
    {
        if (string.IsNullOrEmpty(token)) return Redirect("/auth/forgot-password");
        return View("~/Views/Auth/ResetPassword.cshtml", new ResetPasswordViewModel { Token = token });
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View("~/Views/Auth/ResetPassword.cshtml", model);

        var dto = new ResetPasswordDto { Token = model.Token, NewPassword = model.NewPassword, ConfirmPassword = model.ConfirmPassword };
        var result = await _authService.ResetPasswordAsync(dto, GetClientIp(), Request.Headers.UserAgent.ToString());

        if (!result.Success) { model.ErrorMessage = result.Message; return View("~/Views/Auth/ResetPassword.cshtml", model); }
        model.IsSuccess = true;
        return View("~/Views/Auth/ResetPassword.cshtml", model);
    }

    // ─── VERIFY EMAIL ─────────────────────────────────────────────────────────

    [HttpGet("verify-email")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyEmail(string? token = null)
    {
        var model = new VerifyEmailViewModel { Token = token };
        if (string.IsNullOrEmpty(token)) { model.ErrorMessage = "Token xác thực không hợp lệ."; return View("~/Views/Auth/VerifyEmail.cshtml", model); }

        var result = await _authService.VerifyEmailAsync(token, GetClientIp());
        model.IsSuccess = result.Success;
        model.ErrorMessage = result.Success ? null : result.Message;
        return View("~/Views/Auth/VerifyEmail.cshtml", model);
    }

    // ─── CHANGE PASSWORD ──────────────────────────────────────────────────────

    [HttpGet("change-password")]
    [Authorize]
    public IActionResult ChangePassword()
        => View("~/Views/Auth/ChangePassword.cshtml", new ChangePasswordViewModel());

    [HttpPost("change-password")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View("~/Views/Auth/ChangePassword.cshtml", model);

        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            model.ErrorMessage = "Không xác định được người dùng.";
            return View("~/Views/Auth/ChangePassword.cshtml", model);
        }

        var dto = new ChangePasswordDto { CurrentPassword = model.CurrentPassword, NewPassword = model.NewPassword, ConfirmPassword = model.ConfirmPassword };
        var result = await _authService.ChangePasswordAsync(userId, dto);

        if (!result.Success) { model.ErrorMessage = result.Message; return View("~/Views/Auth/ChangePassword.cshtml", model); }
        model.SuccessMessage = "Đổi mật khẩu thành công!";
        return View("~/Views/Auth/ChangePassword.cshtml", model);
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private async Task TryLogoutAsync()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
        {
            try { await _authService.LogoutAsync(userId, GetClientIp(), Request.Headers.UserAgent.ToString()); }
            catch { /* ignore */ }
        }
    }

    private void SetAuthCookies(string accessToken, string refreshToken, int expiryDays)
    {
        var opts = new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(expiryDays)
        };
        Response.Cookies.Append(AccessTokenCookie, accessToken, opts);
        Response.Cookies.Append(RefreshTokenCookie, refreshToken, opts);
    }

    private void ClearAuthCookies()
    {
        var opts = new CookieOptions { HttpOnly = true, Secure = Request.IsHttps };
        Response.Cookies.Delete(AccessTokenCookie, opts);
        Response.Cookies.Delete(RefreshTokenCookie, opts);
    }

    private string? GetClientIp()
    {
        var fwd = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(fwd)) return fwd.Split(',')[0].Trim();
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
