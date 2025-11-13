using System.ComponentModel.DataAnnotations;

namespace neighborhood_collab.DTOs.UserDTOs
{
    public class UserSignupDTOs
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required, MaxLength(20)]
        public string Phone { get; set; }

        [Required]
        public DateTime Dob { get; set; }

        [Required, MaxLength(10)]
        public string Gender { get; set; }
    }
}
