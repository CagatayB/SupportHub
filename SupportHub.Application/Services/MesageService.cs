using Microsoft.AspNetCore.SignalR;
using SupportHub.Application.DTOs.Message;
using SupportHub.Application.Interfaces;
using SupportHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<MessageDto>> GetMessagesByTicketIdAsync(int ticketId)
        {
            return await _context.TicketMessages
                .Where(m => m.TicketId == ticketId && !m.IsDeleted)
                .OrderBy(m => m.CreatedAt)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    TicketId = m.TicketId,
                    MessageText = m.MessageText,
                    UserName = "Kullanıcı " + m.UserId, // İleride Identity ile gerçek isim gelecek
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt,
                    IsOwner = false // Auth eklenince düzelecek
                })
                .ToListAsync();
        }

        public async Task<MessageDto> SendMessageAsync(int ticketId, SendMessageRequest request, string userId)
        {
            var message = new TicketMessage
            {
                TicketId = ticketId,
                MessageText = request.MessageText,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.TicketMessages.Add(message);
            await _context.SaveChangesAsync();

            var dto = new MessageDto
            {
                Id = message.Id,
                TicketId = message.TicketId,
                MessageText = message.MessageText,
                UserName = "User",
                CreatedAt = message.CreatedAt,
                IsOwner = true
            };

            // Yeni mesaj gönderildiğinde ilgili ticket'a bağlı tüm kullanıcıları bilgilendir.
            await _notificationService.SendMessageNotificationAsync(ticketId, dto);

            return dto;
        }
    }
}
