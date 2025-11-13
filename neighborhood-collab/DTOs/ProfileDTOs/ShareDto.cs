using System.ComponentModel.DataAnnotations;

namespace neighborhood_collab.DTOs.ShareDTOs
{
    public class CreateShareDto
    {
        [Required]
        public int PostId { get; set; }

        [Required]
        public int UserId { get; set; }

        public string? Caption { get; set; }
    }

    public class ShareDto
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string? Caption { get; set; }
        public DateTime SharedAt { get; set; }
        public UserInfoDto User { get; set; } = new UserInfoDto();
    }

    public class UserInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ProfileImage { get; set; }
    }
}
