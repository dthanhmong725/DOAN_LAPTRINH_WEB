using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOAN_LAPTRINHWEB.Controllers.MVC;

[Route("chat")]
[Authorize]
public class ChatMvcController : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View("~/Views/Chat/Index.cshtml");
}
