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
            .Include(m => m.ReplyTo).ThenInclude(r => r!.Sender)
            .Include(m => m.Reactions).ThenInclude(r => r.User)
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
        PinnedAt = message.PinnedAt,
        PinnedById = message.PinnedById,
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
        } : null,
        Reactions = (message.Reactions ?? new List<ChatMessageReaction>()).Select(r => new ReactionDto
        {
            UserId = r.UserId,
            Username = r.User?.Username ?? string.Empty,
            Emoji = r.Emoji,
            CreatedAt = r.CreatedAt
        }).ToList()
    };

    public async Task<ApiResponse<ChatMessageDto>> SendMessageAsync(int roomId, int userId, string content, string? attachmentUrl, string? attachmentType, int? replyToId)
    {
        if (string.IsNullOrWhiteSpace(content))
            return ApiResponse<ChatMessageDto>.ErrorResponse("Nội dung không được trống");

        var membership = await _context.ChatRoomMembers
            .FirstOrDefaultAsync(crm => crm.ChatRoomId == roomId && crm.UserId == userId);
        if (membership == null)
            return ApiResponse<ChatMessageDto>.ErrorResponse("Bạn không phải thành viên của phòng chat này");

        if (membership.IsMuted)
            return ApiResponse<ChatMessageDto>.ErrorResponse("Bạn đã bị tắt tiếng trong phòng chat này");

        ChatMessage? replyTo = null;
        if (replyToId.HasValue)
        {
            replyTo = await _context.ChatMessages
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.Id == replyToId.Value && m.ChatRoomId == roomId && !m.IsDeleted);
        }

        var message = new ChatMessage
        {
            Content = content.Trim(),
            AttachmentUrl = attachmentUrl,
            AttachmentType = attachmentType,
            ChatRoomId = roomId,
            SenderId = userId,
            ReplyToId = replyTo?.Id,
            CreatedAt = DateTime.UtcNow
        };

        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync();

        // Reload with includes
        message = await _context.ChatMessages
            .Include(m => m.Sender)
            .Include(m => m.ReplyTo).ThenInclude(r => r!.Sender)
            .Include(m => m.Reactions).ThenInclude(r => r.User)
            .FirstAsync(m => m.Id == message.Id);

        return ApiResponse<ChatMessageDto>.SuccessResponse(MapMessageToDto(message), "Tin nhắn đã được gửi");
    }

    public async Task<ApiResponse<bool>> EditMessageAsync(int messageId, int userId, string newContent)
    {
        if (string.IsNullOrWhiteSpace(newContent))
            return ApiResponse<bool>.ErrorResponse("Nội dung không được trống");

        var message = await _context.ChatMessages.FindAsync(messageId);
        if (message == null)
            return ApiResponse<bool>.ErrorResponse("Tin nhắn không tồn tại");
        if (message.SenderId != userId)
            return ApiResponse<bool>.ErrorResponse("Bạn không có quyền sửa tin nhắn này");
        if (message.IsDeleted)
            return ApiResponse<bool>.ErrorResponse("Không thể sửa tin nhắn đã xóa");

        message.Content = newContent.Trim();
        message.IsEdited = true;
        message.EditedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Đã sửa tin nhắn");
    }

    public async Task<ApiResponse<bool>> DeleteMessageAsync(int messageId, int userId)
    {
        var message = await _context.ChatMessages.FindAsync(messageId);
        if (message == null)
            return ApiResponse<bool>.ErrorResponse("Tin nhắn không tồn tại");
        if (message.SenderId != userId)
            return ApiResponse<bool>.ErrorResponse("Bạn không có quyền xóa tin nhắn này");

        message.IsDeleted = true;
        message.Content = "[Đã xóa]";
        await _context.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Đã xóa tin nhắn");
    }

    public async Task<ApiResponse<bool>> ToggleReactionAsync(int messageId, int userId, string emoji)
    {
        if (string.IsNullOrWhiteSpace(emoji) || emoji.Length > 10)
            return ApiResponse<bool>.ErrorResponse("Emoji không hợp lệ");

        var message = await _context.ChatMessages
            .Include(m => m.Reactions)
            .FirstOrDefaultAsync(m => m.Id == messageId);

        if (message == null || message.IsDeleted)
            return ApiResponse<bool>.ErrorResponse("Tin nhắn không tồn tại");

        // Verify user is member of the chat room
        var isMember = await _context.ChatRoomMembers
            .AnyAsync(crm => crm.ChatRoomId == message.ChatRoomId && crm.UserId == userId);
        if (!isMember)
            return ApiResponse<bool>.ErrorResponse("Bạn không phải thành viên của phòng");

        var existing = message.Reactions.FirstOrDefault(r => r.UserId == userId && r.Emoji == emoji);
        if (existing != null)
        {
            _context.ChatMessageReactions.Remove(existing);
        }
        else
        {
            _context.ChatMessageReactions.Add(new ChatMessageReaction
            {
                MessageId = messageId,
                UserId = userId,
                Emoji = emoji,
                CreatedAt = DateTime.UtcNow
            });
        }
        await _context.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, existing != null ? "Đã bỏ reaction" : "Đã thêm reaction");
    }

    public async Task<ApiResponse<bool>> TogglePinMessageAsync(int messageId, int userId)
    {
        var message = await _context.ChatMessages.FindAsync(messageId);
        if (message == null)
            return ApiResponse<bool>.ErrorResponse("Tin nhắn không tồn tại");

        // Verify user is admin of the chat room
        var membership = await _context.ChatRoomMembers
            .FirstOrDefaultAsync(crm => crm.ChatRoomId == message.ChatRoomId && crm.UserId == userId);
        if (membership == null)
            return ApiResponse<bool>.ErrorResponse("Bạn không phải thành viên của phòng");

        if (message.PinnedAt.HasValue)
        {
            message.PinnedAt = null;
            message.PinnedById = null;
        }
        else
        {
            // Unpin other messages in the same room (only 1 pinned at a time)
            var otherPinned = await _context.ChatMessages
                .Where(m => m.ChatRoomId == message.ChatRoomId && m.PinnedAt.HasValue)
                .ToListAsync();
            foreach (var p in otherPinned)
            {
                p.PinnedAt = null;
                p.PinnedById = null;
            }

            message.PinnedAt = DateTime.UtcNow;
            message.PinnedById = userId;
        }
        await _context.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, message.PinnedAt.HasValue ? "Đã ghim" : "Đã bỏ ghim");
    }

    public async Task<ApiResponse<ChatMessageDto?>> GetPinnedMessageAsync(int roomId, int userId)
    {
        var membership = await _context.ChatRoomMembers
            .AnyAsync(crm => crm.ChatRoomId == roomId && crm.UserId == userId);
        if (!membership)
            return ApiResponse<ChatMessageDto?>.ErrorResponse("Không có quyền truy cập");

        var pinned = await _context.ChatMessages
            .Include(m => m.Sender)
            .Include(m => m.Reactions).ThenInclude(r => r.User)
            .FirstOrDefaultAsync(m => m.ChatRoomId == roomId && m.PinnedAt.HasValue && !m.IsDeleted);

        return ApiResponse<ChatMessageDto?>.SuccessResponse(pinned != null ? MapMessageToDto(pinned) : null);
    }

    public async Task<PaginatedResponse<ChatMessageDto>> SearchMessagesAsync(int roomId, int userId, string term, int page, int pageSize)
    {
        var membership = await _context.ChatRoomMembers
            .AnyAsync(crm => crm.ChatRoomId == roomId && crm.UserId == userId);
        if (!membership)
            return new PaginatedResponse<ChatMessageDto>
            {
                Success = false,
                Message = "Không có quyền truy cập",
                Data = new List<ChatMessageDto>()
            };

        var query = _context.ChatMessages
            .Include(m => m.Sender)
            .Include(m => m.ReplyTo).ThenInclude(r => r!.Sender)
            .Include(m => m.Reactions).ThenInclude(r => r.User)
            .Where(m => m.ChatRoomId == roomId && !m.IsDeleted && m.Content.Contains(term));

        var totalItems = await query.CountAsync();
        var messages = await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResponse<ChatMessageDto>
        {
            Success = true,
            Data = messages.Select(MapMessageToDto).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    public async Task<ApiResponse<bool>> AddMemberAsync(int roomId, int adderUserId, string username)
    {
        var adderMembership = await _context.ChatRoomMembers
            .FirstOrDefaultAsync(crm => crm.ChatRoomId == roomId && crm.UserId == adderUserId);
        if (adderMembership == null)
            return ApiResponse<bool>.ErrorResponse("Bạn không phải thành viên của phòng");

        var userToAdd = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (userToAdd == null)
            return ApiResponse<bool>.ErrorResponse("Không tìm thấy người dùng");

        var existing = await _context.ChatRoomMembers
            .FirstOrDefaultAsync(crm => crm.ChatRoomId == roomId && crm.UserId == userToAdd.Id);
        if (existing != null)
            return ApiResponse<bool>.ErrorResponse("Người dùng đã là thành viên");

        var room = await _context.ChatRooms.FindAsync(roomId);
        if (room == null)
            return ApiResponse<bool>.ErrorResponse("Phòng chat không tồn tại");

        _context.ChatRoomMembers.Add(new ChatRoomMember
        {
            UserId = userToAdd.Id,
            ChatRoomId = roomId,
            IsAdmin = false,
            JoinedAt = DateTime.UtcNow
        });
        room.MemberCount++;
        await _context.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, $"Đã thêm {username} vào phòng");
    }

    public async Task<ApiResponse<bool>> MarkAsReadAsync(int roomId, int userId)
    {
        var membership = await _context.ChatRoomMembers
            .FirstOrDefaultAsync(crm => crm.ChatRoomId == roomId && crm.UserId == userId);
        if (membership == null)
            return ApiResponse<bool>.ErrorResponse("Không có quyền truy cập");

        membership.LastReadAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Đã đánh dấu đã đọc");
    }

    public async Task<ApiResponse<bool>> UpdateLastReadAsync(int roomId, int userId)
    {
        return await MarkAsReadAsync(roomId, userId);
    }

    public async Task<int> GetTotalUnreadCountAsync(int userId)
    {
        var memberships = await _context.ChatRoomMembers
            .Include(crm => crm.ChatRoom)
            .ThenInclude(cr => cr.Messages)
            .Where(crm => crm.UserId == userId && crm.ChatRoom.IsActive)
            .ToListAsync();

        var total = 0;
        foreach (var crm in memberships)
        {
            total += crm.ChatRoom.Messages.Count(m => m.SenderId != userId &&
                (!crm.LastReadAt.HasValue || m.CreatedAt > crm.LastReadAt));
        }

        return total;
    }
}
