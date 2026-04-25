using Microsoft.AspNetCore.SignalR;
using SupportHub.Application.DTOs.Message;
using SupportHub.Application.Interfaces;
using SupportHub.Domain.Entities;

namespace SupportHub.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IApplicationDbContext _context;
        private readonly ITicketNotificationService _notificationService;

        public MessageService(IApplicationDbContext context, ITicketNotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<MessageDto> SendMessageAsync(int ticketId, string text, string userId)
        {
            var message = new TicketMessage
            {
                TicketId = ticketId,
                MessageText = text,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.TicketMessages.Add(message);
            await _context.SaveChangesAsync();

            var dto = new MessageDto
            {
                Id = message.Id,
                MessageText = message.MessageText,
                UserId = message.UserId,
                CreatedAt = message.CreatedAt
            };

            // Yeni mesaj gönderildiğinde ilgili ticket'a bağlı tüm kullanıcıları bilgilendir.
            await _notificationService.SendMessageNotificationAsync(ticketId, dto);

            return dto;
        }
    }
}
