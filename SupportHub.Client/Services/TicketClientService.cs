using Microsoft.AspNetCore.SignalR.Client;
using SupportHub.Application.DTOs.Ticket;
using System.Diagnostics.Tracing;
using System.Net.Http.Json;

namespace SupportHub.Client.Services
{
    public class TicketClientService
    {
        private readonly HttpClient _http;
        private HubConnection _hubConnection;

        public List<TicketDto> Tickets { get; set; } = new();
        public event Action? OnDataChanged; // UI triggered when data changes

        public TicketClientService(HttpClient http, IConfiguration config)
        {
            _http = http;

            // Initialize SignalR connection
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(config["SignalRHubUrl"] + "/hubs/tickets" ?? throw new InvalidOperationException("SignalRHubUrl not configured"))
                .WithAutomaticReconnect()
                .Build();

            // Listen for updates from the server
            _hubConnection.On<TicketDto>("TicketUpdated", (updatedTicket) => {
                var index = Tickets.FindIndex(t => t.Id == updatedTicket.Id);

                if (index != -1)
                {
                    // Mevcut ise güncelle
                    Tickets[index] = updatedTicket;
                }
                else
                {
                    // Yeni ise listenin başına ekle (Sayaç bu sayede artacaktır)
                    Tickets.Insert(0, updatedTicket);
                }

                OnDataChanged?.Invoke();
            });
        }

        public async Task StartAsync()
        {
            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
                Console.WriteLine("SignalR Bağlantısı Kuruldu: " + _hubConnection.ConnectionId);
            }
        }

        public async Task LoadTicketsAsync()
        {
            Tickets = await _http.GetFromJsonAsync<List<TicketDto>>("api/tickets") ?? new();
            OnDataChanged?.Invoke();
        }
    }
}
