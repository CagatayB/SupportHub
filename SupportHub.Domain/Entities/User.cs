using System.ComponentModel.DataAnnotations;
using static SupportHub.Domain.Enums.Enums;

namespace SupportHub.Domain.Entities
{
    public class User : BaseEntity
    {
        [Required]
        [StringLength(255, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.SupportStaff;
    }
}
