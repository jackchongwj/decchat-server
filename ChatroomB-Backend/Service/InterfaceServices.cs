using ChatroomB_Backend.Models;
using NuGet.Protocol.Core.Types;
using ChatroomB_Backend.DTO;

namespace ChatroomB_Backend.Service
{
    public interface IUserService
    {
        Task<IEnumerable<Users>> GetByName(string profileName);                                                         //Get user by user profile name
        Task<IEnumerable<ChatlistVM>> GetChatListByUserId(int userId); //return chatlist
    }

    public interface IFriendService 
    {
        Task<int> AddFriends(Friends friends);  // add new friend 

    }
}
