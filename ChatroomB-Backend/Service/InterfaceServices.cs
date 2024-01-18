using ChatroomB_Backend.Models;
using NuGet.Protocol.Core.Types;

namespace ChatroomB_Backend.Service
{
    public interface IUserService
    {
        Task<IEnumerable<Users>> GetByName(string profileName, int userId);                                                   //Get user by user profile name and filter friend request
        Task<IEnumerable<Users>> GetFriendRequest(int userId);                                                               //Get All Friend request
    }

    public interface IFriendService 
    {
        Task<int> AddFriends(Friends friends);                                                                                 // add new friend 

    }
}
