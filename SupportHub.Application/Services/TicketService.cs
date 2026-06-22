using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SupportHub.Application.DTOs.Ticket;
using SupportHub.Application.Interfaces;
using SupportHub.Domain.Entities;
using static SupportHub.Domain.Enums.Enums;

namespace SupportHub.Application.Services
{
    public class TicketService : ITicketService
    {
        private readonly IApplicationDbContext _context;
        private readonly ITicketNotificationService _ticketNotificationService;


        public TicketService(IApplicationDbContext context, ITicketNotificationService ticketNotificationService)
        {
            _context = context;
            _ticketNotificationService = ticketNotificationService;
        }

        public async Task<TicketDto> CreateTicketAsync(CreateTicketRequest request, int userId)
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

            var dto = new TicketDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Status = ticket.Status.ToString(),
                Priority = ticket.Priority.ToString(),
                CreatedAt = ticket.CreatedAt
            };

            // Hub yerine interface üzerinden çağırıyoruz
            await _ticketNotificationService.NotifyTicketCreated(dto);

            return dto;
        }

        public async Task<IEnumerable<TicketDto>> GetAllTicketsAsync()
        {
            return await _context.Tickets
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .Select(t => new TicketDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = t.Status.ToString(),
                    Priority = t.Priority.ToString(),
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<TicketDto?> GetTicketByIdAsync(int id)
        {
            // Tek bir sorguda hem join yapıp hem de veriyi getiriyoruz
            var ticket = await _context.Tickets
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (ticket == null) return null;

            return new TicketDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = ticket.Status.ToString(),
                Priority = ticket.Priority.ToString(),
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt,
                CreatedByUserId = ticket.CreatedByUserId,
                CreatedByUserName = ticket.CreatedByUser?.Username ?? "Bilinmiyor",
                AssignedToUserId = ticket.AssignedToUserId,
                AssignedToUserName = ticket.AssignedToUser?.Username ?? "Henüz Atanmadı"
            };
        }

        public async Task<bool> UpdateStatusAsync(int ticketId, int status)
        {
            // 1. Talebi veritabanından bul
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
            {
                return false;
            }

            // 2. Durumu güncelle (Enum casting kullanarak)
            // Not: TicketStatus enum'ının int değerleri ile UI'dan gelen değerlerin eşleştiğinden emin ol
            ticket.Status = (TicketStatus)status;
            ticket.UpdatedAt = DateTime.UtcNow;

            // 3. Değişiklikleri kaydet
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                await _ticketNotificationService.NotifyTicketUpdated(new TicketDto
                {
                    Id = ticket.Id,
                    Status = ticket.Status.ToString(),
                    Priority = ticket.Priority.ToString(),
                    CreatedAt = ticket.CreatedAt,
                    CreatedByUserId = ticket.CreatedByUserId,
                    AssignedToUserId = ticket.AssignedToUserId
                });
                return true;
            }

            return false;
        }

        public async Task<bool> AssignTicketAsync(int ticketId, int StaffUserId)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null) return false;

            // Talebi personelle ilişkilendir ve durumunu güncelle
            ticket.AssignedToUserId = StaffUserId;
            ticket.Status = TicketStatus.InProgress;
            ticket.UpdatedAt = DateTime.UtcNow;

            var result = await _context.SaveChangesAsync();


            if (result > 0)
            {
                await _ticketNotificationService.NotifyTicketUpdated(new TicketDto
                {
                    Id = ticket.Id,
                    Status = ticket.Status.ToString(),
                    Priority = ticket.Priority.ToString(),
                    CreatedAt = ticket.CreatedAt,
                    CreatedByUserId = ticket.CreatedByUserId,
                    AssignedToUserId = ticket.AssignedToUserId
                });
                return true;
            }
            return false;
        }
    }
}
