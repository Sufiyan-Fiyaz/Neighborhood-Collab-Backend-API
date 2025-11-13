using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace neighborhood_collab.DTOs.PostDTOs
{
    // Create Post DTO
    public class CreatePostDto
    {
        [Required]
        public int UserId { get; set; }

        [MaxLength(5000)]
        public string? Content { get; set; }

        [MaxLength(50)]
        public string Audience { get; set; } = "Public";

        // Multiple files allowed (for PostMedia)
        public List<IFormFile>? MediaFiles { get; set; }
    }

    // DTO for updating post
    public class UpdatePostDto
    {
        [Required]
        public int PostId { get; set; }

        [MaxLength(5000)]
        public string? Content { get; set; }

        [MaxLength(50)]
        public string? Audience { get; set; }

        // Replace existing or add new media
        public List<IFormFile>? NewMediaFiles { get; set; }

        public bool RemoveExistingMedia { get; set; } = false;
    }

    // DTO for single media item in a post
    public class PostMediaDto
    {
        public int Id { get; set; }
        public string MediaUrl { get; set; } = string.Empty;
        public string MediaType { get; set; } = "image"; // "image" or "video"
    }

    // User info in post/feed
    public class UserInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ProfileImage { get; set; }
    }

    // DTO for displaying post with all related info
    public class PostDto
    {
        public int PostId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Audience { get; set; } = "Public";
        public DateTime CreatedAt { get; set; }

        public int LikeCount { get; set; } = 0;
        public int CommentCount { get; set; } = 0;
        public int ShareCount { get; set; } = 0;

        public UserInfoDto User { get; set; } = new UserInfoDto();
        public List<PostMediaDto>? Media { get; set; }
    }

    // Response after creating post
    public class PostResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int PostId { get; set; }
        public List<PostMediaDto>? Media { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
