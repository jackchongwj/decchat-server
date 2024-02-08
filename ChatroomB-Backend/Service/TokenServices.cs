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

        public TokenServices(ITokenRepo repo, ITokenUtils tokenUtils)
        {
            _repo = repo;
            _tokenUtils = tokenUtils;
        }

        //public async Task<string> RenewAccessToken(RefreshToken refreshToken)
        //{
        //    bool isValid = await _repo.IsRefreshTokenValid(refreshToken);

        //    if (!isValid)
        //    {
        //        // Token is invalid or expired
        //        // You can throw an exception or handle this case as per your application's need
        //        throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        //    }

        //    // If valid, generate a new access token
        //    string newAccessToken = _tokenUtils.GenerateAccessToken(/* user info or claims */);

        //    return newAccessToken;
        //}

        public async Task<ActionResult> StoreRefreshToken(RefreshToken token)
        {
            return await _repo.StoreRefreshToken(token);
        }

        public async Task<ActionResult> RemoveRefreshToken(RefreshToken token)
        {
            return await _repo.RemoveRefreshToken(token);
        }

        public async Task<ActionResult> ValidateRefreshToken(RefreshToken token)
        {
            return await _repo.ValidateRefreshToken(token);
        }
    }
}
