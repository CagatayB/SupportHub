using Microsoft.EntityFrameworkCore;
using SupportHub.Application.Interfaces;
using SupportHub.Domain.Entities;

namespace SupportHub.Infrastructure.Persisteance
{
    public class SupportHubDbContext : DbContext, IApplicationDbContext
    {
        public SupportHubDbContext(DbContextOptions<SupportHubDbContext> options) : base(options) { }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketMessage> TicketMessages { get; set; }
        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure entity relationships and constraints here if needed
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique()
                .HasFilter("[Email] IS NOT NULL AND [Email] != '' AND [IsDeleted] = 0");
        }

    }

}