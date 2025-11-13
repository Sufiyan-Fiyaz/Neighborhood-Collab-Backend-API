using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace neighborhood_collab.DTOs.ComplaintDTOs
{
    public class ComplaintCreateDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(10)]
        public string UserType { get; set; }  // 'Resident' or 'Visitor'

        [Required]
        [StringLength(150)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [StringLength(20)]
        public string Category { get; set; }  // 'Garbage', 'Noise', etc.

        [StringLength(100)]
        public string? CustomCategory { get; set; }

        [StringLength(15)]
        public string? Priority { get; set; } = "Low";  // 'Low', 'Medium', 'High', etc.

        [StringLength(150)]
        public string? Location { get; set; }

        [StringLength(150)]
        public string? NearestPopularPlace { get; set; }

        [StringLength(20)]
        public string? Scope { get; set; } = "Society";  // 'Street', 'Block', etc.

        [StringLength(255)]
        public string? TaggedPersons { get; set; }

        public List<IFormFile>? MediaFiles { get; set; }

        [Column("remarks_count")]
        public int? RemarksCount { get; set; }

        public bool IsAnonymous { get; set; } = false;

        [StringLength(30)]
        public string? AssignedDepartment { get; set; } = "Maintenance";

        [StringLength(100)]
        public string? CustomDepartment { get; set; }

        // ✅ Added new fields for tracking likes/dislikes (optional in creation)
        public int PollLikes { get; set; } = 0;
        public int PollDislikes { get; set; } = 0;
    }
}
