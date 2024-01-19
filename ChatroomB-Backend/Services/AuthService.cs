using ChatroomB_Backend.Models;
using System.Security.Cryptography;
using System.Text;

namespace ChatroomB_Backend.Service
{
    public class AuthService : IAuthService
    {
        //public AuthService(IAuthRepo authService) 
        //{ 
        
        //}
        public void SetPassword(Users user, string password)
        {

        }

        public bool VerifyPassword(Users user, string password)
        {
            string hashedPasswordToCheck = HashPassword(password);
            return user.HashedPassword == hashedPasswordToCheck;
        }

        private static string GenerateSalt()
        {
            byte[] saltBytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        private static string HashPassword(string password)
        {
            string salt = GenerateSalt();
            using (var sha256 = SHA256.Create())
            {
                byte[] saltedPasswordBytes = Encoding.UTF8.GetBytes(password + salt);
                byte[] hashedBytes = sha256.ComputeHash(saltedPasswordBytes);
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
