using Microsoft.EntityFrameworkCore;
using DOAN_LAPTRINHWEB.Data;
using DOAN_LAPTRINHWEB.Interfaces;
using DOAN_LAPTRINHWEB.Models.DTOs;
using DOAN_LAPTRINHWEB.Models.Entities;

namespace DOAN_LAPTRINHWEB.Services;

public class ChatService : IChatService
{
    private readonly AppDbContext _context;

    public ChatService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResponse<ChatRoomDto>> GetUserChatRoomsAsync(int userId, int page, int pageSize)
    {
        var query = _context.ChatRoomMembers
            .Include(crm => crm.ChatRoom)
            .ThenInclude(cr => cr.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
            .ThenInclude(m => m.Sender)
            .Include(crm => crm.ChatRoom)
            .ThenInclude(cr => cr.Members)
            .ThenInclude(m => m.User)
            .Where(crm => crm.UserId == userId && crm.ChatRoom.IsActive)
            .OrderByDescending(crm => crm.ChatRoom.Messages.Max(m => m.CreatedAt));

        var totalItems = await query.CountAsync();
        var rooms = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = rooms.Select(crm =>
        {
            var room = crm.ChatRoom;
            var lastMessage = room.Messages.FirstOrDefault();
            var unreadCount = room.Messages.Count(m => m.SenderId != userId &&
                (!crm.LastReadAt.HasValue || m.CreatedAt > crm.LastReadAt));

            return new ChatRoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Description = room.Description,
                Type = room.Type.ToString(),
                MemberCount = room.MemberCount,
                LastMessage = lastMessage != null ? MapMessageToDto(lastMessage) : null,
                UnreadCount = unreadCount,
                CreatedAt = room.CreatedAt,
                Members = room.Members.Select(m => new UserDto
                {
                    Id = m.User.Id,
                    Username = m.User.Username,
                    DisplayName = m.User.DisplayName,
                    AvatarUrl = m.User.AvatarUrl
                }).ToList()
            };
        }).ToList();

        return new PaginatedResponse<ChatRoomDto>
        {
            Success = true,
            Data = result,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    public async Task<ApiResponse<ChatRoomDto>> GetChatRoomAsync(int roomId, int userId)
    {
        var membership = await _context.ChatRoomMembers
            .Include(crm => crm.ChatRoom)
            .ThenInclude(cr => cr.Members)
            .ThenInclude(m => m.User)
            .Include(crm => crm.ChatRoom)
            .ThenInclude(cr => cr.CreatedBy)
            .FirstOrDefaultAsync(crm => crm.ChatRoomId == roomId && crm.UserId == userId);

        if (membership == null)
            return ApiResponse<ChatRoomDto>.ErrorResponse("Chat room not found or access denied");

        var room = membership.ChatRoom;

        return ApiResponse<ChatRoomDto>.SuccessResponse(new ChatRoomDto
        {
            Id = room.Id,
            Name = room.Name,
            Description = room.Description,
            Type = room.Type.ToString(),
            MemberCount = room.MemberCount,
            CreatedAt = room.CreatedAt,
            Members = room.Members.Select(m => new UserDto
            {
                Id = m.User.Id,
                Username = m.User.Username,
                DisplayName = m.User.DisplayName,
                AvatarUrl = m.User.AvatarUrl
            }).ToList()
        });
    }

    public async Task<ApiResponse<ChatRoomDto>> CreateChatRoomAsync(CreateChatRoomDto dto, int creatorId)
    {
        var creator = await _context.Users.FindAsync(creatorId);
        if (creator == null)
            return ApiResponse<ChatRoomDto>.ErrorResponse("User not found");

        var roomType = dto.MemberIds?.Count > 0 ? ChatRoomType.Group : ChatRoomType.Direct;

        var room = new ChatRoom
        {
            Name = dto.Name,
            Description = dto.Description,
            Type = roomType,
            CreatedById = creatorId,
            CreatedAt = DateTime.UtcNow
        };

        _context.ChatRooms.Add(room);
        await _context.SaveChangesAsync();

        // Add creator as admin
        _context.ChatRoomMembers.Add(new ChatRoomMember
        {
            UserId = creatorId,
            ChatRoomId = room.Id,
            IsAdmin = true,
            JoinedAt = DateTime.UtcNow
        });

        // Add other members
        if (dto.MemberIds != null)
        {
            foreach (var memberId in dto.MemberIds)
            {
                _context.ChatRoomMembers.Add(new ChatRoomMember
                {
                    UserId = memberId,
                    ChatRoomId = room.Id,
                    IsAdmin = false,
                    JoinedAt = DateTime.UtcNow
                });
            }
        }

        room.MemberCount = (dto.MemberIds?.Count ?? 0) + 1;
        await _context.SaveChangesAsync();

        room.CreatedBy = creator;

        return ApiResponse<ChatRoomDto>.SuccessResponse(new ChatRoomDto
        {
            Id = room.Id,
            Name = room.Name,
            Description = room.Description,
            Type = room.Type.ToString(),
            MemberCount = room.MemberCount,
            CreatedAt = room.CreatedAt,
            Members = new List<UserDto>
            {
                new UserDto
                {
                    Id = creator.Id,
                    Username = creator.Username,
                    DisplayName = creator.DisplayName,
                    AvatarUrl = creator.AvatarUrl
                }
            }
        }, "Chat room created");
    }

    public async Task<ApiResponse<bool>> JoinChatRoomAsync(int roomId, int userId)
    {
        var room = await _context.ChatRooms.FindAsync(roomId);
        if (room == null)
            return ApiResponse<bool>.ErrorResponse("Chat room not found");

        var existing = await _context.ChatRoomMembers
            .FirstOrDefaultAsync(crm => crm.ChatRoomId == roomId && crm.UserId == userId);

        if (existing != null)
            return ApiResponse<bool>.SuccessResponse(true, "Already a member");

        _context.ChatRoomMembers.Add(new ChatRoomMember
        {
            UserId = userId,
            ChatRoomId = roomId,
            IsAdmin = false,
            JoinedAt = DateTime.UtcNow
        });

        room.MemberCount++;
        await _context.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Joined chat room");
    }

    public async Task<ApiResponse<bool>> LeaveChatRoomAsync(int roomId, int userId)
    {
        var membership = await _context.ChatRoomMembers
            .Include(crm => crm.ChatRoom)
            .FirstOrDefaultAsync(crm => crm.ChatRoomId == roomId && crm.UserId == userId);

        if (membership == null)
            return ApiResponse<bool>.ErrorResponse("Not a member of this chat room");

        if (membership.IsAdmin && membership.ChatRoom.CreatedById == userId)
            return ApiResponse<bool>.ErrorResponse("Cannot leave - you are the creator");

        membership.ChatRoom.MemberCount = Math.Max(0, membership.ChatRoom.MemberCount - 1);
        _context.ChatRoomMembers.Remove(membership);
        await _context.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Left chat room");
    }

    public async Task<PaginatedResponse<ChatMessageDto>> GetMessagesAsync(int roomId, int page, int pageSize, int userId)
    {
        var membership = await _context.ChatRoomMembers
            .FirstOrDefaultAsync(crm => crm.ChatRoomId == roomId && crm.UserId == userId);

        if (membership == null)
            return new PaginatedResponse<ChatMessageDto>
            {
                Success = false,
                Message = "Access denied",
                Data = new List<ChatMessageDto>()
            };

        var query = _context.ChatMessages
            .Include(m => m.Sender)
            .Include(m => m.ReplyTo)
            .Where(m => m.ChatRoomId == roomId && !m.IsDeleted)
            .OrderByDescending(m => m.CreatedAt);

        var totalItems = await query.CountAsync();
        var messages = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Update last read
        membership.LastReadAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new PaginatedResponse<ChatMessageDto>
        {
            Success = true,
            Data = messages.Select(MapMessageToDto).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    private static ChatMessageDto MapMessageToDto(ChatMessage message) => new()
    {
        Id = message.Id,
        Content = message.Content,
        AttachmentUrl = message.AttachmentUrl,
        AttachmentType = message.AttachmentType,
        IsEdited = message.IsEdited,
        IsDeleted = message.IsDeleted,
        CreatedAt = message.CreatedAt,
        EditedAt = message.EditedAt,
        Sender = new UserDto
        {
            Id = message.Sender.Id,
            Username = message.Sender.Username,
            DisplayName = message.Sender.DisplayName,
            AvatarUrl = message.Sender.AvatarUrl
        },
        ChatRoomId = message.ChatRoomId,
        ReplyToId = message.ReplyToId,
        ReplyTo = message.ReplyTo != null ? new ChatMessageDto
        {
            Id = message.ReplyTo.Id,
            Content = message.ReplyTo.Content,
            Sender = new UserDto
            {
                Id = message.ReplyTo.Sender.Id,
                Username = message.ReplyTo.Sender.Username
            }
        } : null
    };
}
