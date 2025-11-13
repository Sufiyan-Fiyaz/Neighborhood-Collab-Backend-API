using System.ComponentModel.DataAnnotations;

namespace neighborhood_collab.DTOs.VendorDTOs
{
    public class VendorRegistrationDTOs
    {
        [Required]
        public int UserId { get; set; }  // Existing user id

        [Required,MaxLength(150)]
        public string Email { get; set; }

        [Required, MaxLength(150)]
        public string ShopName { get; set; }

        [Required, MaxLength(100)]
        public string License { get; set; }

        [Required, MaxLength(20)]
        public string Contact { get; set; }

        [Required, MaxLength(255)]
        public string Address { get; set; }
    }
}
