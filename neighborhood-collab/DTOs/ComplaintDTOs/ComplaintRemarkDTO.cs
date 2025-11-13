namespace neighborhood_collab.DTOs.ComplaintDTOs
{
    public class ComplaintRemarkDTO
    {
        public int RemarkId { get; set; }
        public int ComplaintId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }

        // ✅ Add user info here
        public UserInfoDto User { get; set; } = new UserInfoDto();
    }

    public class UserInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ProfileImage { get; set; }
    }
}
