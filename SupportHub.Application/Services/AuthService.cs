using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SupportHub.Application.Interfaces;
using SupportHub.Application.DTOs.Auth;
using SupportHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SupportHub.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IApplicationDbContext _context;

        public AuthService(IConfiguration configuration, IApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<string> RegisterAsync(RegisterRequest request)
        {
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password), // Parolayı hash'leyerek saklıyoruz
                Role = "Customer" // Varsayılan olarak tüm yeni kullanıcılar "Customer" rolüne sahip olacak
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return "Kayıt başarılı";
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return null;
            
            var token = CreateToken(user);

            return new AuthResponse { Token = token, Username = user.Username, Role = user.Role };
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role) // Kritik: Rol burada ekleniyor
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Token"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
