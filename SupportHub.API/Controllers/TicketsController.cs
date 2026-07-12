using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportHub.Application.DTOs.Message;
using SupportHub.Application.DTOs.Ticket;
using SupportHub.Application.Interfaces;
using SupportHub.Application.Services;
using SupportHub.Domain.Entities;
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


        [HttpPost]
        [Authorize] //Only users who are logged into the system (and have the Token) can submit requests.
        public async Task<IActionResult> Create([FromBody] CreateTicketRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            { 
                return Unauthorized("User information could not be retrieved.");
            }    
            
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
       
        
        [HttpPatch("{id}/assign")]
        [Authorize(Roles = "Admin,Manager,TeamLead")]
        public async Task<IActionResult> AssignTicket(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int StaffUserId))
                return Unauthorized("Kullanıcı bilgisi alınamadı.");

            var result = await _ticketService.AssignTicketAsync(id, StaffUserId);

            return result ? Ok(new { Message = "Talep başarıyla atandı." }) : NotFound("Talebe ulaşılamadı veya atanamadı.");
        }



        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }


        [HttpPatch("{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] int status)
        {
            var result = await _ticketService.UpdateStatusAsync(id, status);
            if (!result) return NotFound("Talebe ulaşılamadı veya güncellenemedi.");

            return Ok(new { Message = "Durum başarıyla güncellendi." });
        }
    }
}
