using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;
using ChatroomB_Backend.Utils;

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
            bool doesExist = await _userService.DoesUsernameExist(username);

            if (!doesExist)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            Users user = await _repo.GetUserCredentials(username);

            string hashedPassword = _authUtils.HashPassword(password, user.Salt);

            if (hashedPassword != user.HashedPassword)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            return user;
        }

        public async Task<string> GetSalt(string username)
        {
            return await _repo.GetSalt(username);
        }

        public async Task<bool> VerifyPassword(string username, string hashedPassword)
        {
            return await _repo.VerifyPassword(username, hashedPassword);
        }

        public async Task AddUser(string username, string password, string profileName)
        {
            bool isUnique = await _userService.DoesUsernameExist(username);

            if (isUnique)
            {
                throw new ArgumentException("Duplicate username detected");
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
                throw new Exception("Registration failed");
            }
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
