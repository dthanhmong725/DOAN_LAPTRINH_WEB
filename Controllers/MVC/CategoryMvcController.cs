using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOAN_LAPTRINHWEB.Controllers.MVC;

[Route("categories")]
public class CategoryMvcController : Controller
{
    [HttpGet("")]
    [AllowAnonymous]
    public IActionResult Index() => View("~/Views/Categories/Index.cshtml");

    [HttpGet("{slug}")]
    [AllowAnonymous]
    public IActionResult Detail(string slug)
    {
        ViewData["CategorySlug"] = slug;
        return View("~/Views/Categories/Detail.cshtml");
    }
}
