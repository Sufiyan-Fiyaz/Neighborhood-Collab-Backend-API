using System.ComponentModel.DataAnnotations;

namespace neighborhood_collab.DTOs.CommentDTOs
{
    public class CreateCommentDto
    {
        [Required]
        public int PostId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty;
    }

    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int PostId { get; set; }
        public UserInfoDto User { get; set; } = new UserInfoDto();
    }

    public class UserInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ProfileImage { get; set; }
    }
}
