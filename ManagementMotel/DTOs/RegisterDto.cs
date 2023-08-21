using System.ComponentModel.DataAnnotations;

namespace ManagementMotel.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "FistName is required")]
        [MaxLength(50)]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        [MaxLength(20)]
        public string? LastName { get; set; }

        public string? CCCD { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Address { get; set; }

        public int Gender { get; set; }

        [MaxLength(12)]
        public string? MobilePhone { get; set; }

        public string? Location { get; set; }
    }
}
