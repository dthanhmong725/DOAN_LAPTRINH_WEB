using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using DOAN_LAPTRINHWEB.Data;
using DOAN_LAPTRINHWEB.Models.DTOs;
using DOAN_LAPTRINHWEB.Models.Entities;

namespace DOAN_LAPTRINHWEB.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly AppDbContext _context;

    public ChatHub(AppDbContext context)
    {
        _context = context;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = int.Parse(Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _context.Users.FindAsync(userId);

        if (user != null)
        {
            // Add user to all their chat room groups
            var rooms = await _context.ChatRoomMembers
                .Where(crm => crm.UserId == userId)
                .Select(crm => crm.ChatRoomId)
                .ToListAsync();

            foreach (var roomId in rooms)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"room_{roomId}");
            }

            // Broadcast online status
            await Clients.All.SendAsync("UserOnline", new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                DisplayName = user.DisplayName,
                AvatarUrl = user.AvatarUrl
            });
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = int.Parse(Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _context.Users.FindAsync(userId);

        if (user != null)
        {
            await Clients.All.SendAsync("UserOffline", user.Id);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinRoom(int roomId)
    {
        var userId = int.Parse(Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // Verify membership
        var isMember = await _context.ChatRoomMembers
            .AnyAsync(crm => crm.ChatRoomId == roomId && crm.UserId == userId);

        if (!isMember)
        {
            await Clients.Caller.SendAsync("Error", "Access denied");
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"room_{roomId}");

        var user = await _context.Users.FindAsync(userId);
        await Clients.Group($"room_{roomId}").SendAsync("UserJoined", new
        {
            UserId = userId,
            Username = user?.Username,
            DisplayName = user?.DisplayName,
            AvatarUrl = user?.AvatarUrl
        });
    }

    public async Task LeaveRoom(int roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"room_{roomId}");

        var userId = int.Parse(Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _context.Users.FindAsync(userId);

        await Clients.Group($"room_{roomId}").SendAsync("UserLeft", new
        {
            UserId = userId,
            Username = user?.Username
        });
    }

    public async Task SendMessage(int roomId, string content, int? replyToId = null)
    {
        var userId = int.Parse(Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // Verify membership and not muted
        var membership = await _context.ChatRoomMembers
            .FirstOrDefaultAsync(crm => crm.ChatRoomId == roomId && crm.UserId == userId);

        if (membership == null || membership.IsMuted)
        {
            await Clients.Caller.SendAsync("Error", membership?.IsMuted == true ? "You are muted" : "Access denied");
            return;
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return;

        // Rate limiting for chat (max 1 message per second)
        var lastMessage = await _context.ChatMessages
            .Where(m => m.SenderId == userId)
            .OrderByDescending(m => m.CreatedAt)
            .FirstOrDefaultAsync();

        if (lastMessage != null && (DateTime.UtcNow - lastMessage.CreatedAt).TotalSeconds < 1)
        {
            await Clients.Caller.SendAsync("Error", "Please wait before sending another message");
            return;
        }

        var room = await _context.ChatRooms.FindAsync(roomId);
        if (room == null || !room.IsActive) return;

        // Handle file attachments (stored as URLs for now)
        string? attachmentUrl = null;
        string? attachmentType = null;

        // Check if content is a file URL
        if (content.StartsWith("[FILE:") && content.Contains("]"))
        {
            var fileInfo = content.Substring(7, content.IndexOf("]") - 7).Split('|');
            if (fileInfo.Length >= 2)
            {
                attachmentUrl = fileInfo[0];
                attachmentType = fileInfo[1];
                content = content.Substring(content.IndexOf("]") + 1).Trim();
                if (string.IsNullOrEmpty(content))
                    content = $"Sent a file: {Path.GetFileName(attachmentUrl)}";
            }
        }

        var message = new ChatMessage
        {
            Content = content,
            SenderId = userId,
            ChatRoomId = roomId,
            ReplyToId = replyToId,
            CreatedAt = DateTime.UtcNow
        };

        if (!string.IsNullOrEmpty(attachmentUrl))
        {
            message.AttachmentUrl = attachmentUrl;
            message.AttachmentType = attachmentType;
        }

        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync();

        // Update membership last read
        membership.LastReadAt = DateTime.UtcNow;

        // Update room last activity
        room.LastActivityAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var messageDto = new ChatMessageDto
        {
            Id = message.Id,
            Content = message.Content,
            AttachmentUrl = message.AttachmentUrl,
            AttachmentType = message.AttachmentType,
            IsEdited = false,
            IsDeleted = false,
            CreatedAt = message.CreatedAt,
            Sender = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                DisplayName = user.DisplayName,
                AvatarUrl = user.AvatarUrl
            },
            ChatRoomId = roomId,
            ReplyToId = replyToId
        };

        if (replyToId.HasValue)
        {
            var replyTo = await _context.ChatMessages
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.Id == replyToId.Value);

            if (replyTo != null)
            {
                messageDto.ReplyTo = new ChatMessageDto
                {
                    Id = replyTo.Id,
                    Content = replyTo.Content,
                    Sender = new UserDto { Id = replyTo.Sender.Id, Username = replyTo.Sender.Username }
                };
            }
        }

        await Clients.Group($"room_{roomId}").SendAsync("NewMessage", messageDto);
    }

    public async Task EditMessage(int messageId, string newContent)
    {
        var userId = int.Parse(Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var message = await _context.ChatMessages.FindAsync(messageId);
        if (message == null || message.SenderId != userId)
        {
            await Clients.Caller.SendAsync("Error", "Cannot edit this message");
            return;
        }

        message.Content = newContent;
        message.IsEdited = true;
        message.EditedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await Clients.Group($"room_{message.ChatRoomId}").SendAsync("MessageEdited", new
        {
            MessageId = messageId,
            NewContent = newContent,
            EditedAt = message.EditedAt
        });
    }

    public async Task DeleteMessage(int messageId)
    {
        var userId = int.Parse(Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var userRole = Context.User!.FindFirst(ClaimTypes.Role)?.Value;

        var message = await _context.ChatMessages.FindAsync(messageId);
        if (message == null)
        {
            await Clients.Caller.SendAsync("Error", "Message not found");
            return;
        }

        // Only sender, room admin, or global admin can delete
        if (message.SenderId != userId && userRole != "Admin" && userRole != "Moderator")
        {
            await Clients.Caller.SendAsync("Error", "Cannot delete this message");
            return;
        }

        message.IsDeleted = true;
        message.Content = "[Message deleted]";

        await _context.SaveChangesAsync();

        await Clients.Group($"room_{message.ChatRoomId}").SendAsync("MessageDeleted", messageId);
    }

    public async Task SendTypingIndicator(int roomId)
    {
        var userId = int.Parse(Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await _context.Users.FindAsync(userId);

        await Clients.OthersInGroup($"room_{roomId}").SendAsync("UserTyping", new
        {
            UserId = userId,
            Username = user?.Username,
            DisplayName = user?.DisplayName
        });
    }

    public async Task ToggleReaction(int messageId, string emoji)
    {
        if (string.IsNullOrWhiteSpace(emoji) || emoji.Length > 10)
        {
            await Clients.Caller.SendAsync("Error", "Invalid emoji");
            return;
        }

        var userId = int.Parse(Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var message = await _context.ChatMessages
            .Include(m => m.Reactions)
            .FirstOrDefaultAsync(m => m.Id == messageId);

        if (message == null || message.IsDeleted)
        {
            await Clients.Caller.SendAsync("Error", "Message not found");
            return;
        }

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

        // Reload with user info
        message = await _context.ChatMessages
            .Include(m => m.Reactions).ThenInclude(r => r.User)
            .FirstAsync(m => m.Id == messageId);

        var reactions = message.Reactions.Select(r => new
        {
            UserId = r.UserId,
            Username = r.User?.Username ?? "",
            Emoji = r.Emoji,
            CreatedAt = r.CreatedAt
        }).ToList();

        await Clients.Group($"room_{message.ChatRoomId}").SendAsync("MessageReaction", new
        {
            MessageId = messageId,
            Reactions = reactions
        });
    }

    public async Task TogglePinMessage(int messageId)
    {
        var userId = int.Parse(Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var message = await _context.ChatMessages.FindAsync(messageId);

        if (message == null)
        {
            await Clients.Caller.SendAsync("Error", "Message not found");
            return;
        }

        if (message.PinnedAt.HasValue)
        {
            message.PinnedAt = null;
            message.PinnedById = null;
        }
        else
        {
            // Unpin other messages in same room
            var others = await _context.ChatMessages
                .Where(m => m.ChatRoomId == message.ChatRoomId && m.PinnedAt.HasValue)
                .ToListAsync();
            foreach (var p in others)
            {
                p.PinnedAt = null;
                p.PinnedById = null;
            }

            message.PinnedAt = DateTime.UtcNow;
            message.PinnedById = userId;
        }
        await _context.SaveChangesAsync();

        await Clients.Group($"room_{message.ChatRoomId}").SendAsync("MessagePinned", new
        {
            MessageId = messageId,
            PinnedAt = message.PinnedAt,
            PinnedById = message.PinnedById
        });
    }

    public async Task MarkAsRead(int roomId)
    {
        var userId = int.Parse(Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var membership = await _context.ChatRoomMembers
            .FirstOrDefaultAsync(crm => crm.ChatRoomId == roomId && crm.UserId == userId);
        if (membership == null) return;

        membership.LastReadAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        await Clients.OthersInGroup($"room_{roomId}").SendAsync("MessageRead", new
        {
            UserId = userId,
            RoomId = roomId,
            ReadAt = membership.LastReadAt
        });
    }

    public async Task<MentionResult[]> SearchUsers(string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return Array.Empty<MentionResult>();

        var users = await _context.Users
            .Where(u => u.Username.Contains(query) || (u.DisplayName != null && u.DisplayName.Contains(query)))
            .Where(u => u.IsActive && !u.IsBanned)
            .Take(10)
            .Select(u => new MentionResult
            {
                Id = u.Id,
                Username = u.Username,
                DisplayName = u.DisplayName,
                AvatarUrl = u.AvatarUrl
            })
            .ToListAsync();

        return users.ToArray();
    }
}

public class MentionResult
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
}
