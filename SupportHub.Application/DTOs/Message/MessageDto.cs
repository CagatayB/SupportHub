namespace SupportHub.Application.DTOs.Message
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string MessageText { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsOwner { get; set; }
    }
}
