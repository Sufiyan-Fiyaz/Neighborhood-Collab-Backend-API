using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using neighborhood_collab.Data;
using neighborhood_collab.Models.LoginModels;
using System.Text.Json;
using neighborhood_collab.DTOs.UserDTOs;

namespace neighborhood_collab.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/user/signup
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] UserSignupDTOs dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email already exists");

            if (await _context.Users.AnyAsync(u => u.Phone == dto.Phone))
                return BadRequest("Phone already exists");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Phone = dto.Phone,
                Dob = dto.Dob,
                Gender = dto.Gender,
                Roles = JsonSerializer.Serialize(new string[] { "user" }),
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var response = new UserResponseDTOs
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Dob = user.Dob,
                Gender = user.Gender,
                Roles = user.Roles,
                CreatedAt = user.CreatedAt
            };

            return Ok(new { Message = "User registered successfully", User = response });
        }

        // POST: api/user/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return BadRequest("Invalid email or password");

            bool verified = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);
            if (!verified)
                return BadRequest("Invalid email or password");

            var response = new UserResponseDTOs
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Dob = user.Dob,
                Gender = user.Gender,
                Roles = user.Roles,
                CreatedAt = user.CreatedAt
            };

            return Ok(new { Message = "Login successful", User = response });
        }


        // GET: api/user/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            var response = new UserResponseDTOs
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Dob = user.Dob,
                Gender = user.Gender,
                Roles = user.Roles,
                CreatedAt = user.CreatedAt,
                ProfileImage = user.ProfileImage, //  Added
                Bio = user.Bio                     //Added
            };

            return Ok(response);
        }


        // PUT: api/user/update-profile
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTOs dto)
        {
            var user = await _context.Users.FindAsync(dto.Id);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            if (!string.IsNullOrWhiteSpace(dto.Name))
                user.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Bio))
                user.Bio = dto.Bio;

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Profile updated successfully" });
        }


        [HttpPut("update-profile-with-image/{id}")]
        public async Task<IActionResult> UpdateProfileWithImage(int id, [FromForm] UpdateProfileWithImageDTO dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            // 🔹 Update basic fields
            if (!string.IsNullOrWhiteSpace(dto.Name))
                user.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                user.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.Phone))
                user.Phone = dto.Phone;

            if (!string.IsNullOrWhiteSpace(dto.Bio))
                user.Bio = dto.Bio;

            if (!string.IsNullOrWhiteSpace(dto.Gender))
                user.Gender = dto.Gender;

            if (dto.Dob.HasValue)
                user.Dob = dto.Dob.Value;

            // 🔹 Handle image upload
            if (dto.Image != null && dto.Image.Length > 0)
            {
                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads","profile");
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                var fileName = $"{Guid.NewGuid()}_{dto.Image.FileName}";
                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }

                user.ProfileImage = $"/uploads/profile/{fileName}";

            }

            await _context.SaveChangesAsync();

            // Return updated user in DTO format
            var response = new UserResponseDTOs
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Dob = user.Dob,
                Gender = user.Gender,
                Roles = user.Roles,
                CreatedAt = user.CreatedAt,
                ProfileImage = user.ProfileImage,
                Bio = user.Bio
            };

            return Ok(response);
        }




    }
}
