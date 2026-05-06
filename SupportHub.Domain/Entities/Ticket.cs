using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public int CreatedByUserId { get; set; } // Müşteri/Kullanıcı
        public int? AssignedToUserId { get; set; } // Destek Personeli

        // Navigation Properties (EF Core Join yapabilmesi için)
        [ForeignKey("CreatedByUserId")]
        public virtual User CreatedByUser { get; set; } = null!;
        [ForeignKey("AssignedToUserId")]
        public virtual User? AssignedToUser { get; set; }

        public ICollection<TicketMessage> Messages { get; set; } = new List<TicketMessage>();
    }
}
