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
            var query = from m in _context.TicketMessages
                        join u in _context.Users on m.UserId equals u.Id
                        where m.TicketId == ticketId && !m.IsDeleted
                        orderby m.CreatedAt
                        select new MessageDto
                        {
                            Id = m.Id,
                            TicketId = m.TicketId,
                            UserId = m.UserId,     
                            UserName = u.Username,
                            MessageText = m.MessageText,
                            CreatedAt = m.CreatedAt,
                            UpdatedAt = m.UpdatedAt
                        };
            return await query.ToListAsync();
        }

        public async Task<MessageDto> SendMessageAsync(int ticketId, SendMessageRequest request, int userId)
        {

            var ticketExists = await _context.Tickets.AnyAsync(t => t.Id == ticketId && !t.IsDeleted);
            if (!ticketExists)
            {
                throw new Exception("Ticket not found.");
            }

            var userExists = await _context.Users
                .Select(u => new {u.Id, u.Username })
                .FirstOrDefaultAsync(u => u.Id == userId);

            if(userExists == null)
            {
                throw new Exception("User not found.");
            }

            var message = new TicketMessage
            {
                TicketId = ticketId,
                MessageText = request.MessageText,
                UserId = userId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.TicketMessages.Add(message);
            await _context.SaveChangesAsync();

            var dto = new MessageDto
            {
                Id = message.Id,
                TicketId = message.TicketId,
                MessageText = message.MessageText,
                UserId = message.UserId,
                UserName = userExists.Username,
                CreatedAt = message.CreatedAt
            };

            // When a new message is sent, notify all users associated with the relevant ticket.
            await _notificationService.SendMessageNotificationAsync(ticketId, dto);

            return dto;
        }
    }
}
