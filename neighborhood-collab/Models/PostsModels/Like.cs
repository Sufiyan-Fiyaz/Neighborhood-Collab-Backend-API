using neighborhood_collab.Models.LoginModels;

namespace neighborhood_collab.Models.PostsModels
{
    // Models/Like.cs
    public class Like
    {
        public int LikeId { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Post? Post { get; set; }
        public User? User { get; set; }
    }


}
