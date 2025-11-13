namespace neighborhood_collab.DTOs.UserDTOs
{
    public class UpdateProfileWithImageDTO
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? Dob { get; set; }       // 🆕 matches UserResponseDTOs
        public string? Gender { get; set; }      // 🆕 matches UserResponseDTOs
        public string? Bio { get; set; }
        public IFormFile? Image { get; set; }    // 🆕 for profile image upload
    }
}
