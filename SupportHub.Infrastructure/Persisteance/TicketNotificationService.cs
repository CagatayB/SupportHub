using Microsoft.AspNetCore.SignalR;
using SupportHub.Application.DTOs.Message;
using SupportHub.Application.Interfaces;
using SupportHub.Infrastructure.Hubs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SupportHub.Infrastructure.Persisteance
{
    public class TicketNotificationService : ITicketNotificationService
    {
        private readonly IHubContext<TicketHub> _hubContext;

        public TicketNotificationService(IHubContext<TicketHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendMessageNotificationAsync(int ticketId, MessageDto message)
        {
            await _hubContext.Clients.Group(ticketId.ToString())
                .SendAsync("ReceiveMessage", message);
        }
    }
}
