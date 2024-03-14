using Amazon.Runtime.Internal.Util;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;
using ChatroomB_Backend.Utils;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

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

        public async Task<Users> Authenticate(string username, string password)
        {
            Users user = await _repo.GetUserCredentials(username);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            string hashedPassword = _authUtils.HashPassword(password, user.Salt);

            if (hashedPassword != user.HashedPassword)
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            return user;
        }

        public async Task AddUser(string username, string password, string profileName)
        {
            bool isUnique = await _userService.DoesUsernameExist(username);

            if (isUnique)
            {
                throw new InvalidOperationException("This username is unavailable");
            }

            string salt = _authUtils.GenerateSalt();
            string hashedPassword = _authUtils.HashPassword(password, salt);

            Users user = new Users
            {
                UserName = username,
                ProfileName = profileName,
                HashedPassword = hashedPassword,
                Salt = salt,
            };

            bool isSuccess = await _repo.AddUser(user);

            if (!isSuccess)
            {
                throw new InvalidOperationException("Registration failed. No records were updated.");
            }
        }

        public async Task ChangePassword(string username, string currentPassword, string newPassword)
        {
            Users user = await _repo.GetUserCredentials(username);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            string currentPasswordHashed = _authUtils.HashPassword(currentPassword, user.Salt);

            if (currentPasswordHashed != user.HashedPassword)
            {
                throw new UnauthorizedAccessException("Current password entered is incorrect.");
            }

            string newPasswordHashed = _authUtils.HashPassword(newPassword, user.Salt);

            bool isSuccess = await _repo.ChangePassword(username, newPasswordHashed);

            if (!isSuccess)
            {
                throw new InvalidOperationException("Change password failed. No records were updated.");
            }

        }

    }
}
