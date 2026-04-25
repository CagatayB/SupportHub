using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SupportHub.Application.Interfaces;
using SupportHub.Application.Services;
using SupportHub.Infrastructure.Hubs;
using SupportHub.Infrastructure.Persisteance;
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
        ClockSkew = TimeSpan.Zero // Token süresi bittiği an yetkiyi kesmek için
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

app.Run();