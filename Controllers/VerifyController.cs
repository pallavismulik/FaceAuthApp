using Microsoft.AspNetCore.Mvc;
using FaceAuthApp.Data;
using FaceAuthApp.Services;
using Microsoft.EntityFrameworkCore;

namespace FaceAuthApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VerifyController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly FaceService _faceService;
        private readonly ImageCompareService _imageCompareService;
        private readonly IWebHostEnvironment _env;

        public VerifyController(AppDbContext context, FaceService faceService, ImageCompareService imageCompareService, IWebHostEnvironment env)
        {
            _context = context;
            _faceService = faceService;
            _imageCompareService = imageCompareService;
            _env = env;
        }

        [HttpPost("face")]
        public async Task<IActionResult> VerifyFace([FromForm] IFormFile image)
        {
            try
            {
                if (image == null || image.Length == 0)
                    return BadRequest("Image is required.");

                var uploadsFolder = Path.Combine(_env.WebRootPath, "temp");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = $"verify_{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                var targetEncoding = _faceService.ExtractFaceFeatures(filePath);
                if (targetEncoding == null)
                    return BadRequest("No face detected in image.");

                var users = await _context.Users.ToListAsync();
                foreach (var user in users)
                {
                    var storedEncoding = _faceService.ParseEncoding(user.FaceEncoding);
                    var similarity = _faceService.CompareFaces(storedEncoding, targetEncoding);

                    if (similarity >= 0.85) // Threshold: adjustable//if (similarity >= 0.85) // Threshold: adjustable
                    {
                        return Ok(new
                        {
                            matched = true,
                            confidence = Math.Round(similarity * 100, 2),
                            user = new { user.Id, user.FullName, user.Email }
                        });
                    }
                    else
                    {
                        var uploadsFolderPath = Path.Combine(_env.WebRootPath, "uploads");
                        string imagePathDB = user.ImagePath;
                        string registeredImage =  Path.Combine(uploadsFolderPath, Path.GetFileName(imagePathDB));

                        var isMatch = _imageCompareService.CompareFacesLBPH(registeredImage, filePath);
                        if (isMatch)
                        {
                            return Ok(new
                            {
                                matched = true,
                                confidence = Math.Round(similarity * 100, 2),
                                user = new { user.Id, user.FullName, user.Email }
                            });
                        }

                    }
                }

                return Ok(new
                {
                    matched = false,
                    confidence = 0.0,
                    message = "No matching face found. Possible fraud or new user."
                });
            }
            catch (Exception ex) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"VerifyController - Error while verifying the face - {ex.Message}");
            }
            finally
            {
                var tempFolderPath = Path.Combine(_env.WebRootPath, "temp");
                CleanUpTempFolder(tempFolderPath);
            }
        }

        #region "Private methods"

        private void CleanUpTempFolder(string tempFolderPath)
        {
            try
            {
                if (Directory.Exists(tempFolderPath))
                {
                    var files = Directory.GetFiles(tempFolderPath);
                    foreach (var file in files)
                    {
                        System.IO.File.Delete(file);
                    }
                }
            }
            catch (Exception cleanupEx)
            {
                Console.WriteLine($"Failed to clean up temp file.");
            }
        }

        #endregion //Private methods
    }
} 
