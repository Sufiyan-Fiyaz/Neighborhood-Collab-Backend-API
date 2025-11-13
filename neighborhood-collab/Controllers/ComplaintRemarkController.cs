using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using neighborhood_collab.Data;
using neighborhood_collab.Models.ComplaintModels;
using neighborhood_collab.DTOs.ComplaintDTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace neighborhood_collab.Controllers.Complaints
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintRemarksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ComplaintRemarksController(AppDbContext context)
        {
            _context = context;
        }

        // =====================
        // GET: api/ComplaintRemarks
        // =====================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ComplaintRemarkDTO>>> GetAllRemarks()
        {
            var remarks = await _context.ComplaintRemarks
                .Select(r => new ComplaintRemarkDTO
                {
                    RemarkId = r.RemarkId,
                    ComplaintId = r.ComplaintId,
                    UserId = r.UserId,
                    Content = r.Content,
                    SubmittedAt = r.SubmittedAt
                })
                .ToListAsync();

            return Ok(remarks);
        }

        // =====================
        // GET: api/ComplaintRemarks/{id}
        // =====================
        [HttpGet("{id}")]
        public async Task<ActionResult<ComplaintRemarkDTO>> GetRemarkById(int id)
        {
            var remark = await _context.ComplaintRemarks.FindAsync(id);

            if (remark == null)
                return NotFound();

            var dto = new ComplaintRemarkDTO
            {
                RemarkId = remark.RemarkId,
                ComplaintId = remark.ComplaintId,
                UserId = remark.UserId,
                Content = remark.Content,
                SubmittedAt = remark.SubmittedAt
            };

            return Ok(dto);
        }

        // =====================
        // GET: api/ComplaintRemarks/complaint/{complaintId}
        // =====================
        [HttpGet("complaint/{complaintId}")]
        public async Task<IActionResult> GetRemarksByComplaint(int complaintId)
        {
            var remarks = await _context.ComplaintRemarks
      .Include(r => r.User)
      .Where(r => r.ComplaintId == complaintId)
      .Select(r => new ComplaintRemarkDTO
      {
          RemarkId = r.RemarkId,
          ComplaintId = r.ComplaintId,
          UserId = r.UserId,
          Content = r.Content,
          SubmittedAt = r.SubmittedAt,
          User = new UserInfoDto
          {
              Id = r.User.Id,
              Name = r.User.Name,
              ProfileImage = r.User.ProfileImage
          }
      })
      .ToListAsync();
            return Ok(remarks);
        }


        // =====================
        // POST: api/ComplaintRemarks
        // =====================
        [HttpPost]
        public async Task<ActionResult<ComplaintRemarkDTO>> AddRemark([FromBody] ComplaintRemarkDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate ComplaintId
            var complaint = await _context.Complaints.FindAsync(dto.ComplaintId);
            if (complaint == null)
                return NotFound(new { Message = "Complaint not found." });

            var remark = new ComplaintRemark
            {
                ComplaintId = dto.ComplaintId,

                UserId = dto.UserId,
                Content = dto.Content
            };

            _context.ComplaintRemarks.Add(remark);
            await _context.SaveChangesAsync();

            // 🔹 Update remark count in Complaints table
            complaint.RemarksCount = await _context.ComplaintRemarks
                .CountAsync(r => r.ComplaintId == dto.ComplaintId);

            await _context.SaveChangesAsync();

            dto.RemarkId = remark.RemarkId;
            dto.SubmittedAt = remark.SubmittedAt;

            return CreatedAtAction(nameof(GetRemarkById), new { id = remark.RemarkId }, dto);
        }

        // =====================
        // PUT: api/ComplaintRemarks/{id}
        // =====================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRemark(int id, [FromBody] ComplaintRemarkDTO dto)
        {
            if (id != dto.RemarkId)
                return BadRequest("Remark ID mismatch.");

            var remark = await _context.ComplaintRemarks.FindAsync(id);
            if (remark == null)
                return NotFound();

            remark.Content = dto.Content;
            _context.Entry(remark).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =====================
        // DELETE: api/ComplaintRemarks/{id}
        // =====================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRemark(int id, [FromQuery] int userId)
        {
            var remark = await _context.ComplaintRemarks.FindAsync(id);
            if (remark == null)
                return NotFound();

            // ✅ Restrict deletion to remark owner only
            if (remark.UserId != userId)
                return Forbid("You can only delete your own remarks.");

            int complaintId = remark.ComplaintId;

            _context.ComplaintRemarks.Remove(remark);
            await _context.SaveChangesAsync();

            // ✅ Update remarks count after deletion
            var complaint = await _context.Complaints.FindAsync(complaintId);
            if (complaint != null)
            {
                complaint.RemarksCount = await _context.ComplaintRemarks
                    .CountAsync(r => r.ComplaintId == complaintId);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

    }
}
