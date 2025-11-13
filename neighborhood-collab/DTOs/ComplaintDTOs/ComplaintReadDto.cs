using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace neighborhood_collab.DTOs.ComplaintDTOs
{
    public class ComplaintReadDto
    {
        public int ComplaintId { get; set; }

        public int UserId { get; set; }

        public string UserType { get; set; }  // 'Resident' or 'Visitor'

        public string Title { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }  // 'Garbage', 'Noise', etc.

        public string? CustomCategory { get; set; }

        public string Priority { get; set; }

        public string? Location { get; set; }

        public string? NearestPopularPlace { get; set; }

        public string Scope { get; set; }

        public string? TaggedPersons { get; set; }

        // ✅ Replaced single PollVotes with Like/Dislike breakdown
        public int PollLikes { get; set; } = 0;
        public int PollDislikes { get; set; } = 0;

        public string? MediaUrls { get; set; }

        [Column("remarks_count")]
        public int? RemarksCount { get; set; }

        public bool IsAnonymous { get; set; }

        public string AssignedDepartment { get; set; }

        public string? CustomDepartment { get; set; }

        public DateTime SubmittedAt { get; set; }
    }
}
