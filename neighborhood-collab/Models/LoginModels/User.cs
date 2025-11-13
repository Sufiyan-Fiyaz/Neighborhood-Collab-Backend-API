using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace neighborhood_collab.Models.LoginModels
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(150)]
        public string Email { get; set; }

        [Required, MaxLength(255)]
        public string Password { get; set; }

        [Required, MaxLength(20)]
        public string Phone { get; set; }

        [Required]
        public DateTime Dob { get; set; }

        [Required, MaxLength(10)]
        public string Gender { get; set; }

        public string Roles { get; set; } = "[\"user\"]";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        //  Map column names exactly as DB
        [Column("profile_image"), MaxLength(255)]
        public string? ProfileImage { get; set; }  //  nullable

        [Column("bio")]
        public string? Bio { get; set; }           //  nullable

    }
}
