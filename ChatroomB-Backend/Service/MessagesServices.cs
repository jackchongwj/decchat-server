using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Hubs;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;
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
            message.TimeStamp = DateTime.Now;

            ChatRoomMessage newMessage =  await _MessageRepo.AddMessages(message);

            await _hubContext.Clients.Group("PR"+ newMessage.ChatRoomId.ToString()).SendAsync("UpdateMessage", newMessage);
            return newMessage;
        }

        public async Task<IEnumerable<ChatRoomMessage>> GetMessages(int ChatRoomId)
        {
            return await _MessageRepo.GetMessages(ChatRoomId);
        }
    }
}
