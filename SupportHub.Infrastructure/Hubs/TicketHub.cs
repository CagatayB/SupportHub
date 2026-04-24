using Microsoft.AspNetCore.SignalR;

namespace SupportHub.Infrastructure.Hubs
{
    public class TicketHub : Hub
    {
        public async Task JoinTicketGroup(int ticketId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, ticketId.ToString());
        }

        public async Task LeaveTicketGroup(int ticketId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, ticketId.ToString());
        }
    }
}
