using System.ComponentModel.DataAnnotations;

namespace FaceAuthApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        public string Phone { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string ImagePath { get; set; }

        public string FaceEncoding { get; set; }

        public DateTime CreatedAt { get; set; } 
    }
}
