using Microsoft.EntityFrameworkCore;
using SupportHub.Domain.Entities;

namespace SupportHub.Application.Interfaces
{
    public interface ISupportHubDbContext
    {
        DbSet<Ticket> Tickets { get; set; }
        DbSet<TicketMessage> TicketMessages { get; set; }
        DbSet<User> Users { get; set; }

        // SaveChangesAsync metodunu interface'e ekliyoruz ki Service içinden çağırabilelim
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
