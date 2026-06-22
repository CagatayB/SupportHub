namespace SupportHub.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "SupportStaff"; // Admin = user and role management.
                                                           // Supervisor = can manage tickets and assign them to support staff.
                                                           // SupportStaff = can manage tickets assigned to them.

    }
}
