using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOAN_LAPTRINHWEB.Controllers.MVC;

[Route("")]
public class HomeMvcController : Controller
{
    [HttpGet("")]
    [HttpGet("home")]
    [AllowAnonymous]
    public IActionResult Index() => View("~/Views/Home/Index.cshtml");

    [HttpGet("search")]
    [AllowAnonymous]
    public IActionResult Search(string? q = null)
    {
        ViewData["SearchQuery"] = q ?? string.Empty;
        return View("~/Views/Home/Search.cshtml");
    }

    [HttpGet("leaderboard")]
    [AllowAnonymous]
    public IActionResult Leaderboard() => View("~/Views/Home/Leaderboard.cshtml");
}
