using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;

namespace ChatroomB_Backend.Service
{
    public class MessagesServices : IMessageService
    {
        private readonly IMessageRepo _MessageRepo;

        public MessagesServices(IMessageRepo messageRepo)
        {
            this._MessageRepo = messageRepo;
        }

        public async Task<int> AddMessages(Messages message)
        {
            message.TimeStamp = DateTime.Now;

            return await _MessageRepo.AddMessages(message);
        }

        public async Task<IEnumerable<Messages>> GetMessages(int ChatRoomId)
        {
            return await _MessageRepo.GetMessages(ChatRoomId);
        }
    }
}
