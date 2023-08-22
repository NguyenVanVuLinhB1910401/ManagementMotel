using Microsoft.AspNetCore.Identity;

namespace ManagementMotel.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExprityTime { get; set; }
        public string? ResetPasswordToken { get; set; }
        public int UserDetailId { get; set; }
        public UserDetail UserDetail { get; set; }
    }
}
