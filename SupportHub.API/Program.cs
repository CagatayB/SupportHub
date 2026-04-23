using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SupportHub.Application.Interfaces;
using SupportHub.Application.Services;
using SupportHub.Infrastructure.Persisteance;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- Veritabanı ve Context Kayıtları ---
builder.Services.AddDbContext<SupportHubDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ISupportHubDbContext'i SupportHubDbContext olarak kaydediyoruz, böylece uygulama içinde ISupportHubDbContext istediğimizde SupportHubDbContext örneği verilecek.
builder.Services.AddScoped<ISupportHubDbContext>(provider =>
    provider.GetRequiredService<SupportHubDbContext>());

// --- Servis Kayıtları (DI) ---
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITicketService, TicketService>();


// --- JWT Authentication Ayarları ---
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


// --- Standart Web API Servisleri --- 
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthentication(); // 1. Kimsin?
app.UseAuthorization();  // 2. Yetkin var mı?

app.MapControllers();

app.Run();