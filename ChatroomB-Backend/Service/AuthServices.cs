using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;
using ChatroomB_Backend.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace ChatroomB_Backend.Service
{
    public class AuthServices : IAuthService
    {

        private readonly IAuthRepo _repo;
        private readonly IUserService _userService;
        private readonly IAuthUtils _authUtils;

        public AuthServices(IAuthRepo repo, IUserService userService, IAuthUtils authUtils)
        {
            _repo = repo;
            _userService = userService;
            _authUtils = authUtils;
        }

        public async Task<string> GetSalt(string username)
        {
            return await _repo.GetSalt(username);
        }

        public async Task<bool> VerifyPassword(string username, string hashedPassword)
        {
            return await _repo.VerifyPassword(username, hashedPassword);
        }

        public async Task<IActionResult> AddUser(Users user)
        {
            return await _repo.AddUser(user);
        }

        public async Task<bool> ChangePassword(int userId, string currentPassword, string newPassword)
        {
            Users user = await _userService.GetUserById(userId);
            if (user == null) return false;

            string salt = await GetSalt(user.UserName);
            string hashedCurrentPassword = _authUtils.HashPassword(currentPassword, salt);

            bool isCurrentPasswordValid = await _repo.VerifyPassword(user.UserName, hashedCurrentPassword);
            if (!isCurrentPasswordValid) return false;

            string newHashedPassword = _authUtils.HashPassword(newPassword, salt);

            return await _repo.ChangePassword(userId, newHashedPassword);
        }
    }
}
