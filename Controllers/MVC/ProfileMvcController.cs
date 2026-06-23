using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOAN_LAPTRINHWEB.Controllers.MVC;

[Route("users")]
public class ProfileMvcController : Controller
{
    [HttpGet("profile")]
    [AllowAnonymous]
    public IActionResult Profile(string? username = null)
    {
        ViewData["ProfileUsername"] = username;
        return View("~/Views/Users/Profile.cshtml");
    }

    [HttpGet("settings")]
    [Authorize]
    public IActionResult Settings() => View("~/Views/Users/Settings.cshtml");
}
