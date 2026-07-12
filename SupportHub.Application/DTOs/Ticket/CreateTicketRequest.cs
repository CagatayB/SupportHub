using System;
using System.Collections.Generic;
using System.Text;
using static SupportHub.Domain.Enums.Enums;

namespace SupportHub.Application.DTOs.Ticket
{
    public class CreateTicketRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TicketPriority Priority { get; set; }
    }
}
