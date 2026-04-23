using System;
using System.Collections.Generic;
using System.Text;
using static SupportHub.Domain.Enums.Enums;

namespace SupportHub.Domain.Entities
{
    public class Ticket : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TicketStatus Status { get; set; } = TicketStatus.Open;
        public TicketPriority Priority { get; set; } = TicketPriority.Medium;

        // İlişkiler
        public string CreatedByUserId { get; set; } = string.Empty; // Müşteri/Kullanıcı
        public string? AssignedToUserId { get; set; } // Destek Personeli

        public ICollection<TicketMessage> Messages { get; set; } = new List<TicketMessage>();
    }
}
