using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DOAN_LAPTRINHWEB.Interfaces;
using DOAN_LAPTRINHWEB.Models.DTOs;

namespace DOAN_LAPTRINHWEB.Controllers;

[ApiController]
[Route("api/chat")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet("rooms")]
    public async Task<IActionResult> GetChatRooms([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _chatService.GetUserChatRoomsAsync(userId, page, pageSize);
        return Ok(result);
    }

    [HttpGet("rooms/{roomId}")]
    public async Task<IActionResult> GetChatRoom(int roomId)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _chatService.GetChatRoomAsync(roomId, userId);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost("rooms")]
    public async Task<IActionResult> CreateChatRoom([FromBody] CreateChatRoomDto dto)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _chatService.CreateChatRoomAsync(dto, userId);

        if (!result.Success)
            return BadRequest(result);

        return Created($"/api/chat/rooms/{result.Data?.Id}", result);
    }

    [HttpPost("rooms/{roomId}/join")]
    public async Task<IActionResult> JoinChatRoom(int roomId)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _chatService.JoinChatRoomAsync(roomId, userId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("rooms/{roomId}/leave")]
    public async Task<IActionResult> LeaveChatRoom(int roomId)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _chatService.LeaveChatRoomAsync(roomId, userId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("rooms/{roomId}/messages")]
    public async Task<IActionResult> GetMessages(int roomId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _chatService.GetMessagesAsync(roomId, page, pageSize, userId);

        if (!result.Success)
            return Unauthorized(result);

        return Ok(result);
    }
}
