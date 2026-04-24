namespace SupportHub.Application.Interfaces
{
    public interface ITicketHubService
    {
        Task SendMessageToGroupAsync(int ticketId, object message);
    }
}
