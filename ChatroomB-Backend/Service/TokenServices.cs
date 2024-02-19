using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;
using ChatroomB_Backend.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ChatroomB_Backend.Service
{
    public class TokenServices : ITokenService
    {
        private readonly ITokenRepo _repo;
        private readonly ITokenUtils _tokenUtils;
        private readonly IUserService _userService;

        public TokenServices(ITokenRepo repo, ITokenUtils tokenUtils, IUserService userService)
        {
            _repo = repo;
            _tokenUtils = tokenUtils;
            _userService = userService;
        }

        public async Task<string> RenewAccessToken(RefreshToken refreshToken, int userId)
        {
            bool isValid = await _repo.IsRefreshTokenValid(refreshToken);

            if (!isValid)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");
            }

            string username = await _userService.GetUserName(userId);

            // If valid, generate a new access token
            string newAccessToken = _tokenUtils.GenerateAccessToken(userId, username);

            return newAccessToken;
        }

        public async Task<IActionResult> StoreRefreshToken(RefreshToken token)
        {
            return await _repo.StoreRefreshToken(token);
        }

        public async Task<IActionResult> RemoveRefreshToken(RefreshToken token)
        {
            return await _repo.RemoveRefreshToken(token);
        }

        public async Task<IActionResult> ValidateRefreshToken(RefreshToken token)
        {
            return await _repo.ValidateRefreshToken(token);
        }
    }
}
