using ManagementMotel.DTOs;
using ManagementMotel.Helpers;
using ManagementMotel.Interfaces;
using ManagementMotel.Models;
using ManagementMotel.UtilityServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ManagementMotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthenticateController(IUnitOfWork unitOfWork, IConfiguration configuration, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            try
            {
                var user = await _unitOfWork.UserManager.FindByNameAsync(model.Username);
                if (user != null && await _unitOfWork.UserManager.CheckPasswordAsync(user, model.Password))
                {
                    var userRoles = await _unitOfWork.UserManager.GetRolesAsync(user);

                    var authClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }
                    var token = CreateToken(authClaims);
                    var refreshToken = GenerateRefreshToken();
                    int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int RefreshTokenValidityInDays);

                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExprityTime = DateTime.Now.AddDays(RefreshTokenValidityInDays);
                    //user.RefreshTokenExprityTime = DateTime.Now.AddMinutes(5);
                    await _unitOfWork.UserManager.UpdateAsync(user);


                    return Ok(new
                    {
                        message = "Login successfully",
                        AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                        RefreshToken = refreshToken,
                        expiration = token.ValidTo
                    });
                }
                return BadRequest(new { Status = "Failure", Message = "Incorrect username or password." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenDto model)
        {
            try
            {
                string accessToken = model.AccessToken;
                string refreshToken = model.RefreshToken;

                var principle = getPriciple(accessToken);
                var newAccessToken = CreateToken(principle.Claims.ToList());
                var newRefreshToken = GenerateRefreshToken();
                var userName = principle?.Identity?.Name;
                var user = await _unitOfWork.UserManager.FindByNameAsync(userName);
                if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExprityTime <= DateTime.Now)
                {
                    return Unauthorized("Invalid refresh token");
                }
                user.RefreshToken = newRefreshToken;
                await _unitOfWork.UserManager.UpdateAsync(user);
                return new ObjectResult(new
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                    RefreshToken = newRefreshToken,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private ClaimsPrincipal getPriciple(string accessToken)
        {
            var tokenPars = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
                ValidateLifetime = false,

            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principle = tokenHandler.ValidateToken(accessToken, tokenPars, out SecurityToken secToken);
            return principle;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private JwtSecurityToken CreateToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int TokenValidityInMinutes);
            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddMinutes(TokenValidityInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }



        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            return await RegisterUser(model, new string[] { UserRoles.Employee });
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDto model)
        {
            return await RegisterUser(model, new string[] { UserRoles.Admin, UserRoles.Employee });
        }

        private async Task<IActionResult> RegisterUser([FromBody] RegisterDto model, string[] roles)
        {
            var userExists = await _unitOfWork.UserManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status409Conflict, new { Status = "Error", Message = "User already exists!" });
            var emailExists = await _unitOfWork.UserManager.FindByEmailAsync(model.Email);
            if (emailExists != null)
                return StatusCode(StatusCodes.Status409Conflict, new { Status = "Error", Message = "Email already exists!" });

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
            };
            var result = await _unitOfWork.UserManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            foreach (var role in roles)
            {
                if (!await _unitOfWork.RoleManager.RoleExistsAsync(role))
                    await _unitOfWork.RoleManager.CreateAsync(new IdentityRole(role));

                if (await _unitOfWork.RoleManager.RoleExistsAsync(role))
                    await _unitOfWork.UserManager.AddToRoleAsync(user, role);
            }

            return Ok(new { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost("send-reset-email/{email}")]
        public async Task<IActionResult> SendMail(string email)
        {
            try
            {
                var user = await _unitOfWork.UserManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return NotFound(new { Status = 404, Massage = "Email doesn't exists." });
                }
                //var tokenBytes = RandomNumberGenerator.GetBytes(64);
                //var emailToken = Convert.ToBase64String(tokenBytes);
                user.ResetPasswordToken = await _unitOfWork.UserManager.GeneratePasswordResetTokenAsync(user);
                string from = _configuration["EmailSettings:From"];
                var emailModel = new EmailModel(email, "Reset Password!!", EmailBody.EmailStringBody(email, user.ResetPasswordToken));
                _emailService.SendEmail(emailModel);
                await _unitOfWork.UserManager.UpdateAsync(user);
                return Ok(new { Status = 200, Message = "Email Sent!" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            //var newToken = model.EmailToken.Replace(" ", "+");
            var user = await _unitOfWork.UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound(new { Status = 404, Message = "User doesn't exist." });
            }
            var tokenCode = user.ResetPasswordToken;

            // Check if the token is still valid
            var isTokenValid = await _unitOfWork.UserManager.VerifyUserTokenAsync(user, _unitOfWork.UserManager.Options.Tokens.PasswordResetTokenProvider, UserManager<IdentityUser>.ResetPasswordTokenPurpose, tokenCode);
            if (!isTokenValid || tokenCode != model.EmailToken)
            {
                // Handle expired or invalid token
                // You may want to redirect to an error page or display a specific error message
                return BadRequest(new { Status = 400, Message = "Invalid reset link." });
            }
            var changePasswordResult = await _unitOfWork.UserManager.ResetPasswordAsync(user, tokenCode, model.NewPassword);
            return Ok(new { Status = 200, Message = "Password Reset Successfully" });

        }
    }
}
