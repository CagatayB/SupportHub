using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SupportHub.Application.Interfaces;
using SupportHub.Application.Services;
using SupportHub.Infrastructure.Data;
using SupportHub.Infrastructure.Hubs;
using SupportHub.Infrastructure.Persisteance;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- Veritabanı ve Context Kayıtları ---
builder.Services.AddDbContext<SupportHubDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// IApplicationDbContext'i SupportHubDbContext olarak kaydediyoruz, böylece uygulama içinde IApplicationDbContext istediğimizde SupportHubDbContext örneği verilecek.
builder.Services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetRequiredService<SupportHubDbContext>());

// --- (DI) ---
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ITicketNotificationService, TicketNotificationService>();
builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<SupportHubDbContext>());
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// --- JWT Authentication ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Token"]!)),
        ValidateIssuer = false,
        ValidateAudience = false,
        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:5001") // Blazor port
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});




// --- Web API Servisleri ---
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("BlazorPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<TicketHub>("/hubs/tickets");

// Database seeding process
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    try
//    {
//        var context = services.GetRequiredService<SupportHubDbContext>();

//        // Eğer bekleyen migration varsa önce onları uygular (Code-First için harikadır)
//        await context.Database.MigrateAsync();

//        // Tohumlama işlemini başlat
//        await DataSeeder.SeedUsersAsync(context);
//    }
//    catch (Exception ex)
//    {
//        var logger = services.GetRequiredService<ILogger<Program>>();
//        logger.LogError(ex, "Veritabanı tohumlanırken bir hata oluştu.");
//    }
//}

app.Run();