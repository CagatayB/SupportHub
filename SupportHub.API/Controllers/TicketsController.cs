using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportHub.Application.DTOs.Message;
using SupportHub.Application.DTOs.Ticket;
using SupportHub.Application.Interfaces;
using SupportHub.Application.Services;

namespace SupportHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly IMessageService _messageService;


        public TicketsController(ITicketService ticketService, IMessageService messageService)
        {
            _ticketService = ticketService;
            _messageService = messageService;
        }

        [HttpGet("{ticketId}/messages")]
        public async Task<IActionResult> GetMessages(int ticketId)
        {
            var messages = await _messageService.GetMessagesByTicketIdAsync(ticketId);
            return Ok(messages);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTicketRequest request)
        {
            // In a real application, you would get the user ID from the authenticated user context
            var userId = "test-user-123";

            var result = await _ticketService.CreateTicketAsync(request, userId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null) return NotFound();

            return Ok(ticket);
        }

        [HttpPost("{ticketId}/messages")]
        public async Task<IActionResult> SendMessage(int ticketId, [FromBody] SendMessageRequest request)
        {
            // Şimdilik test ID'si, Auth sonrası User.FindFirstValue(ClaimTypes.NameIdentifier) olacak
            var userId = "test-user-123";

            var result = await _messageService.SendMessageAsync(ticketId, request, userId);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,SupportStaff")] // Sadece yetkililer atama yapabilir
        [HttpPatch("{id}/assign")] // Kısmi güncelleme olduğu için Patch tercih edilir
        public async Task<IActionResult> AssignTicket(int id, [FromBody] AssignTicketRequest request)
        {
            if (string.IsNullOrEmpty(request.StaffUserId))
                return BadRequest("Personel ID'si boş olamaz.");

            var result = await _ticketService.AssignTicketAsync(id, request);

            if (!result)
                return NotFound($"ID'si {id} olan talep bulunamadı.");

            return Ok(new { message = $"Talep başarıyla personele (@request.StaffUserId) atandı." });
        }

        [HttpGet] // Artık api/tickets adresine gelen GET isteklerini karşılar
        public async Task<IActionResult> GetAll()
        {
            // Auth mekanizması tam oturduğunda burada kullanıcının rolüne göre 
            // filtreleme yapabilirsin (Admin hepsini, User sadece kendisininkileri görür).
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }
    }
}
