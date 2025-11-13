using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace neighborhood_collab.Models.PostsModels
{
    public class PostMedia
    {
        [Key]
        public int MediaId { get; set; }

        // ✅ Fixed: Proper foreign key relationship
        public int PostId { get; set; }

        [Required]
        [MaxLength(500)]
        public string MediaUrl { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string MediaType { get; set; } = string.Empty; // 'image' or 'video'

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // 🔗 Navigation property
        [ForeignKey(nameof(PostId))]
        public Post? Post { get; set; }
    }
}
