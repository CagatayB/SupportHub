using Microsoft.AspNetCore.Mvc;
using SupportHub.Application.Interfaces;
using SupportHub.Application.DTOs.Auth;

namespace SupportHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) => _authService = authService;

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterRequest request) => Ok(await _authService.RegisterAsync(request));

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginRequest request)
        {
            var token = await _authService.LoginAsync(request);
            return token == null ? BadRequest("Hatalı kullanıcı adı veya şifre") : Ok(token);
        }
    }
}
