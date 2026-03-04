using System.ComponentModel.DataAnnotations;

namespace ProcurementSystem.API.DTOs
{
    public class RegisterDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        public string FullName { get; set; }

        public string Role { get; set; } = "User";
    }
}
