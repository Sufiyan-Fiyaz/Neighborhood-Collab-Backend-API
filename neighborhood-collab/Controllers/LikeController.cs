using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using neighborhood_collab.Data;
using neighborhood_collab.Models.PostsModels;

namespace neighborhood_collab.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LikesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LikesController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ POST: api/Likes/toggle
        [HttpPost("toggle")]
        public async Task<IActionResult> ToggleLike([FromQuery] int postId, [FromQuery] int userId)
        {
            if (postId <= 0 || userId <= 0)
                return BadRequest(new { message = "Invalid postId or userId." });

            var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == postId);
            if (post == null)
                return NotFound(new { message = "Post not found." });

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

            if (existingLike != null)
            {
                // 🟠 Unlike
                _context.Likes.Remove(existingLike);
                post.LikeCount = Math.Max(0, post.LikeCount - 1); // Prevent negative count
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    liked = false,
                    likeCount = post.LikeCount
                });
            }
            else
            {
                // 🔵 Like
                var like = new Like
                {
                    PostId = postId,
                    UserId = userId
                };

                _context.Likes.Add(like);
                post.LikeCount += 1;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    liked = true,
                    likeCount = post.LikeCount
                });
            }
        }

        // ✅ GET: api/Likes/{postId}
        [HttpGet("{postId}")]
        public async Task<IActionResult> GetLikes(int postId)
        {
            if (postId <= 0)
                return BadRequest(new { message = "Invalid postId." });

            var likeCount = await _context.Likes.CountAsync(l => l.PostId == postId);
            return Ok(new { postId, likeCount });
        }

        // ✅ NEW: Check if a user has liked a post
        // e.g., GET /api/Likes/isLiked?postId=10&userId=5
        [HttpGet("isLiked")]
        public async Task<IActionResult> IsLiked([FromQuery] int postId, [FromQuery] int userId)
        {
            if (postId <= 0 || userId <= 0)
                return BadRequest(new { message = "Invalid postId or userId." });

            var isLiked = await _context.Likes
                .AnyAsync(l => l.PostId == postId && l.UserId == userId);

            return Ok(new { isLiked });
        }
    }
}


