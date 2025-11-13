using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using neighborhood_collab.Data;
using neighborhood_collab.Models.ComplaintModels;

namespace neighborhood_collab.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComplaintReactionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ComplaintReactionsController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ POST: api/ComplaintReactions/toggle?complaintId=14&userId=14&reactionType=like
        [HttpPost("toggle")]
        public async Task<IActionResult> ToggleReaction(
            [FromQuery] int complaintId,
            [FromQuery] int userId,
            [FromQuery] string reactionType)
        {
            if (complaintId <= 0 || userId <= 0 || string.IsNullOrEmpty(reactionType))
                return BadRequest(new { message = "Invalid complaintId, userId, or reactionType." });

            if (reactionType != "like" && reactionType != "dislike")
                return BadRequest(new { message = "reactionType must be either 'like' or 'dislike'." });

            var complaint = await _context.Complaints.FirstOrDefaultAsync(c => c.ComplaintId == complaintId);
            if (complaint == null)
                return NotFound(new { message = "Complaint not found." });

            var existingReaction = await _context.ComplaintVotes
                .FirstOrDefaultAsync(r => r.ComplaintId == complaintId && r.UserId == userId);

            bool isLike = reactionType == "like"; // ✅ convert string to bool

            if (existingReaction != null)
            {
                if (existingReaction.IsLike == isLike)
                {
                    // 🟡 Same reaction → Remove it (toggle off)
                    _context.ComplaintVotes.Remove(existingReaction);

                    if (isLike)
                        complaint.PollLikes = Math.Max(0, complaint.PollLikes - 1);
                    else
                        complaint.PollDislikes = Math.Max(0, complaint.PollDislikes - 1);
                }
                else
                {
                    // 🔄 Opposite reaction → Switch
                    if (isLike)
                    {
                        complaint.PollLikes += 1;
                        complaint.PollDislikes = Math.Max(0, complaint.PollDislikes - 1);
                    }
                    else
                    {
                        complaint.PollDislikes += 1;
                        complaint.PollLikes = Math.Max(0, complaint.PollLikes - 1);
                    }

                    existingReaction.IsLike = isLike;
                    _context.ComplaintVotes.Update(existingReaction);
                }
            }
            else
            {
                // 🆕 New reaction
                var newReaction = new ComplaintVote
                {
                    ComplaintId = complaintId,
                    UserId = userId,
                    IsLike = isLike // ✅ bool, not string
                };

                _context.ComplaintVotes.Add(newReaction);

                if (isLike)
                    complaint.PollLikes += 1;
                else
                    complaint.PollDislikes += 1;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                complaintId,
                liked = isLike,
                disliked = !isLike,
                likeCount = complaint.PollLikes,
                dislikeCount = complaint.PollDislikes
            });


        }
        // GET: api/ComplaintReactions/status?complaintId=21&userId=8
        [HttpGet("status")]
        public async Task<IActionResult> GetReactionStatus([FromQuery] int complaintId, [FromQuery] int userId)
        {
            var complaint = await _context.Complaints.FirstOrDefaultAsync(c => c.ComplaintId == complaintId);
            if (complaint == null)
                return NotFound(new { message = "Complaint not found." });

            var existingReaction = await _context.ComplaintVotes
                .FirstOrDefaultAsync(r => r.ComplaintId == complaintId && r.UserId == userId);

            return Ok(new
            {
                isLiked = existingReaction?.IsLike == true,
                isDisliked = existingReaction?.IsLike == false,
                likeCount = complaint.PollLikes,
                dislikeCount = complaint.PollDislikes
            });
        }

    }
}
