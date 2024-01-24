using ChatroomB_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace ChatroomB_Backend.Repository
{
    public interface IUserRepo
    {
        Task<IEnumerable<Users>> GetByName(string profileName);                                                         //Get user by user profile name
        Task<bool> IsUsernameUnique(string username);
        Task<int> GetUserId(string username);
    }   

    public interface IFriendRepo
    {
        Task<int> AddFriends(Friends friends);                                                                                 // add new friend 
    }

    public interface IAuthRepo
    {
        Task<string> GetSalt(string username);
        Task<bool> VerifyPassword(string username, string hashedPassword);
        Task<ActionResult> AddUser(Users user);
    }

    public interface ITokenRepo
    {
        Task<ActionResult> StoreRefreshToken(RefreshToken token);
        Task<ActionResult> RemoveRefreshToken(RefreshToken token);
    }
}
