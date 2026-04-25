using SupportHub.Application.DTOs.Message;

namespace SupportHub.Application.Interfaces
{
    public interface IMessageService
    {
        Task<MessageDto> SendMessageAsync(int ticketId, string text, string userId);
    }
}
