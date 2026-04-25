using SupportHub.Application.DTOs.Message;

namespace SupportHub.Application.Interfaces
{
    public interface IMessageService
    {

        Task<List<MessageDto>> GetMessagesByTicketIdAsync(int ticketId);
        Task<MessageDto> SendMessageAsync(int ticketId, SendMessageRequest request, string userId);
    }
}
