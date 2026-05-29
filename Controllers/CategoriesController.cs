using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DOAN_LAPTRINHWEB.Interfaces;
using DOAN_LAPTRINHWEB.Models.DTOs;
using DOAN_LAPTRINHWEB.Authorization;

namespace DOAN_LAPTRINHWEB.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly IPostService _postService;

    public CategoriesController(ICategoryService categoryService, IPostService postService)
    {
        _categoryService = categoryService;
        _postService = postService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _categoryService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var result = await _categoryService.GetBySlugAsync(slug);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpGet("{slug}/posts")]
    public async Task<IActionResult> GetPostsBySlug(
        string slug,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var categoryResult = await _categoryService.GetBySlugAsync(slug);
        if (!categoryResult.Success)
            return NotFound(ApiResponse<object>.ErrorResponse("Không tìm thấy danh mục"));

        var postsResult = await _postService.GetAllAsync(page, pageSize, categoryResult.Data!.Id, null, null);

        return Ok(new
        {
            success = true,
            data = postsResult.Data,
            category = categoryResult.Data,
            page = postsResult.Page,
            pageSize = postsResult.PageSize,
            totalItems = postsResult.TotalItems,
            totalPages = postsResult.TotalPages
        });
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.ErrorResponse("Dữ liệu không hợp lệ"));

        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _categoryService.CreateAsync(dto, userId);

        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetBySlug), new { slug = result.Data?.Slug }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
    public async Task<IActionResult> Update(int id, [FromBody] CreateCategoryDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.ErrorResponse("Dữ liệu không hợp lệ"));

        var result = await _categoryService.UpdateAsync(id, dto);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _categoryService.DeleteAsync(id);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
