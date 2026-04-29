using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportHub.Application.DTOs.Message;
using SupportHub.Application.DTOs.Ticket;
using SupportHub.Application.Interfaces;
using SupportHub.Application.Services;
using System.Security.Claims;

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

        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTicketRequest request)
        {
            // In a real application, you would get the user ID from the authenticated user context
            var userId = "123"; // In a real application, you would get the user ID from the authenticated user context

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
            string userId = "123";

            var result = await _messageService.SendMessageAsync(ticketId, request, userId);
            return Ok(result);
        }


        //[Authorize(Roles = "Admin,SupportStaff")]
        [HttpPatch("{id}/assign")]
        public async Task<IActionResult> AssignTicket(int id)
        {
            var staffUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (staffUserId == null) return NotFound();

            var request = new AssignTicketRequest { StaffUserId = staffUserId };
            var result = await _ticketService.AssignTicketAsync(id, request);

            return result ? Ok(new { Message = "Talep başarıyla atandı." }) : NotFound("Talebe ulaşılamadı veya atanamadı.");
        }



        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }


        [HttpPatch("{id}/status")]
     // [Authorize(Roles = "Admin,SupportStaff")] // Sadece yetkili personel durum değiştirebilir
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] int status)
        {
            var result = await _ticketService.UpdateStatusAsync(id, status);
            if (!result) return NotFound("Talebe ulaşılamadı veya güncellenemedi.");

            return Ok(new { Message = "Durum başarıyla güncellendi." });
        }
    }
}
