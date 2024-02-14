using System.Security.Claims;

namespace ChatroomB_Backend.Utils
{
    public interface IAuthUtils
    {
        string GenerateSalt();
        string HashPassword(string password, string salt);
    }
    
    public interface ITokenUtils
    {
        string GenerateAccessToken(int userId);
        string GenerateRefreshToken();
    }
}
