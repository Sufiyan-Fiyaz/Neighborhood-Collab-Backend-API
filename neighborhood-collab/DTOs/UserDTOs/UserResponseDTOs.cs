namespace neighborhood_collab.DTOs.UserDTOs
{
    public class UserResponseDTOs
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime Dob { get; set; }
        public string Gender { get; set; }
        public string Roles { get; set; }
        public DateTime CreatedAt { get; set; }

        //  Add these fields
        public string? ProfileImage { get; set; } 
        public string? Bio { get; set; }
    }
}
