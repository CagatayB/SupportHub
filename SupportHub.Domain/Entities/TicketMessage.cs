namespace SupportHub.Domain.Entities
{
    public class TicketMessage : BaseEntity
    {
        public string MessageText { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; } = null!;
    }
}
