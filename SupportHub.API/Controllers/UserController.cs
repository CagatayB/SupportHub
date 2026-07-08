using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportHub.Application.DTOs.Auth;
using SupportHub.Infrastructure.Persisteance;
using System.Security.Claims;
using static SupportHub.Domain.Enums.Enums;

namespace SupportHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class UserController : ControllerBase
    {
        private readonly SupportHubDbContext _context;

        public UserController(SupportHubDbContext context)
        {
            _context = context;
        }
        
        
        [HttpPatch("update-role")]
        [Authorize(Roles = "Admin,Manager")] // admin and manager can update roles.
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateRoleDto updateRoleDto)
        {
            var currentUserRoleStr = User.FindFirst(ClaimTypes.Role)?.Value;
            if (!Enum.TryParse<UserRole>(currentUserRoleStr, out var currentUserRole))
                return Unauthorized("Role information could not be obtained.");

            if (currentUserRole == UserRole.Manager && updateRoleDto.NewRole == UserRole.Admin)
            {
                return BadRequest("Manager role users cannot assign Admin role to others.");
            }

            var user = await _context.Users.FindAsync(updateRoleDto.UserId);
            if (user == null)
            {
                return NotFound();
            }

            user.Role = updateRoleDto.NewRole;
            user.UpdatedAt = DateTime.UtcNow; // Update the UpdatedAt timestamp

            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
