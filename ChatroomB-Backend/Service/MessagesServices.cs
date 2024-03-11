using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Hubs;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatroomB_Backend.Service
{
    public class MessagesServices : IMessageService
    {
        private readonly IMessageRepo _MessageRepo;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessagesServices(IMessageRepo messageRepo, IHubContext<ChatHub> hubContext)
        {
            this._MessageRepo = messageRepo;
            this._hubContext = hubContext;
        }

        public async Task <ChatRoomMessage> AddMessages(Messages message)
        {
            TimeZoneInfo singaporeTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");

            message.TimeStamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, singaporeTimeZone);

            ChatRoomMessage newMessage =  await _MessageRepo.AddMessages(message);

            await _hubContext.Clients.Group(newMessage.ChatRoomId.ToString()!).SendAsync("UpdateMessage", newMessage);
            return newMessage;
        }

        public async Task<int> DeleteMessage(int MessageId, int ChatRoomId)
        {
            int result = await _MessageRepo.DeleteMessage(MessageId);
            if(result != 0)
            {
                await _hubContext.Clients.Group(ChatRoomId.ToString()).SendAsync("DeleteMessage", MessageId);
            }

            return result;
        }

        public async Task<int> EditMessage(EditMessage NewMessage)
        {
            int result = await _MessageRepo.EditMessage(NewMessage);
            if (result == 0)
            {
                await _hubContext.Clients.Group(NewMessage.ChatRoomId.ToString()!).SendAsync("EditMessage", NewMessage);
            }

            return result;
        }

        public async Task<IEnumerable<ChatRoomMessage>> GetMessages(int ChatRoomId, int MessageId)
        {
            return await _MessageRepo.GetMessages(ChatRoomId, MessageId);
        }

        public async Task<int> GetTotalSearchMessage(int ChatRoomId, string SearchValue)
        {
            return await _MessageRepo.GetTotalSearchMessage(ChatRoomId, SearchValue);
        }
    }
}
