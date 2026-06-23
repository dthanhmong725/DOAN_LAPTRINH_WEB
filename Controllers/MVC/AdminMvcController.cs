using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOAN_LAPTRINHWEB.Controllers.MVC;

[Route("admin")]
[Authorize]
public class AdminMvcController : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View("~/Views/Admin/Index.cshtml");

    [HttpGet("moderator")]
    public IActionResult Moderator() => View("~/Views/Admin/Moderator.cshtml");
}
