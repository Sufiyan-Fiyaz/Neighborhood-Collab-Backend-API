using Microsoft.EntityFrameworkCore;
using neighborhood_collab.Models.ComplaintModels;
using neighborhood_collab.Models.LoginModels;
using neighborhood_collab.Models.PostsModels;
using neighborhood_collab.Models.VendorModels;

namespace neighborhood_collab.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // =====================
        // DBSETS
        // =====================
        public DbSet<User> Users { get; set; }
        public DbSet<VendorInfo> VendorInfos { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Share> Shares { get; set; }
        public DbSet<PostMedia> PostMedia { get; set; }

        public DbSet<Complaint>Complaints { get; set; }
        public DbSet<ComplaintVote> ComplaintVotes { get; set; }
        public DbSet<ComplaintRemark> ComplaintRemarks { get; set; }


        // =====================
        // MODEL CONFIGURATION
        // =====================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>()
        .ToTable(tb => tb.HasTrigger("trg_UpdateCommentCount"));

            modelBuilder.Entity<Like>()
    .ToTable(tb => tb.HasTrigger("trg_UpdateLikeCount"));

            modelBuilder.Entity<ComplaintVote>()
                .ToTable(tb => tb.HasTrigger("trg_UpdateComplaintVoteCount"));

            modelBuilder.Entity<ComplaintRemark>()
                .ToTable(tb => tb.HasTrigger("trg_UpdateComplaintRemarks"));

            base.OnModelCreating(modelBuilder);

            // =====================
            // USERS TABLE
            // =====================
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Password).IsRequired().HasMaxLength(255);
                entity.Property(u => u.Phone).IsRequired().HasMaxLength(20);
                entity.HasIndex(u => u.Phone).IsUnique();
                entity.Property(u => u.Dob).IsRequired();
                entity.Property(u => u.Gender).IsRequired().HasMaxLength(10);
                entity.Property(u => u.Roles).HasDefaultValue("[\"user\"]");
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // =====================
            // VENDOR INFO
            // =====================
            modelBuilder.Entity<VendorInfo>(entity =>
            {
                entity.ToTable("VendorInfo");
                entity.HasKey(v => v.Id);
                entity.Property(v => v.ShopName).IsRequired().HasMaxLength(150);
                entity.Property(v => v.License).IsRequired().HasMaxLength(100);
                entity.Property(v => v.Contact).IsRequired().HasMaxLength(20);
                entity.Property(v => v.Address).IsRequired().HasMaxLength(255);
                entity.Property(v => v.CreatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(v => v.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =====================
            // POSTS - ✅ FIXED
            // =====================
            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("Posts");
                entity.HasKey(p => p.PostId);
                entity.Property(p => p.Content).HasColumnType("nvarchar(max)");
                entity.Property(p => p.Audience).HasMaxLength(50);
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(p => p.LikeCount).HasDefaultValue(0);
                entity.Property(p => p.CommentCount).HasDefaultValue(0);
                entity.Property(p => p.ShareCount).HasDefaultValue(0);

                entity.HasOne(p => p.User)
                      .WithMany()
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =====================
            // POST MEDIA - ✅ FIXED
            // =====================
            modelBuilder.Entity<PostMedia>(entity =>
            {
                entity.ToTable("PostMedia");
                entity.HasKey(m => m.MediaId);
                entity.Property(m => m.MediaUrl)
                      .IsRequired()
                      .HasColumnType("nvarchar(max)");
                entity.Property(m => m.MediaType)
                      .IsRequired()
                      .HasMaxLength(11);
                entity.Property(m => m.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(m => m.Post)
                      .WithMany(p => p.PostMedia)
                      .HasForeignKey(m => m.PostId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =====================
            // LIKES - ✅ FIXED
            // =====================
            modelBuilder.Entity<Like>(entity =>
            {
                entity.ToTable("Likes");
                entity.HasKey(l => l.LikeId);
                entity.Property(l => l.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.HasIndex(l => new { l.PostId, l.UserId }).IsUnique();

                entity.HasOne(l => l.Post)
                      .WithMany(p => p.Likes)
                      .HasForeignKey(l => l.PostId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(l => l.User)
                      .WithMany()
                      .HasForeignKey(l => l.UserId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // =====================
            // COMMENTS - ✅ FIXED
            // =====================
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comments");
                entity.HasKey(c => c.CommentId);

                entity.Property(c => c.Content)
                      .IsRequired()
                      .HasColumnType("nvarchar(max)");

                entity.Property(c => c.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(c => c.Post)
                      .WithMany(p => p.Comments)
                      .HasForeignKey(c => c.PostId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.User)
                      .WithMany()
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.NoAction);
            });


            // =====================
            // SHARES - ✅ FIXED
            // =====================
            modelBuilder.Entity<Share>(entity =>
            {
                entity.ToTable("Shares");
                entity.HasKey(s => s.ShareId);
                entity.Property(s => s.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.HasIndex(s => new { s.PostId, s.UserId }).IsUnique();

                // ✅ FIXED: Reference navigation properties
                entity.HasOne<Post>()
                      .WithMany(p => p.Shares)
                      .HasForeignKey(s => s.PostId)
                      .OnDelete(DeleteBehavior.NoAction);  

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(s => s.UserId)
                      .OnDelete(DeleteBehavior.NoAction);  
            });

            // =====================
            // COMPLAINTS 🧱
            // =====================
            modelBuilder.Entity<Complaint>(entity =>
            {
                entity.ToTable(tb => tb.HasTrigger("trg_UpdateComplaintVoteCount"));
                entity.ToTable("complaints");

                entity.HasKey(c => c.ComplaintId)
                      .HasName("PK_complaints");

                entity.Property(c => c.ComplaintId)
                      .HasColumnName("complaint_id");

                entity.Property(c => c.UserId)
                      .HasColumnName("user_id")
                      .IsRequired();

                entity.Property(c => c.UserType)
                      .IsRequired()
                      .HasMaxLength(10)
                      .HasColumnName("user_type");

                entity.Property(c => c.Title)
                      .IsRequired()
                      .HasMaxLength(150)
                      .HasColumnName("title");

                entity.Property(c => c.Description)
                      .IsRequired()
                      .HasColumnType("nvarchar(max)")
                      .HasColumnName("description");

                entity.Property(c => c.Category)
                      .IsRequired()
                      .HasMaxLength(20)
                      .HasColumnName("category");

                entity.Property(c => c.CustomCategory)
                      .HasMaxLength(100)
                      .HasColumnName("custom_category");

                entity.Property(c => c.Priority)
                      .HasMaxLength(15)
                      .HasDefaultValue("Low")
                      .HasColumnName("priority");

                entity.Property(c => c.Location)
                      .HasMaxLength(150)
                      .HasColumnName("location");

                entity.Property(c => c.NearestPopularPlace)
                      .HasMaxLength(150)
                      .HasColumnName("nearest_popular_place");

                entity.Property(c => c.Scope)
                      .HasMaxLength(20)
                      .HasDefaultValue("Society")
                      .HasColumnName("scope");

                entity.Property(c => c.TaggedPersons)
                      .HasMaxLength(255)
                      .HasColumnName("tagged_persons");

                entity.Property(c => c.MediaUrls)
                      .HasColumnType("nvarchar(max)")
                      .HasColumnName("media_urls");

                entity.Property(c => c.RemarksCount)
        .HasColumnType("int")
        .HasColumnName("remarks_count");


                entity.Property(c => c.IsAnonymous)
                      .HasDefaultValue(false)
                      .HasColumnName("is_anonymous");

                entity.Property(c => c.AssignedDepartment)
                      .HasMaxLength(30)
                      .HasDefaultValue("Maintenance")
                      .HasColumnName("assigned_department");

                entity.Property(c => c.CustomDepartment)
                      .HasMaxLength(100)
                      .HasColumnName("custom_department");

                entity.Property(c => c.SubmittedAt)
                      .HasDefaultValueSql("GETDATE()")
                      .HasColumnName("submitted_at");

                entity.Property(c => c.PollLikes)
                      .HasColumnName("poll_likes")
                      .HasDefaultValue(0);

                entity.Property(c => c.PollDislikes)
                      .HasColumnName("poll_dislikes")
                      .HasDefaultValue(0);

                entity.HasOne(c => c.User)
                      .WithMany()
                      .HasForeignKey(c => c.UserId)
                      .HasPrincipalKey(u => u.Id)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_complaints_users");
            });


            // =====================
            // COMPLAINT VOTES 🗳️
            // =====================
            modelBuilder.Entity<ComplaintVote>(entity =>
            {
                entity.ToTable("complaintvotes");

                entity.HasKey(v => v.VoteId)
                      .HasName("PK_complaint_votes");

                entity.Property(v => v.VoteId)
                      .HasColumnName("vote_id");

                entity.Property(v => v.ComplaintId)
                      .HasColumnName("complaint_id")
                      .IsRequired();

                entity.Property(v => v.UserId)
                      .HasColumnName("user_id")
                      .IsRequired();

                entity.Property(v => v.IsLike)
       .HasColumnName("is_like")
       .IsRequired();

                entity.HasOne(v => v.Complaint)
                      .WithMany(c => c.ComplaintVotes)
                      .HasForeignKey(v => v.ComplaintId)
                                  .HasConstraintName("FK_votes_complaints");

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(v => v.UserId)
                      .OnDelete(DeleteBehavior.NoAction)
                      .HasConstraintName("FK_votes_users");
                entity.HasIndex(v => new { v.ComplaintId, v.UserId }).IsUnique();
            });

            // =====================
            // COMPLAINT REMARKS 💬
            // =====================
            modelBuilder.Entity<ComplaintRemark>(entity =>
            {
                entity.ToTable("remarks", tb => tb.HasTrigger("trg_UpdateComplaintRemarks"));

                entity.HasKey(r => r.RemarkId)
                      .HasName("PK_complaint_remarks");

                entity.Property(r => r.RemarkId)
                      .HasColumnName("remark_id");

                entity.Property(r => r.ComplaintId)
                      .HasColumnName("complaint_id")
                      .IsRequired();

                entity.Property(r => r.UserId)
                      .HasColumnName("user_id")
                      .IsRequired();

                entity.Property(r => r.Content)
                      .HasColumnName("content")
                      .HasColumnType("nvarchar(max)")
                      .IsRequired();

                entity.Property(r => r.SubmittedAt)
                      .HasColumnName("submitted_at")
                      .HasDefaultValueSql("GETDATE()");

                entity.HasIndex(r => r.ComplaintId)
                      .HasDatabaseName("IX_remarks_complaint_id");

                // ✅ Define Relationships
                entity.HasOne(r => r.Complaint)
                      .WithMany(c => c.Remarks)
                      .HasForeignKey(r => r.ComplaintId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_remarks_complaints");

                entity.HasOne(r => r.User)
                      .WithMany()
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_remarks_users");
            });



        }
    }
}