using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportHub.Application.DTOs.Auth;
using SupportHub.Application.Interfaces;

namespace SupportHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) => _authService = authService;

        
        [HttpPost("register")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(new { Message = result });
        }

        //public async Task<ActionResult> Register(RegisterRequest request) => Ok(await _authService.RegisterAsync(request));

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginRequest request)
        {
            var token = await _authService.LoginAsync(request);
            return token == null ? BadRequest("Hatalı kullanıcı adı veya şifre") : Ok(token);
        }
    }
}
