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
        Task<IActionResult> AddUser(Users user);
        Task<bool> VerifyPassword(string username, string hashedPassword);
    }

    public interface ITokenRepo
    {
        Task<IActionResult> StoreRefreshToken(RefreshToken token);
    }
}
