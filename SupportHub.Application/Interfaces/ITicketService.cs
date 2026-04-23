using SupportHub.Application.DTOs.Ticket;

namespace SupportHub.Application.Interfaces
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketDto>> GetAllTicketsAsync();
        Task<TicketDto?> GetTicketByIdAsync(int id);
        Task<TicketDto> CreateTicketAsync(CreateTicketRequest request, string userId);
        Task<bool> UpdateStatusAsync(int ticketId, int status);
    }
}
