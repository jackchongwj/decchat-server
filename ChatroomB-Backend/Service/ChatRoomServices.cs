
using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Repository;

namespace ChatroomB_Backend.Service
{
    public class ChatRoomServices : IChatRoomService
    {
        private readonly IChatRoomRepo _repo;

        public ChatRoomServices(IChatRoomRepo _repository)
        {
            _repo = _repository;
        }

        public async Task<int> AddChatRoom(FriendRequest request)
        {
            return (await _repo.AddChatRoom(request));
        }
    }
}
