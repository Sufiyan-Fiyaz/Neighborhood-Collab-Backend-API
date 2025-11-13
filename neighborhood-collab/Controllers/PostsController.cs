using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using neighborhood_collab.Data;
using neighborhood_collab.DTOs.PostDTOs;
using neighborhood_collab.Models.PostsModels;

namespace neighborhood_collab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        // Proper constructor with dependency injection
        public PostsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostDTO dto)
        {
            try
            {
                // Add debug check
                if (_context == null)
                {
                    return StatusCode(500, new { message = "Database context is not initialized." });
                }

                if (dto == null)
                    return BadRequest(new { message = "Invalid request data." });

                if (dto.UserId <= 0)
                    return BadRequest(new { message = "Invalid UserId." });

                bool hasText = !string.IsNullOrWhiteSpace(dto.Content);
                bool hasFiles = dto.MediaFiles != null && dto.MediaFiles.Any();
                if (!hasText && !hasFiles)
                    return BadRequest(new { message = "Post must contain either text or media." });

                var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
                if (!userExists)
                    return BadRequest(new { message = $"User with ID {dto.UserId} does not exist." });

                var post = new Post
                {
                    UserId = dto.UserId,
                    Content = dto.Content?.Trim(),
                    Audience = string.IsNullOrWhiteSpace(dto.Audience) ? "Public" : dto.Audience,
                    CreatedAt = DateTime.UtcNow,
                    LikeCount = 0,
                    CommentCount = 0,
                    ShareCount = 0
                };

                _context.Posts.Add(post);
                await _context.SaveChangesAsync();

                // Handle Media Files
                if (dto.MediaFiles != null && dto.MediaFiles.Count > 0)
                {
                    var webRoot = _env.WebRootPath;
                    if (string.IsNullOrEmpty(webRoot))
                    {
                        webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    }

                    var uploadsFolder = Path.Combine(webRoot, "uploads", "posts");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".mp4", ".mov", ".avi" };
                    const long maxFileSize = 10 * 1024 * 1024;

                    foreach (var file in dto.MediaFiles)
                    {
                        if (file == null || file.Length == 0)
                            continue;

                        if (string.IsNullOrEmpty(file.FileName))
                            continue;

                        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                        if (!allowedExtensions.Contains(ext))
                            return BadRequest(new { message = $"Invalid file type: {file.FileName}" });

                        if (file.Length > maxFileSize)
                            return BadRequest(new { message = $"File too large: {file.FileName}" });

                        var uniqueName = $"{Guid.NewGuid()}{ext}";
                        var filePath = Path.Combine(uploadsFolder, uniqueName);

                        try
                        {
                            await using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            var mediaType = (ext == ".mp4" || ext == ".mov" || ext == ".avi") ? "video" : "image";
                            var mediaUrl = $"/uploads/posts/{uniqueName}";

                            _context.PostMedia.Add(new PostMedia
                            {
                                PostId = post.PostId,
                                MediaType = mediaType,
                                MediaUrl = mediaUrl,
                                CreatedAt = DateTime.UtcNow
                            });
                        }
                        catch (Exception fileEx)
                        {
                            _context.Posts.Remove(post);
                            await _context.SaveChangesAsync();

                            return StatusCode(500, new
                            {
                                success = false,
                                message = $"Failed to save file {file.FileName}",
                                error = fileEx.Message
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                }

                return Ok(new
                {
                    success = true,
                    message = "Post created successfully",
                    data = new
                    {
                        post.PostId,
                        post.UserId,
                        post.Content,
                        post.Audience,
                        post.CreatedAt
                    }
                });
            }
            catch (DbUpdateException ex)
            {
                var inner = ex.InnerException?.Message ?? "No inner exception details.";
                return StatusCode(500, new
                {
                    success = false,
                    message = "Database update failed.",
                    error = ex.Message,
                    innerException = inner
                });
            }
            catch (IOException ioEx)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "File saving failed.",
                    error = ioEx.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred.",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }
    



        // =======================================================
        // ✅ GET USER POSTS
        // =======================================================
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserPosts(int userId)
        {
            var posts = await _context.Posts
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Include(p => p.PostMedia)
                .Select(p => new
                {
                    p.PostId,
                    p.Content,
                    p.Audience,
                    p.CreatedAt,
                    p.LikeCount,
                    p.CommentCount,
                    p.ShareCount,
                    Media = p.PostMedia.Select(m => new { m.MediaUrl, m.MediaType }).ToList()
                })
                .ToListAsync();

            return Ok(new { success = true, data = posts });
        }

        // =======================================================
        // ✅ GET FEED (Public + Own Posts)
        // =======================================================
        [HttpGet("feed/{userId}")]
        public async Task<IActionResult> GetFeed(int userId)
        {
            var posts = await _context.Posts
                .Where(p => p.Audience == "Public" || p.UserId == userId)
                .Include(p => p.User)
                .Include(p => p.PostMedia)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new
                {
                    p.PostId,
                    p.Content,
                    p.Audience,
                    p.CreatedAt,
                    p.LikeCount,
                    p.CommentCount,
                    p.ShareCount,
                    User = new
                    {
                        p.User.Id,
                        p.User.Name,
                        p.User.ProfileImage
                    },
                    Media = p.PostMedia.Select(m => new { m.MediaUrl, m.MediaType }).ToList()
                })
                .ToListAsync();

            return Ok(new { success = true, data = posts });
        }

        // =======================================================
        // ✅ DELETE POST
        // =======================================================
        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost(int postId)
        {
            var post = await _context.Posts
                .Include(p => p.PostMedia)
                .FirstOrDefaultAsync(p => p.PostId == postId);

            if (post == null)
                return NotFound(new { success = false, message = "Post not found." });

            // Delete media files
            var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            foreach (var media in post.PostMedia)
            {
                var filePath = Path.Combine(webRootPath, media.MediaUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Post and media deleted successfully." });
        }
    }
}

