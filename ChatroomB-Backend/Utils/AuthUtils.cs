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

    }
}
