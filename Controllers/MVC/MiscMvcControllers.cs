using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOAN_LAPTRINHWEB.Controllers.MVC;

[Route("bookmarks")]
[Authorize]
public class BookmarksMvcController : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View("~/Views/Bookmarks/Index.cshtml");
}

[Route("badges")]
public class BadgesMvcController : Controller
{
    [HttpGet("")]
    [AllowAnonymous]
    public IActionResult Index() => View("~/Views/Badges/Index.cshtml");
}

[Route("reputation")]
[Authorize]
public class ReputationMvcController : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View("~/Views/Reputation/Index.cshtml");
}

[Route("upload")]
[Authorize]
public class UploadMvcController : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View("~/Views/Upload/Index.cshtml");
}

[Route("security")]
[Authorize]
public class SecurityMvcController : Controller
{
    [HttpGet("logs")]
    public IActionResult Logs() => View("~/Views/Security/Logs.cshtml");
}
