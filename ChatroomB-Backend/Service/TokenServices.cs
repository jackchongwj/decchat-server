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

        public TokenServices(ITokenRepo repo, ITokenUtils tokenUtils, IUserService userService)
        {
            _repo = repo;
            _tokenUtils = tokenUtils;
        }

        public async Task<string> RenewAccessToken(RefreshToken token, int userId, string username)
        {
            bool isValid = await _repo.IsRefreshTokenValid(token, userId, username);

            // Remove refresh token if its invalid or expired
            if (!isValid)
            {
                await RemoveRefreshToken(token);
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");
            }

            // Roll the expiry date forward to current time + 7 days
            await UpdateRefreshToken(token);

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

        public async Task<IActionResult> UpdateRefreshToken(RefreshToken token)
        {
            return await _repo.UpdateRefreshToken(token);
        }

    }
}
