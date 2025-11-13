using neighborhood_collab.Models.LoginModels;

namespace neighborhood_collab.Models.PostsModels
{
    // Models/Comment.cs
    public class Comment
    {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Post? Post { get; set; }
        public User? User { get; set; }
    }


}
