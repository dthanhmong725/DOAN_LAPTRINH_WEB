using Microsoft.EntityFrameworkCore;
using DOAN_LAPTRINHWEB.Models.Entities;

namespace DOAN_LAPTRINHWEB.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<PostVote> PostVotes => Set<PostVote>();
    public DbSet<CommentVote> CommentVotes => Set<CommentVote>();
    public DbSet<PostTag> PostTags => Set<PostTag>();
    public DbSet<PostAttachment> PostAttachments => Set<PostAttachment>();
    public DbSet<Bookmark> Bookmarks => Set<Bookmark>();
    public DbSet<Badge> Badges => Set<Badge>();
    public DbSet<UserBadge> UserBadges => Set<UserBadge>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<RateLimitRecord> RateLimitRecords => Set<RateLimitRecord>();
    public DbSet<SecurityLog> SecurityLogs => Set<SecurityLog>();
    public DbSet<ChatRoom> ChatRooms => Set<ChatRoom>();
    public DbSet<ChatRoomMember> ChatRoomMembers => Set<ChatRoomMember>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<UserUpload> UserUploads => Set<UserUpload>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User indexes
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // SecurityLog index
        modelBuilder.Entity<SecurityLog>()
            .HasIndex(s => s.IpAddress);

        modelBuilder.Entity<SecurityLog>()
            .HasIndex(s => s.Action);

        modelBuilder.Entity<SecurityLog>()
            .HasIndex(s => s.CreatedAt);

        // RefreshToken -> User
        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // SecurityLog -> User (optional)
        modelBuilder.Entity<SecurityLog>()
            .HasOne(s => s.User)
            .WithMany(u => u.SecurityLogs)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Category -> User (CreatedBy)
        modelBuilder.Entity<Category>()
            .HasOne(c => c.CreatedBy)
            .WithMany()
            .HasForeignKey(c => c.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        // Post -> User (Author)
        modelBuilder.Entity<Post>()
            .HasOne(p => p.Author)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Post -> Category
        modelBuilder.Entity<Post>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Posts)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Comment -> User (Author)
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Author)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Comment -> Post
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        // Comment self-reference (replies)
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);

        // PostVote composite unique
        modelBuilder.Entity<PostVote>()
            .HasIndex(pv => new { pv.UserId, pv.PostId })
            .IsUnique();

        // CommentVote composite unique
        modelBuilder.Entity<CommentVote>()
            .HasIndex(cv => new { cv.UserId, cv.CommentId })
            .IsUnique();

        // Bookmark composite unique
        modelBuilder.Entity<Bookmark>()
            .HasIndex(b => new { b.UserId, b.PostId })
            .IsUnique();

        // PostTag -> Post
        modelBuilder.Entity<PostTag>()
            .HasOne(pt => pt.Post)
            .WithMany(p => p.PostTags)
            .HasForeignKey(pt => pt.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        // PostAttachment -> Post & User
        modelBuilder.Entity<PostAttachment>()
            .HasOne(pa => pa.Post)
            .WithMany(p => p.Attachments)
            .HasForeignKey(pa => pa.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PostAttachment>()
            .HasOne(pa => pa.UploadedBy)
            .WithMany()
            .HasForeignKey(pa => pa.UploadedById)
            .OnDelete(DeleteBehavior.Restrict);

        // UserBadge -> User & Badge
        modelBuilder.Entity<UserBadge>()
            .HasOne(ub => ub.User)
            .WithMany(u => u.Badges)
            .HasForeignKey(ub => ub.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserBadge>()
            .HasOne(ub => ub.Badge)
            .WithMany()
            .HasForeignKey(ub => ub.BadgeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserBadge>()
            .HasIndex(ub => new { ub.UserId, ub.BadgeId })
            .IsUnique();

        // ActivityLog -> User
        modelBuilder.Entity<ActivityLog>()
            .HasOne(al => al.User)
            .WithMany(u => u.ActivityLogs)
            .HasForeignKey(al => al.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // RateLimitRecord indexes
        modelBuilder.Entity<RateLimitRecord>()
            .HasIndex(rlr => new { rlr.UserId, rlr.Endpoint });

        // ChatRoom -> User (CreatedBy)
        modelBuilder.Entity<ChatRoom>()
            .HasOne(cr => cr.CreatedBy)
            .WithMany()
            .HasForeignKey(cr => cr.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        // ChatRoomMember -> User & ChatRoom
        modelBuilder.Entity<ChatRoomMember>()
            .HasOne(crm => crm.User)
            .WithMany(u => u.ChatRooms)
            .HasForeignKey(crm => crm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ChatRoomMember>()
            .HasOne(crm => crm.ChatRoom)
            .WithMany(cr => cr.Members)
            .HasForeignKey(crm => crm.ChatRoomId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ChatRoomMember>()
            .HasIndex(crm => new { crm.UserId, crm.ChatRoomId })
            .IsUnique();

        // ChatMessage -> User (Sender) & ChatRoom
        modelBuilder.Entity<ChatMessage>()
            .HasOne(cm => cm.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(cm => cm.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ChatMessage>()
            .HasOne(cm => cm.ChatRoom)
            .WithMany(cr => cr.Messages)
            .HasForeignKey(cm => cm.ChatRoomId)
            .OnDelete(DeleteBehavior.Cascade);

        // ChatMessage self-reference (reply)
        modelBuilder.Entity<ChatMessage>()
            .HasOne(cm => cm.ReplyTo)
            .WithMany()
            .HasForeignKey(cm => cm.ReplyToId)
            .OnDelete(DeleteBehavior.Restrict);

        // UserUpload -> User
        modelBuilder.Entity<UserUpload>()
            .HasOne(u => u.User)
            .WithMany()
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Notification -> Recipient (User)
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Recipient)
            .WithMany()
            .HasForeignKey(n => n.RecipientId)
            .OnDelete(DeleteBehavior.Cascade);

        // Notification -> Actor (User, optional)
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Actor)
            .WithMany()
            .HasForeignKey(n => n.ActorId)
            .OnDelete(DeleteBehavior.NoAction);

        // Notification -> Post (optional)
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Post)
            .WithMany()
            .HasForeignKey(n => n.PostId)
            .OnDelete(DeleteBehavior.NoAction);

        // Notification -> Comment (optional)
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Comment)
            .WithMany()
            .HasForeignKey(n => n.CommentId)
            .OnDelete(DeleteBehavior.NoAction);

        // Index for fast unread count queries
        modelBuilder.Entity<Notification>()
            .HasIndex(n => new { n.RecipientId, n.IsRead });

        // UserUpload -> Post (optional)
        modelBuilder.Entity<UserUpload>()
            .HasOne(u => u.Post)
            .WithMany()
            .HasForeignKey(u => u.PostId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<UserUpload>()
            .HasIndex(u => u.UserId);

        // Seed default categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Pentest", Description = "Thảo luận về phương pháp, công cụ và kỹ thuật kiểm thử xâm nhập. Chia sẻ kinh nghiệm từ các bài lab thực tế.", Icon = "ti-bug", Color = "#f85149", Slug = "pentest", DisplayOrder = 1, CreatedById = 1 },
            new Category { Id = 2, Name = "Malware Analysis", Description = "Phân tích malware tĩnh và động, reverse engineering, nghiên cứu virus và các mối đe dọa mới.", Icon = "ti-virus", Color = "#f0883e", Slug = "malware", DisplayOrder = 2, CreatedById = 1 },
            new Category { Id = 3, Name = "Web Security", Description = "OWASP Top 10, XSS, SQLi, CSRF, LFI/RFI và bảo mật ứng dụng web. Các bài lab và writeup CTF.", Icon = "ti-world", Color = "#388bfd", Slug = "web-security", DisplayOrder = 3, CreatedById = 1 },
            new Category { Id = 4, Name = "Network Security", Description = "Tường lửa, IDS/IPS, giám sát mạng, giao thức và phân tích lưu lượng. Công cụ Wireshark, Snort.", Icon = "ti-network", Color = "#2ecc71", Slug = "network", DisplayOrder = 4, CreatedById = 1 },
            new Category { Id = 5, Name = "Cryptography", Description = "Mã hóa, hash, PKI, chữ ký số và các giao thức mật mã. Bài toán và thách thức crypto.", Icon = "ti-lock", Color = "#bc8cff", Slug = "crypto", DisplayOrder = 5, CreatedById = 1 },
            new Category { Id = 6, Name = "SOC / Blue Team", Description = "Trung tâm điều hành bảo mật, phát hiện và ứng phó sự cố, threat hunting và DFIR.", Icon = "ti-shield", Color = "#00e5a0", Slug = "soc", DisplayOrder = 6, CreatedById = 1 },
            new Category { Id = 7, Name = "Reverse Engineering", Description = "Disassembler, decompiler, phân tích binary, unpacking và patching. Công cụ IDA, Ghidra, x64dbg.", Icon = "ti-cpu", Color = "#f39c12", Slug = "reverse-engineering", DisplayOrder = 7, CreatedById = 1 }
        );

        // Seed default badges
        modelBuilder.Entity<Badge>().HasData(
            new Badge { Id = 1, Name = "First Post", Description = "Created your first post", Icon = "file-text", Color = "#6c757d", Type = "milestone", ReputationRequired = 0 },
            new Badge { Id = 2, Name = "Active Member", Description = "Made 10 posts", Icon = "star", Color = "#ffc107", Type = "milestone", ReputationRequired = 10 },
            new Badge { Id = 3, Name = "Contributor", Description = "Made 50 posts", Icon = "award", Color = "#17a2b8", Type = "milestone", ReputationRequired = 50 },
            new Badge { Id = 4, Name = "Veteran", Description = "Made 100 posts", Icon = "shield", Color = "#28a745", Type = "milestone", ReputationRequired = 100 },
            new Badge { Id = 5, Name = "Helpful", Description = "Received 10 upvotes", Icon = "thumbs-up", Color = "#fd7e14", Type = "reputation", ReputationRequired = 100 },
            new Badge { Id = 6, Name = "Popular", Description = "Received 50 upvotes", Icon = "heart", Color = "#dc3545", Type = "reputation", ReputationRequired = 500 },
            new Badge { Id = 7, Name = "Legendary", Description = "Received 100 upvotes", Icon = "crown", Color = "#6f42c1", Type = "reputation", ReputationRequired = 1000 },
            new Badge { Id = 8, Name = "Bookworm", Description = "Bookmarked 10 posts", Icon = "bookmark", Color = "#20c997", Type = "special", ReputationRequired = 0 },
            new Badge { Id = 9, Name = "Night Owl", Description = "Posted between midnight and 5 AM", Icon = "moon", Color = "#0dcaf0", Type = "special", ReputationRequired = 0 },
            new Badge { Id = 10, Name = "Century Club", Description = "Reached 100 reputation points", Icon = "trophy", Color = "#ffc107", Type = "rank", ReputationRequired = 100 }
        );

        // Seed admin user
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@cyberforum.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                DisplayName = "Administrator",
                Bio = "System Administrator",
                Role = UserRole.Admin,
                Rank = UserRank.Elite,
                ReputationPoints = 9999,
                IsActive = true,
                IsEmailVerified = true
            }
        );
    }
}
