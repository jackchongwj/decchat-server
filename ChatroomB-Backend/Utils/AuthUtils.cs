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

        public ActionResult<int> ExtractUserIdFromJWT(ClaimsPrincipal user)
        {
            ClaimsIdentity? identity = user.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return new UnauthorizedObjectResult("User identity not found");
            }

            Claim userIdClaim = identity.FindFirst("userId")!; // Claim name should match the one used in the token creation
            if (userIdClaim == null)
            {
                return new UnauthorizedObjectResult("User ID claim not found in the token");
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return new BadRequestObjectResult("Invalid User ID claim value");
            }

            return userId;
        }
            
    }
}
