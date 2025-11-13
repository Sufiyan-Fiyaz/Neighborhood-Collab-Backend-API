using System.ComponentModel.DataAnnotations;

namespace neighborhood_collab.DTOs.UserDTOs
{
    public class UserLoginDTO
    {
        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }
    }
}
