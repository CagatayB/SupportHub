using SupportHub.Application.DTOs.Message;
using System;
using System.Collections.Generic;
using System.Text;


namespace SupportHub.Application.Interfaces
{
    public interface IMessageService
    {
        Task<MessageDto> SendMessageAsync(int ticketId, string text, string userId);
    }
}
