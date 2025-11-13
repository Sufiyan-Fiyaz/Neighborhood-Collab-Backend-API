using neighborhood_collab.Models.LoginModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace neighborhood_collab.Models.ComplaintModels
{
    public class Complaint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ComplaintId { get; set; }

        [Required]
        [ForeignKey("User")]
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
        public string Category { get; set; } // e.g. Garbage, Noise, etc.

        [StringLength(100)]
        public string? CustomCategory { get; set; }

        [StringLength(15)]
        public string Priority { get; set; } = "Low"; // Default value

        [StringLength(150)]
        public string? Location { get; set; }

        [StringLength(150)]
        public string? NearestPopularPlace { get; set; }

        [StringLength(20)]
        public string Scope { get; set; } = "Society"; // Default value

        [StringLength(255)]
        public string? TaggedPersons { get; set; }

        // ✅ Replaced PollVotes with Like/Dislike counters
        public int PollLikes { get; set; } = 0;
        public int PollDislikes { get; set; } = 0;

        public string? MediaUrls { get; set; }

        [Column("remarks_count")]
        public int? RemarksCount { get; set; }

        public bool IsAnonymous { get; set; } = false;

        [StringLength(30)]
        public string AssignedDepartment { get; set; } = "Maintenance";

        [StringLength(100)]
        public string? CustomDepartment { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        // 🔗 Navigation Property
        public virtual User? User { get; set; }

        // 🔗 Navigation Property for ComplaintVotes (optional)
        public virtual ICollection<ComplaintVote>? ComplaintVotes { get; set; }
        public virtual ICollection<ComplaintRemark> Remarks { get; set; } = new List<ComplaintRemark>();


    }
}
