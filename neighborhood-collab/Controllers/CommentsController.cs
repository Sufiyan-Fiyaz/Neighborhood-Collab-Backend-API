using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using neighborhood_collab.Data;
using neighborhood_collab.DTOs.CommentDTOs;
using neighborhood_collab.Models.PostsModels;

namespace neighborhood_collab.Controllers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommentsController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/Comments/post/{postId}
        [HttpGet("post/{postId}")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsByPostId(int postId)
        {
            var comments = await _context.Comments
                .Include(c => c.User)
                .Where(c => c.PostId == postId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentDto
                {
                    Id = c.CommentId,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    PostId = c.PostId,
                    User = new UserInfoDto
                    {
                        Id = c.User.Id,
                        Name = c.User.Name,
                        ProfileImage = c.User.ProfileImage
                    }
                })
                .ToListAsync();


            return Ok(comments);
        }

        // ✅ POST: api/Comments
        [HttpPost]
        public async Task<ActionResult<CommentDto>> CreateComment([FromBody] CreateCommentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users.FindAsync(dto.UserId);
            var post = await _context.Posts.FindAsync(dto.PostId);

            if (user == null || post == null)
                return NotFound("User or Post not found.");

            var comment = new Comment
            {
                PostId = dto.PostId,
                UserId = dto.UserId,
                Content = dto.Content
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            var result = new CommentDto
            {
                Id = comment.CommentId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                PostId = comment.PostId,
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    ProfileImage = user.ProfileImage
                }
            };

            return Ok(result);
        }

        // ✅ DELETE: api/Comments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
                return NotFound();

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
