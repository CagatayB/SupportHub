using SupportHub.Application.DTOs.Message;

namespace SupportHub.Application.Interfaces
{
    public interface ITicketNotificationService
    {
        Task SendMessageNotificationAsync(int ticketId, MessageDto message);
    }
}
