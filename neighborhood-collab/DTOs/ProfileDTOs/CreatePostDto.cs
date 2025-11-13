using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace neighborhood_collab.DTOs.PostDTOs
{
    public class CreatePostDTO
    {
        [Required(ErrorMessage = "UserId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "UserId must be greater than 0")]
        public int UserId { get; set; }

        [MaxLength(5000, ErrorMessage = "Content cannot exceed 5000 characters")]
        public string? Content { get; set; }

        [MaxLength(50)]
        public string Audience { get; set; } = "Public";

        // ✅ Updated: multiple files instead of single file
        public List<IFormFile>? MediaFiles { get; set; }
    }
}
 