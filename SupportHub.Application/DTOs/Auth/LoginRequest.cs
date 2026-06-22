using System.ComponentModel.DataAnnotations;
namespace SupportHub.Application.DTOs.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "EMail is required.")]
        [EmailAddress(ErrorMessage = "Invalid EMail address.")]
        public string EMail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }
}
