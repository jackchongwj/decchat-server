using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;


namespace ChatroomB_Backend.Repository
{
    public interface IUserRepo
    {
        Task<IEnumerable<UserSearch>> GetByName(string profileName, int userId);                                                    //Get user by user profile name and filter friend request
        Task<IEnumerable<Users>> GetFriendRequest(int userId);                                                                 // Get All Friend request
    }   

    public interface IFriendRepo
    {
        Task<int> AddFriends(Friends friends);                                                                                 // Add new friend 
        Task<int> UpdateFriendRequest (FriendRequest request);                                              // update friend request
       
    }

    public interface IChatRoomRepo
    {
        Task<int> AddChatRoom(FriendRequest request);                                             // add new ChatRoom and user chat room with private user
    }
}
