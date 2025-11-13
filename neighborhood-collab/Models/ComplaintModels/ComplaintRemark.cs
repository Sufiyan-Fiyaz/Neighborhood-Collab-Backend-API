using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using neighborhood_collab.Models.LoginModels;

namespace neighborhood_collab.Models.ComplaintModels
{
    public class ComplaintRemark
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RemarkId { get; set; }

        [Required]
        [ForeignKey("Complaint")]
        public int ComplaintId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Content { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        // ✅ Navigation properties
        public virtual Complaint Complaint { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
