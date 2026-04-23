using System;
using System.Collections.Generic;
using System.Text;

namespace SupportHub.Application.DTOs.Ticket
{
    public class CreateTicketRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Priority { get; set; }
    }
}
