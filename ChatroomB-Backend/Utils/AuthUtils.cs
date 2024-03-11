using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ChatroomB_Backend.Utils
{
    public class AuthUtils : IAuthUtils
    {
        public string GenerateSalt()
        {
            byte[] saltBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] saltedPasswordBytes = Encoding.UTF8.GetBytes(password + salt);
                byte[] hashedBytes = sha256.ComputeHash(saltedPasswordBytes);
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public int ExtractUserIdFromJWT(ClaimsPrincipal user)
        {
            ClaimsIdentity? identity = user.Identity as ClaimsIdentity;
            if (identity == null)
            {
                throw new UnauthorizedAccessException("User identity not found");
            }

            Claim userIdClaim = identity.FindFirst("UserId")!;
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("User ID claim not found in the token");
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new UnauthorizedAccessException("Invalid User ID claim value");
            }

            return userId;
        }

        public string ExtractUsernameFromJWT(ClaimsPrincipal user)
        {
            ClaimsIdentity? identity = user.Identity as ClaimsIdentity;
            if (identity == null)
            {
                throw new UnauthorizedAccessException("User identity not found");
            }

            Claim usernameClaim = identity.FindFirst("Username")!;
            if (usernameClaim == null)
            {
                throw new UnauthorizedAccessException("Username claim not found in the token");
            }

            return usernameClaim.Value;
        }

    }
}
