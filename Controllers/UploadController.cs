using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DOAN_LAPTRINHWEB.Interfaces;
using DOAN_LAPTRINHWEB.Models.DTOs;

namespace DOAN_LAPTRINHWEB.Controllers;

/// <summary>Quản lý upload file: avatar, ảnh bìa, tài liệu (PDF, ảnh, source code, log)</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UploadController : ControllerBase
{
    private readonly IUploadService _uploadService;

    public UploadController(IUploadService uploadService)
    {
        _uploadService = uploadService;
    }

    /// <summary>Upload avatar cá nhân (ảnh JPG/PNG/GIF/WebP, tối đa 5MB)</summary>
    [HttpPost("avatar")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<object>.ErrorResponse("Vui lòng chọn file ảnh"));

        var userId = GetUserId();
        var result = await _uploadService.UploadAvatarAsync(userId, file);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>Upload ảnh bìa hồ sơ (ảnh JPG/PNG/GIF/WebP, tối đa 5MB)</summary>
    [HttpPost("cover")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadCoverPhoto(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<object>.ErrorResponse("Vui lòng chọn file ảnh"));

        var userId = GetUserId();
        var result = await _uploadService.UploadCoverPhotoAsync(userId, file);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Upload tài liệu đa dạng: PDF, ảnh, source code (.cs/.js/.py/.ts/...), file log (.log/.txt).
    /// Tối đa 5MB cho ảnh, 20MB cho tài liệu khác.
    /// </summary>
    [HttpPost("document")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadDocument(IFormFile file, [FromForm] int? postId = null)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<object>.ErrorResponse("Vui lòng chọn file để upload"));

        var userId = GetUserId();
        var result = await _uploadService.UploadDocumentAsync(userId, file, postId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>Lấy danh sách file đã upload của bản thân</summary>
    [HttpGet("my-files")]
    public async Task<IActionResult> GetMyFiles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? fileType = null)
    {
        var userId = GetUserId();
        var result = await _uploadService.GetUserUploadsAsync(userId, page, pageSize, fileType);
        return Ok(result);
    }

    /// <summary>Xóa file upload (chỉ chủ sở hữu mới có quyền)</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteFile(int id)
    {
        var userId = GetUserId();
        var result = await _uploadService.DeleteUploadAsync(userId, id);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    private int GetUserId() =>
        int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
}
