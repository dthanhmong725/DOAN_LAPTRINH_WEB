using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DOAN_LAPTRINHWEB.Controllers.MVC;

[Route("posts")]
public class PostsMvcController : Controller
{
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public IActionResult Detail(int id)
    {
        ViewData["PostId"] = id;
        return View("~/Views/Posts/Detail.cshtml");
    }

    [HttpGet("create")]
    [Authorize]
    public IActionResult Create() => View("~/Views/Posts/Create.cshtml");

    [HttpGet("{id:int}/edit")]
    [Authorize]
    public IActionResult Edit(int id)
    {
        ViewData["PostId"] = id;
        return View("~/Views/Posts/Edit.cshtml");
    }
}
