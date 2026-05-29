using Microsoft.EntityFrameworkCore;
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

    public CommentService(AppDbContext context, IActivityLogService activityLog, IBadgeService badgeService)
    {
        _context = context;
        _activityLog = activityLog;
        _badgeService = badgeService;
        _htmlSanitizer = new HtmlSanitizer();
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
        await _badgeService.CheckAndAwardBadgesAsync(authorId);

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

        return ApiResponse<bool>.SuccessResponse(true, "Comment deleted");
    }

    public async Task<ApiResponse<bool>> VoteAsync(int commentId, int userId, bool isUpvote)
    {
        var comment = await _context.Comments
            .Include(c => c.Author)
            .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);

        if (comment == null)
            return ApiResponse<bool>.ErrorResponse("Comment not found");

        var existingVote = await _context.CommentVotes
            .FirstOrDefaultAsync(v => v.CommentId == commentId && v.UserId == userId);

        if (existingVote != null)
        {
            if (existingVote.IsUpvote == isUpvote)
            {
                _context.CommentVotes.Remove(existingVote);
                if (isUpvote)
                    comment.UpvoteCount--;
                else
                    comment.DownvoteCount--;
            }
            else
            {
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
            }
        }
        else
        {
            _context.CommentVotes.Add(new CommentVote
            {
                CommentId = commentId,
                UserId = userId,
                IsUpvote = isUpvote
            });

            if (isUpvote)
                comment.UpvoteCount++;
            else
                comment.DownvoteCount++;
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

        if (vote.IsUpvote)
            comment.UpvoteCount--;
        else
            comment.DownvoteCount--;

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
                Rank = comment.Author.Rank.ToString()
            },
            ParentCommentId = comment.ParentCommentId,
            UserVote = userVote,
            Replies = comment.Replies?.Where(r => !r.IsDeleted).OrderBy(r => r.CreatedAt).Select(r => MapToDto(r, userId)).ToList() ?? new List<CommentDto>()
        };
    }
}
