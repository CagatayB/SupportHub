using SupportHub.Application.DTOs.Message;
using SupportHub.Application.DTOs.Ticket;

namespace SupportHub.Application.Interfaces
{
    public interface ITicketNotificationService
    {
        Task SendMessageNotificationAsync(int ticketId, MessageDto message);
        Task NotifyTicketCreated(TicketDto ticketDto);
        Task NotifyTicketUpdated(TicketDto ticketDto);
    }
}
