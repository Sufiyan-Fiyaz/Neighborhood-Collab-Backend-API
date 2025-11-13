
using neighborhood_collab.Models.LoginModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace neighborhood_collab.Models.PostsModels
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        // ✅ Fixed: Proper foreign key relationship
        public int UserId { get; set; }

        [MaxLength(5000)]
        public string? Content { get; set; }

        [MaxLength(50)]
        public string Audience { get; set; } = "Public";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public int LikeCount { get; set; } = 0;
        public int CommentCount { get; set; } = 0;
        public int ShareCount { get; set; } = 0;

        // 🔗 Navigation properties
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        public ICollection<PostMedia> PostMedia { get; set; } = new List<PostMedia>();
        public ICollection<Like>? Likes { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Share>? Shares { get; set; }
    }
}
