using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace neighborhood_collab.Models.ComplaintModels
{
    public class ComplaintVote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VoteId { get; set; }

        [Required]
        [ForeignKey("Complaint")]
        public int ComplaintId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        public bool IsLike { get; set; } // true = like, false = dislike

        // Navigation
        public virtual Complaint? Complaint { get; set; }
    }
}
