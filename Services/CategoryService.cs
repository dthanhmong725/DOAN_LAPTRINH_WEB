using Microsoft.EntityFrameworkCore;
using DOAN_LAPTRINHWEB.Data;
using DOAN_LAPTRINHWEB.Interfaces;
using DOAN_LAPTRINHWEB.Models.DTOs;
using DOAN_LAPTRINHWEB.Models.Entities;

namespace DOAN_LAPTRINHWEB.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;

    public CategoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<List<CategoryDto>>> GetAllAsync()
    {
        var categories = await _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Icon = c.Icon,
                Color = c.Color,
                Slug = c.Slug,
                DisplayOrder = c.DisplayOrder,
                IsActive = c.IsActive,
                PostCount = c.PostCount
            })
            .ToListAsync();

        return ApiResponse<List<CategoryDto>>.SuccessResponse(categories);
    }

    public async Task<ApiResponse<CategoryDto>> GetBySlugAsync(string slug)
    {
        var category = await _context.Categories
            .Where(c => c.Slug == slug && c.IsActive)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Icon = c.Icon,
                Color = c.Color,
                Slug = c.Slug,
                DisplayOrder = c.DisplayOrder,
                IsActive = c.IsActive,
                PostCount = c.PostCount
            })
            .FirstOrDefaultAsync();

        if (category == null)
            return ApiResponse<CategoryDto>.ErrorResponse("Category not found");

        return ApiResponse<CategoryDto>.SuccessResponse(category);
    }

    public async Task<ApiResponse<CategoryDto>> CreateAsync(CreateCategoryDto dto, int createdById)
    {
        var slug = GenerateSlug(dto.Name);

        if (await _context.Categories.AnyAsync(c => c.Slug == slug))
            return ApiResponse<CategoryDto>.ErrorResponse("Category with this name already exists");

        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            Icon = dto.Icon,
            Color = dto.Color,
            Slug = slug,
            DisplayOrder = dto.DisplayOrder,
            CreatedById = createdById
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return ApiResponse<CategoryDto>.SuccessResponse(new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Icon = category.Icon,
            Color = category.Color,
            Slug = category.Slug,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive,
            PostCount = category.PostCount
        }, "Category created");
    }

    public async Task<ApiResponse<CategoryDto>> UpdateAsync(int id, CreateCategoryDto dto)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return ApiResponse<CategoryDto>.ErrorResponse("Category not found");

        category.Name = dto.Name;
        category.Description = dto.Description;
        category.Icon = dto.Icon;
        category.Color = dto.Color;
        category.Slug = GenerateSlug(dto.Name);
        category.DisplayOrder = dto.DisplayOrder;

        await _context.SaveChangesAsync();

        return ApiResponse<CategoryDto>.SuccessResponse(new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Icon = category.Icon,
            Color = category.Color,
            Slug = category.Slug,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive,
            PostCount = category.PostCount
        }, "Category updated");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Posts)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return ApiResponse<bool>.ErrorResponse("Không tìm thấy danh mục");

        if (category.Posts.Count > 0)
            return ApiResponse<bool>.ErrorResponse($"Không thể xóa danh mục đang có {category.Posts.Count} bài viết. Hãy xóa hoặc chuyển các bài viết trước.");

        category.IsActive = false;
        await _context.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Đã xóa danh mục");
    }

    private static string GenerateSlug(string name)
    {
        return name.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("&", "and")
            .Replace("+", "plus")
            .Replace("--", "-");
    }
}
