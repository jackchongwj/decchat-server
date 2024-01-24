using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Models;
using NuGet.Protocol.Core.Types;
using ChatroomB_Backend.DTO;

namespace ChatroomB_Backend.Service
{
    public interface IUserService
    {
        Task<IEnumerable<UserSearch>> GetByName(string profileName, int userId);                                                   //Get user by user profile name and filter friend request
        Task<IEnumerable<Users>> GetFriendRequest(int userId);                                                               //Get All Friend request
        Task<Users> GetUserById(int userId);
        Task<int> UpdateUser(Users user);
        Task<int> DeleteUser(int userId);
        Task<int> ChangePassword(int userId, string newPassword);
        Task<IEnumerable<Users>> GetByName(string profileName);                                                         //Get user by user profile name
        Task<IEnumerable<ChatlistVM>> GetChatListByUserId(int userId); //return chatlist
    }

    public interface IFriendService 
    {
        Task<int> AddFriends(Friends friends);                                                                                 // add new friend
        Task<int> UpdateFriendRequest(FriendRequest request);                                              // update friend request                                                                      // 

    }

    public interface IChatRoomService
    {
        Task<int> AddChatRoom(FriendRequest request);                                             // add new ChatRoom and user chat room with private user
    }


    public interface IMessageService
    {
        Task<int> AddMessages(Messages message);                                                                                 // add new friend 
    }
}
