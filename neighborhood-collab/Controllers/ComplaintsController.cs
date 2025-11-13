using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using neighborhood_collab.Data;
using neighborhood_collab.DTOs.ComplaintDTOs;
using neighborhood_collab.Models.ComplaintModels;

namespace neighborhood_collab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ComplaintsController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/complaints
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ComplaintReadDto>>> GetComplaints()
        {
            var complaints = await _context.Complaints
                .Include(c => c.User)
                .Select(c => new ComplaintReadDto
                {
                    ComplaintId = c.ComplaintId,
                    UserId = c.UserId,
                    UserType = c.UserType,
                    Title = c.Title,
                    Description = c.Description,
                    Category = c.Category,
                    CustomCategory = c.CustomCategory,
                    Priority = c.Priority,
                    Location = c.Location,
                    NearestPopularPlace = c.NearestPopularPlace,
                    Scope = c.Scope,
                    TaggedPersons = c.TaggedPersons,
                    PollLikes = c.PollLikes,
                    PollDislikes = c.PollDislikes,
                    MediaUrls = c.MediaUrls,
                    RemarksCount = c.RemarksCount,
                    IsAnonymous = c.IsAnonymous,
                    AssignedDepartment = c.AssignedDepartment,
                    CustomDepartment = c.CustomDepartment,
                    SubmittedAt = c.SubmittedAt
                })
                .ToListAsync();

            return Ok(complaints);
        }

        // ✅ GET: api/complaints/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ComplaintReadDto>> GetComplaint(int id)
        {
            var c = await _context.Complaints
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.ComplaintId == id);

            if (c == null)
                return NotFound(new { message = $"Complaint with ID {id} not found." });

            var dto = new ComplaintReadDto
            {
                ComplaintId = c.ComplaintId,
                UserId = c.UserId,
                UserType = c.UserType,
                Title = c.Title,
                Description = c.Description,
                Category = c.Category,
                CustomCategory = c.CustomCategory,
                Priority = c.Priority,
                Location = c.Location,
                NearestPopularPlace = c.NearestPopularPlace,
                Scope = c.Scope,
                TaggedPersons = c.TaggedPersons,
                PollLikes = c.PollLikes,
                PollDislikes = c.PollDislikes,
                MediaUrls = c.MediaUrls,
                RemarksCount = c.RemarksCount,
                IsAnonymous = c.IsAnonymous,
                AssignedDepartment = c.AssignedDepartment,
                CustomDepartment = c.CustomDepartment,
                SubmittedAt = c.SubmittedAt
            };

            return Ok(dto);
        }

        // ✅ GET: api/complaints/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ComplaintReadDto>>> GetComplaintsByUser(int userId)
        {
            var complaints = await _context.Complaints
                .Include(x => x.User)
                .Where(x => x.UserId == userId)
                .ToListAsync();

            if (!complaints.Any())
                return NotFound(new { message = $"No complaints found for User ID {userId}." });

            var complaintDtos = complaints.Select(c => new ComplaintReadDto
            {
                ComplaintId = c.ComplaintId,
                UserId = c.UserId,
                UserType = c.UserType,
                Title = c.Title,
                Description = c.Description,
                Category = c.Category,
                CustomCategory = c.CustomCategory,
                Priority = c.Priority,
                Location = c.Location,
                NearestPopularPlace = c.NearestPopularPlace,
                Scope = c.Scope,
                TaggedPersons = c.TaggedPersons,
                PollLikes = c.PollLikes,
                PollDislikes = c.PollDislikes,
                MediaUrls = c.MediaUrls,
                RemarksCount = c.RemarksCount,
                IsAnonymous = c.IsAnonymous,
                AssignedDepartment = c.AssignedDepartment,
                CustomDepartment = c.CustomDepartment,
                SubmittedAt = c.SubmittedAt
            }).ToList();

            return Ok(complaintDtos);
        }

        // ✅ POST: api/complaints
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ComplaintReadDto>> CreateComplaint([FromForm] ComplaintCreateDto dto)
        {
            var complaint = new Complaint
            {
                UserId = dto.UserId,
                UserType = dto.UserType,
                Title = dto.Title,
                Description = dto.Description,
                Category = dto.Category,
                CustomCategory = dto.CustomCategory,
                Priority = dto.Priority ?? "Low",
                Location = dto.Location,
                NearestPopularPlace = dto.NearestPopularPlace,
                Scope = dto.Scope ?? "Society",
                TaggedPersons = dto.TaggedPersons,
                RemarksCount = dto.RemarksCount,
                IsAnonymous = dto.IsAnonymous,
                AssignedDepartment = dto.AssignedDepartment ?? "Maintenance",
                CustomDepartment = dto.CustomDepartment,
                SubmittedAt = DateTime.Now
            };

            // ✅ Handle uploaded files
            if (dto.MediaFiles != null && dto.MediaFiles.Any())
            {
                var fileUrls = new List<string>();
                var uploadsFolder = Path.Combine("wwwroot", "uploads", "complaints");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                foreach (var file in dto.MediaFiles)
                {
                    var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    fileUrls.Add($"/uploads/complaints/{fileName}");
                }

                complaint.MediaUrls = string.Join(',', fileUrls);
            }

            _context.Complaints.Add(complaint);
            await _context.SaveChangesAsync();

            var readDto = new ComplaintReadDto
            {
                ComplaintId = complaint.ComplaintId,
                UserId = complaint.UserId,
                UserType = complaint.UserType,
                Title = complaint.Title,
                Description = complaint.Description,
                Category = complaint.Category,
                CustomCategory = complaint.CustomCategory,
                Priority = complaint.Priority,
                Location = complaint.Location,
                NearestPopularPlace = complaint.NearestPopularPlace,
                Scope = complaint.Scope,
                TaggedPersons = complaint.TaggedPersons,
                PollLikes = complaint.PollLikes,
                PollDislikes = complaint.PollDislikes,
                MediaUrls = complaint.MediaUrls,
                RemarksCount = complaint.RemarksCount,
                IsAnonymous = complaint.IsAnonymous,
                AssignedDepartment = complaint.AssignedDepartment,
                CustomDepartment = complaint.CustomDepartment,
                SubmittedAt = complaint.SubmittedAt
            };

            return CreatedAtAction(nameof(GetComplaint), new { id = complaint.ComplaintId }, readDto);
        }

        // ✅ PUT: api/complaints/{id}
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateComplaint(int id, [FromForm] ComplaintUpdateDto dto)
        {
            var complaint = await _context.Complaints.FindAsync(id);
            if (complaint == null)
                return NotFound(new { message = $"Complaint with ID {id} not found." });

            complaint.UserType = dto.UserType ?? complaint.UserType;
            complaint.Title = dto.Title ?? complaint.Title;
            complaint.Description = dto.Description ?? complaint.Description;
            complaint.Category = dto.Category ?? complaint.Category;
            complaint.CustomCategory = dto.CustomCategory ?? complaint.CustomCategory;
            complaint.Priority = dto.Priority ?? complaint.Priority;
            complaint.Location = dto.Location ?? complaint.Location;
            complaint.NearestPopularPlace = dto.NearestPopularPlace ?? complaint.NearestPopularPlace;
            complaint.Scope = dto.Scope ?? complaint.Scope;
            complaint.TaggedPersons = dto.TaggedPersons ?? complaint.TaggedPersons;
            complaint.RemarksCount = dto.RemarksCount ?? complaint.RemarksCount;
            complaint.IsAnonymous = dto.IsAnonymous ?? complaint.IsAnonymous;
            complaint.AssignedDepartment = dto.AssignedDepartment ?? complaint.AssignedDepartment;
            complaint.CustomDepartment = dto.CustomDepartment ?? complaint.CustomDepartment;

            // ✅ Handle media uploads
            if (dto.MediaFiles != null && dto.MediaFiles.Any())
            {
                var fileUrls = new List<string>();
                var uploadsFolder = Path.Combine("wwwroot", "uploads", "complaints");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                foreach (var file in dto.MediaFiles)
                {
                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    var uniqueName = $"{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(uploadsFolder, uniqueName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    fileUrls.Add($"/uploads/complaints/{uniqueName}");
                }

                complaint.MediaUrls = string.Join(',', fileUrls);
            }

            _context.Entry(complaint).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ✅ DELETE: api/complaints/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComplaint(int id)
        {
            var complaint = await _context.Complaints.FindAsync(id);
            if (complaint == null)
                return NotFound(new { message = $"Complaint with ID {id} not found." });

            _context.Complaints.Remove(complaint);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
