using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;
using ChatroomB_Backend.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ChatroomB_Backend.Service
{
    public class TokenServices : ITokenService
    {
        private readonly ITokenRepo _repo;

        public TokenServices(ITokenRepo repo)
        {
            _repo = repo;
        }

        public async Task ValidateRefreshToken(string refreshToken, int userId)
        {
            bool isValid = await _repo.ValidateRefreshToken(refreshToken, userId);

            if (!isValid)
            {
                throw new InvalidOperationException("Invalid or expired refresh token");
            }
        }

        public async Task ValidateAccessToken(int userId, string username)
        {
            string usernameInDB = await _repo.ValidateAccessToken(userId);

            if (username != usernameInDB)
            {
                throw new InvalidOperationException("Invalid access token");
            }
        }

        public async Task StoreRefreshToken(RefreshToken token)
        {
            bool isSuccess = await _repo.StoreRefreshToken(token); 

            if (!isSuccess)
            {
                throw new Exception("Failed to store refresh token");
            }
        }

        public async Task RemoveRefreshToken(string token)
        {
            bool isSuccess = await _repo.RemoveRefreshToken(token);

            if (!isSuccess)
            {
                throw new Exception("Refresh token not found");
            }
        }


        public async Task UpdateRefreshToken(string token)
        {
            bool isSuccess = await _repo.UpdateRefreshToken(token);

            if(!isSuccess)
            {
                throw new Exception("Refresh token not found");
            }
        }
    }
}
