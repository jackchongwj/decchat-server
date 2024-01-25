using ChatroomB_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using System.Security.Claims;

namespace ChatroomB_Backend.Service
{
    public interface IUserService
    {
        Task<IEnumerable<Users>> GetByName(string profileName);                                                         //Get user by user profile name
        Task<bool> IsUsernameUnique(string username);
        Task<int> GetUserId(string username);
    }

    public interface IFriendService 
    {
        Task<int> AddFriends(Friends friends);                                                                                 // add new friend 
    }

    public interface IAuthService
    {
        Task<string> GetSalt(string username);
        Task<bool> VerifyPassword(string username, string hashedPassword);
        Task<ActionResult> AddUser(Users user);
    }

    public interface ITokenService
    {
        Task<ActionResult> StoreRefreshToken(RefreshToken token);
        Task<ActionResult> RemoveRefreshToken(RefreshToken token);
    }
}
