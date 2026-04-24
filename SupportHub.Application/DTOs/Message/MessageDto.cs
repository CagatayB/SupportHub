using System;
using System.Collections.Generic;
using System.Text;

namespace SupportHub.Application.DTOs.Message
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string MessageText { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
