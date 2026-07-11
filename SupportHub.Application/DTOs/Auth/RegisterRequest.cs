using System.ComponentModel.DataAnnotations;
using static SupportHub.Domain.Enums.Enums;

namespace SupportHub.Application.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Kullanıcı rolü zorunludur.")]
        public UserRole Role { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        [StringLength(128, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3-128 karakter arasında olmalıdır.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string Password { get; set; } = string.Empty;

        [Compare(nameof(Password), ErrorMessage = "Şifreler birbiriyle eşleşmiyor.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
