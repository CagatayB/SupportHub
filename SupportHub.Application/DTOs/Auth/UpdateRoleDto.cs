using System;
using System.Collections.Generic;
using System.Text;
using static SupportHub.Domain.Enums.Enums;

namespace SupportHub.Application.DTOs.Auth
{
    public class UpdateRoleDto
    {
        public int UserId { get; set; }
        public UserRole NewRole { get; set; }
    }
}
