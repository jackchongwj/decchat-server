using ChatroomB_Backend.Models;
using ChatroomB_Backend.Repository;
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

        public async Task<IActionResult> StoreRefreshToken(RefreshToken refreshToken)
        {
            return await _repo.StoreRefreshToken(refreshToken);
        }
    }
}
