using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Ganss.Xss;
using DOAN_LAPTRINHWEB.Data;
using DOAN_LAPTRINHWEB.Interfaces;
using DOAN_LAPTRINHWEB.Models.DTOs;
using DOAN_LAPTRINHWEB.Models.Entities;

namespace DOAN_LAPTRINHWEB.Services;

public class CommentService : ICommentService
{
    private readonly AppDbContext _context;
    private readonly IActivityLogService _activityLog;
    private readonly IBadgeService _badgeService;
    private readonly IHtmlSanitizer _htmlSanitizer;
    private readonly INotificationService _notificationService;
    private readonly IReputationService _reputationService;

    public CommentService(
        AppDbContext context,
        IActivityLogService activityLog,
        IBadgeService badgeService,
        INotificationService notificationService,
        IReputationService reputationService)
    {
        _context = context;
        _activityLog = activityLog;
        _badgeService = badgeService;
        _htmlSanitizer = new HtmlSanitizer();
        _notificationService = notificationService;
        _reputationService = reputationService;
    }

    public async Task<PaginatedResponse<CommentDto>> GetByPostAsync(int postId, int page, int pageSize, int? userId)
    {
        // Get only top-level comments (no parent)
        var query = _context.Comments
            .Include(c => c.Author)
            .Include(c => c.Replies).ThenInclude(r => r.Author)
            .Include(c => c.Votes)
            .Where(c => c.PostId == postId && c.ParentCommentId == null && !c.IsDeleted)
            .OrderByDescending(c => c.UpvoteCount - c.DownvoteCount)
            .ThenBy(c => c.CreatedAt);

        var totalItems = await query.CountAsync();
        var comments = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = comments.Select(c => MapToDto(c, userId)).ToList();

        return new PaginatedResponse<CommentDto>
        {
            Success = true,
            Data = result,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    public async Task<ApiResponse<CommentDto>> CreateAsync(int postId, CreateCommentDto dto, int authorId)
    {
        var post = await _context.Posts.FindAsync(postId);
        if (post == null)
            return ApiResponse<CommentDto>.ErrorResponse("Post not found");

        if (post.IsLocked)
            return ApiResponse<CommentDto>.ErrorResponse("This post is locked");

        var author = await _context.Users.FindAsync(authorId);
        if (author == null)
            return ApiResponse<CommentDto>.ErrorResponse("User not found");

        if (dto.ParentCommentId.HasValue)
        {
            var parent = await _context.Comments.FindAsync(dto.ParentCommentId.Value);
            if (parent == null || parent.PostId != postId)
                return ApiResponse<CommentDto>.ErrorResponse("Parent comment not found");
        }

        var comment = new Comment
        {
            Content = _htmlSanitizer.Sanitize(dto.Content),
            AuthorId = authorId,
            PostId = postId,
            ParentCommentId = dto.ParentCommentId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);
        post.CommentCount++;
        post.LastActivityAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        await _activityLog.LogAsync(ActivityType.CommentCreate, authorId, null, null, $"Comment on post: {post.Title}");

        // ===== TÍCH HỢP REPUTATION: Cộng điểm khi bình luận =====
        await _reputationService.ApplyChangeAsync(
            userId: authorId,
            action: ReputationAction.CommentCreated,
            actorId: authorId,
            postId: postId,
            commentId: comment.Id,
            description: $"Đã bình luận vào bài viết: {post.Title}");

        await _badgeService.CheckAndAwardBadgesAsync(authorId);

        // Notify parent comment author if it's a reply
        if (dto.ParentCommentId.HasValue)
        {
            var parent = await _context.Comments.FindAsync(dto.ParentCommentId.Value);
            if (parent != null && parent.AuthorId != authorId)
            {
                await _notificationService.CreateAsync(parent.AuthorId, authorId, NotificationType.Comment, postId, comment.Id);
            }
        }
        else if (authorId != post.AuthorId) // Notify post author if it's a direct comment
        {
            await _notificationService.CreateAsync(post.AuthorId, authorId, NotificationType.Comment, postId, comment.Id);
        }

        // Handle Mentions
        var mentionMatches = Regex.Matches(dto.Content, @"@([a-zA-Z0-9_]+)");
        foreach (Match match in mentionMatches)
        {
            var username = match.Groups[1].Value;
            var mentionedUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (mentionedUser != null && mentionedUser.Id != authorId)
            {
                await _notificationService.CreateAsync(mentionedUser.Id, authorId, NotificationType.Mention, postId, comment.Id);
            }
        }

        comment.Author = author;

        return ApiResponse<CommentDto>.SuccessResponse(MapToDto(comment, authorId), "Comment added");
    }

    public async Task<ApiResponse<CommentDto>> UpdateAsync(int commentId, UpdateCommentDto dto, int userId)
    {
        var comment = await _context.Comments
            .Include(c => c.Author)
            .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);

        if (comment == null)
            return ApiResponse<CommentDto>.ErrorResponse("Comment not found");

        if (comment.AuthorId != userId)
            return ApiResponse<CommentDto>.ErrorResponse("You can only edit your own comments");

        comment.Content = _htmlSanitizer.Sanitize(dto.Content);
        comment.IsEdited = true;
        comment.EditedAt = DateTime.UtcNow;
        comment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await _activityLog.LogAsync(ActivityType.CommentEdit, userId, null, null, $"Comment edited on post ID: {comment.PostId}");

        return ApiResponse<CommentDto>.SuccessResponse(MapToDto(comment, userId), "Comment updated");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int commentId, int userId, UserRole userRole)
    {
        var comment = await _context.Comments
            .Include(c => c.Post)
            .FirstOrDefaultAsync(c => c.Id == commentId);

        if (comment == null)
            return ApiResponse<bool>.ErrorResponse("Comment not found");

        if (comment.AuthorId != userId && userRole == UserRole.User)
            return ApiResponse<bool>.ErrorResponse("You can only delete your own comments");

        comment.IsDeleted = true;
        comment.Post.CommentCount = Math.Max(0, comment.Post.CommentCount - 1);

        await _context.SaveChangesAsync();
        await _activityLog.LogAsync(ActivityType.CommentDelete, userId, null, null, $"Comment deleted on post ID: {comment.PostId}");

        // ===== TÍCH HỢP REPUTATION: Trừ điểm khi bình luận bị xóa =====
        await _reputationService.ApplyChangeAsync(
            userId: comment.AuthorId,
            action: ReputationAction.CommentDeleted,
            actorId: userId,
            postId: comment.PostId,
            commentId: commentId,
            description: "Bình luận đã bị xóa");

        return ApiResponse<bool>.SuccessResponse(true, "Comment deleted");
    }

    public async Task<ApiResponse<bool>> VoteAsync(int commentId, int userId, bool isUpvote)
    {
        var comment = await _context.Comments
            .Include(c => c.Author)
            .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);

        if (comment == null)
            return ApiResponse<bool>.ErrorResponse("Comment not found");

        // Không vote comment của chính mình
        if (comment.AuthorId == userId)
            return ApiResponse<bool>.ErrorResponse("Bạn không thể vote bình luận của chính mình");

        var existingVote = await _context.CommentVotes
            .FirstOrDefaultAsync(v => v.CommentId == commentId && v.UserId == userId);

        if (existingVote != null)
        {
            if (existingVote.IsUpvote == isUpvote)
            {
                // Bỏ vote
                _context.CommentVotes.Remove(existingVote);
                if (isUpvote) comment.UpvoteCount--;
                else comment.DownvoteCount--;

                // Hoàn điểm
                await _reputationService.ApplyChangeAsync(
                    userId: comment.AuthorId,
                    action: isUpvote ? ReputationAction.UpvoteRemoved : ReputationAction.DownvoteRemoved,
                    actorId: userId,
                    commentId: commentId);
            }
            else
            {
                // Đổi chiều vote
                if (existingVote.IsUpvote)
                {
                    comment.UpvoteCount--;
                    comment.DownvoteCount++;
                }
                else
                {
                    comment.DownvoteCount--;
                    comment.UpvoteCount++;
                }
                existingVote.IsUpvote = isUpvote;

                var reverseAction = existingVote.IsUpvote ? ReputationAction.CommentDownvoted : ReputationAction.CommentUpvoted;
                var newAction = existingVote.IsUpvote ? ReputationAction.CommentUpvoted : ReputationAction.CommentDownvoted;
                await _reputationService.ApplyChangeAsync(comment.AuthorId, reverseAction, userId, null, commentId);
                await _reputationService.ApplyChangeAsync(comment.AuthorId, newAction, userId, null, commentId);

                await _notificationService.CreateAsync(comment.AuthorId, userId, isUpvote ? NotificationType.CommentUpvote : NotificationType.CommentDownvote, comment.PostId, commentId);
            }
        }
        else
        {
            // Vote mới
            _context.CommentVotes.Add(new CommentVote
            {
                CommentId = commentId,
                UserId = userId,
                IsUpvote = isUpvote
            });

            if (isUpvote) comment.UpvoteCount++;
            else comment.DownvoteCount++;

            // ===== TÍCH HỢP REPUTATION =====
            var action = isUpvote ? ReputationAction.CommentUpvoted : ReputationAction.CommentDownvoted;
            await _reputationService.ApplyChangeAsync(
                userId: comment.AuthorId,
                action: action,
                actorId: userId,
                commentId: commentId);

            await _notificationService.CreateAsync(comment.AuthorId, userId, isUpvote ? NotificationType.CommentUpvote : NotificationType.CommentDownvote, comment.PostId, commentId);
        }

        await _context.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true);
    }

    public async Task<ApiResponse<bool>> RemoveVoteAsync(int commentId, int userId)
    {
        var vote = await _context.CommentVotes
            .FirstOrDefaultAsync(v => v.CommentId == commentId && v.UserId == userId);

        if (vote == null)
            return ApiResponse<bool>.ErrorResponse("Vote not found");

        var comment = await _context.Comments.FindAsync(commentId);
        if (comment == null)
            return ApiResponse<bool>.ErrorResponse("Comment not found");

        if (vote.IsUpvote) comment.UpvoteCount--;
        else comment.DownvoteCount--;

        _context.CommentVotes.Remove(vote);
        await _context.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true);
    }

    private CommentDto MapToDto(Comment comment, int? userId)
    {
        int? userVote = null;
        if (userId.HasValue)
        {
            var vote = comment.Votes?.FirstOrDefault(v => v.UserId == userId.Value);
            if (vote != null)
                userVote = vote.IsUpvote ? 1 : -1;
        }

        return new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            IsEdited = comment.IsEdited,
            UpvoteCount = comment.UpvoteCount,
            DownvoteCount = comment.DownvoteCount,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            Author = new UserDto
            {
                Id = comment.Author.Id,
                Username = comment.Author.Username,
                DisplayName = comment.Author.DisplayName,
                AvatarUrl = comment.Author.AvatarUrl,
                Role = comment.Author.Role.ToString(),
                Rank = comment.Author.Rank.ToString(),
                ReputationPoints = comment.Author.ReputationPoints
            },
            ParentCommentId = comment.ParentCommentId,
            UserVote = userVote,
            Replies = comment.Replies?.Where(r => !r.IsDeleted).OrderBy(r => r.CreatedAt).Select(r => MapToDto(r, userId)).ToList() ?? new List<CommentDto>()
        };
    }
}
