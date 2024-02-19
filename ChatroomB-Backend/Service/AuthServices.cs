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
            var user = await _userService.GetUserById(userId);
            if (user == null) return false;

            // Get the salt for the user and hash the current password
            string salt = await GetSalt(user.UserName);
            string hashedCurrentPassword = _authUtils.HashPassword(currentPassword, salt);

            // Verify the current password using the VerifyPassword method
            bool isCurrentPasswordValid = await _repo.VerifyPassword(user.UserName, hashedCurrentPassword);
            if (!isCurrentPasswordValid) return false;

            // Hash the new password with the same salt
            string newHashedPassword = _authUtils.HashPassword(newPassword, salt);

            // Call the repository to update the password
            return await _repo.ChangePassword(userId, newHashedPassword);
        }
    }
}
