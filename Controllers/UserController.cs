using Microsoft.AspNetCore.Mvc;
using FaceAuthApp.Models;
using FaceAuthApp.Services;
using FaceAuthApp.Data;
using Microsoft.EntityFrameworkCore;

namespace FaceAuthApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly FaceService _faceService;
        private readonly IWebHostEnvironment _env;

        public UserController(AppDbContext context, FaceService faceService, IWebHostEnvironment env)
        {
            _context = context;
            _faceService = faceService;
            _env = env;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserRegisterDto dto)
        {
            try
            {
                if (dto.Image == null || dto.Image.Length == 0)
                    return BadRequest("Image is required.");

                // Save image
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }

                // Extract face features
                var encoding = _faceService.ExtractFaceFeatures(filePath);
                if (encoding == null)
                    return BadRequest("No face detected or image invalid.");

                // Save user and metadata
                var user = new User
                {
                    FullName = dto.FullName,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    PasswordHash = dto.Password, // Note: In production, hash this
                    ImagePath = fileName,
                    CreatedAt = DateTime.UtcNow,
                    FaceEncoding = string.Join(",", encoding.Select(e => e.ToString("F6")))
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "User registered successfully.", userId = user.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"UserController - Error while registering the user - {ex.Message}");
            } 
        }
    } 
}
