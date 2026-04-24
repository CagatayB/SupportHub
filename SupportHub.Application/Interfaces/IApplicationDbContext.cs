using Microsoft.EntityFrameworkCore;
using SupportHub.Domain.Entities;

namespace SupportHub.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Ticket> Tickets { get; set; }
        DbSet<TicketMessage> TicketMessages { get; set; }
        DbSet<User> Users { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
