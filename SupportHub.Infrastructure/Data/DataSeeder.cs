using SupportHub.Domain.Entities;
using SupportHub.Infrastructure.Persisteance;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using static SupportHub.Domain.Enums.Enums;

// <summary>
/*
    This class is responsible for seeding the database with initial user data.

 */


namespace SupportHub.Infrastructure.Data
{
    public static class DataSeeder
    {
        public static async Task SeedUsersAsync(SupportHubDbContext context)
        {
            // If there are already users in the database, cancel the seeding process.
            if (await context.Users.AnyAsync())
                return;

            // We choose a common and easy-to-remember password.
            string defaultPassword = BCrypt.Net.BCrypt.HashPassword("Test1234!");

            var sampleUsers = new List<User>
        {
            new User
            {
                Username = "SuperAdmin",
                Email = "admin@supporthub.com",
                PasswordHash = defaultPassword,
                Role = UserRole.Admin,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Username = "AhmetManager",
                Email = "manager@supporthub.com",
                PasswordHash = defaultPassword,
                Role = UserRole.Manager,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Username = "AyseLead",
                Email = "lead@supporthub.com",
                PasswordHash = defaultPassword,
                Role = UserRole.TeamLead,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Username = "MehmetStaff",
                Email = "staff@supporthub.com",
                PasswordHash = defaultPassword,
                Role = UserRole.SupportStaff,
                CreatedAt = DateTime.UtcNow
            }
        };

            await context.Users.AddRangeAsync(sampleUsers);
            await context.SaveChangesAsync();
        }
    }
}
