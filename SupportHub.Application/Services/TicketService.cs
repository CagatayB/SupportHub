using SupportHub.Application.DTOs.Ticket;
using SupportHub.Application.Interfaces;
using SupportHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using static SupportHub.Domain.Enums.Enums;

namespace SupportHub.Application.Services
{
    public class TicketService : ITicketService
    {
        // Gelecek adımda Repository eklenecek, şimdilik DbContext ile ilerleyebiliriz
        private readonly IApplicationDbContext _context;

        public TicketService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TicketDto> CreateTicketAsync(CreateTicketRequest request, string userId)
        {
            var ticket = new Ticket
            {
                Title = request.Title,
                Description = request.Description,
                Priority = (TicketPriority)request.Priority,
                CreatedByUserId = userId,
                Status = TicketStatus.Open,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return new TicketDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Status = ticket.Status.ToString()
                // Diğer alanlar...
            };
        }

        public Task<IEnumerable<TicketDto>> GetAllTicketsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<TicketDto?> GetTicketByIdAsync(int id)
        {
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (ticket == null)
                return null;

            return new TicketDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = ticket.Status.ToString(),
                Priority = ticket.Priority.ToString(),
                CreatedAt = ticket.CreatedAt,
                CreatedByUserId = ticket.CreatedByUserId,
                AssignedToUserId = ticket.AssignedToUserId
            };
        }

        public Task<bool> UpdateStatusAsync(int ticketId, int status)
        {
            throw new NotImplementedException();
        }

        // Diğer metodların implementasyonları...
    }
}
