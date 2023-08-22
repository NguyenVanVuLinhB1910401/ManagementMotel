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
        [MaxLength(50)]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "CCCD is required")]
        [MaxLength(12)]
        public string? CCCD { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "MobilePhone is required")]
        [MaxLength(12)]
        public string? MobilePhone { get; set; }

        [Required(ErrorMessage = "ImagePreviousCCCD is required")]
        public IFormFile ImagePreviousCCCD { get; set; }

        [Required(ErrorMessage = "ImageAfterCCCD is required")]
        public IFormFile ImageAfterCCCD { get; set; }
    }
}
