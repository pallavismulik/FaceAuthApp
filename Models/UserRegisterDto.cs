using Microsoft.AspNetCore.Http;

namespace FaceAuthApp.Models
{
    public class UserRegisterDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public IFormFile Image { get; set; } 
    }
}
