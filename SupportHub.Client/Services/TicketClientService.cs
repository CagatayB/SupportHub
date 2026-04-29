using Microsoft.AspNetCore.SignalR.Client;
using SupportHub.Application.DTOs.Message;
using SupportHub.Application.DTOs.Ticket;
using System.Net.Http.Json;

namespace SupportHub.Client.Services
{
    public class TicketClientService
    {
        private readonly HttpClient _http;
        public HubConnection HubConnection { get; private set; }

        public List<TicketDto> Tickets { get; set; } = new();
        public event Action? OnDataChanged; // UI triggered when data changes

        public TicketClientService(HttpClient http, IConfiguration config)
        {
            _http = http;

            // Initialize SignalR connection
            HubConnection = new HubConnectionBuilder()
                .WithUrl(config["ApiBaseUrl"] + "/hubs/tickets" ?? throw new InvalidOperationException("SignalRHubUrl not configured"))
                .WithAutomaticReconnect()
                .Build();

            // Listen for updates from the server
            HubConnection.On<TicketDto>("TicketUpdated", (updatedTicket) => {
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
            try
            {
                if (HubConnection.State == HubConnectionState.Disconnected)
                {
                    await HubConnection.StartAsync();
                    Console.WriteLine($"Bağlantı Başarılı: {HubConnection.ConnectionId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR Hatası: {ex.Message}");
            }
        }

        public async Task LoadTicketsAsync()
        {
            Tickets = await _http.GetFromJsonAsync<List<TicketDto>>("api/tickets") ?? new();
            OnDataChanged?.Invoke();
        }

        public async Task<TicketDto?> GetTicketDetailAsync(int id)
        {
            return await _http.GetFromJsonAsync<TicketDto>($"api/tickets/{id}");
        }

        public async Task<List<MessageDto>> GetTicketMessagesAsync(int ticketId)
        {
            // API tarafında mesajları çeken bir endpoint olduğunu varsayıyoruz
            return await _http.GetFromJsonAsync<List<MessageDto>>($"api/tickets/{ticketId}/messages") ?? new();
        }

        public async Task SendMessageAsync(int ticketId, string text)
        {
            var request = new { TicketId = ticketId, MessageText = text };
            await _http.PostAsJsonAsync($"api/tickets/{ticketId}/messages", request);
        }

        public async Task<bool> UpdateStatusAsync(int ticketId, int status)
        {
            var response = await _http.PatchAsJsonAsync($"api/tickets/{ticketId}/status", status);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AssignTicketAsync(int ticketId)
        {
            var response = await _http.PatchAsync($"api/tickets/{ticketId}/assign", null);
            return response.IsSuccessStatusCode;
        }
    }
}
