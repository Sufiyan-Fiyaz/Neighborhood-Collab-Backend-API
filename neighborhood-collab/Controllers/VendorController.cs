using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using neighborhood_collab.Data;
using neighborhood_collab.Models.VendorModels;
using System.Text.Json;
using neighborhood_collab.DTOs.VendorDTOs;

namespace MyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VendorController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/vendor/register
        [HttpPost("register")]
        public async Task<IActionResult> RegisterVendor([FromBody] VendorRegistrationDTOs dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return BadRequest("User must be registered first");

            // Add "vendor" role if not exists
            var roles = JsonSerializer.Deserialize<List<string>>(user.Roles) ?? new List<string>();
            if (!roles.Contains("vendor"))
                roles.Add("vendor");

            user.Roles = JsonSerializer.Serialize(roles);
            _context.Users.Update(user);

            // Add vendor info
            var vendor = new VendorInfo
            {
                UserId = user.Id,
                ShopName = dto.ShopName,
                License = dto.License,
                Contact = dto.Contact,
                Address = dto.Address
            };

            _context.VendorInfos.Add(vendor);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Vendor registered successfully", VendorId = vendor.Id });
        }
    }
}

