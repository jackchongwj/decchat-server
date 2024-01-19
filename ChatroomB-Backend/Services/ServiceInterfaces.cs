using ChatroomB_Backend.Models;
using NuGet.Protocol.Core.Types;

namespace ChatroomB_Backend.Service
{
    public interface IUserService
    {
        Task<IEnumerable<Users>> GetByName(string profileName);                                                         //Get user by user profile name
    }

    public interface IFriendService 
    {
        Task<int> AddFriends(Friends friends);                                                                                 // add new friend 
    }

    public interface IAuthService
    {
        void SetPassword(Users user, string password);
        bool VerifyPassword(Users user, string password);
    }
}
