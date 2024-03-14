using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ChatroomB_Backend.Utils
{
    public class TokenUtils : ITokenUtils
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _environment;

        public TokenUtils(IConfiguration config, IWebHostEnvironment environment)
        {
            _config = config;
            _environment = environment;
        }

        public string GenerateAccessToken(int userId, string username)
        {
            DateTime expiryDateTime = DateTime.Now.AddMinutes(Convert.ToInt32(_config["JwtSettings:ExpirationMinutes"]));
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create a list of claims with both userId and username
            List<Claim> claims = new List<Claim>
            {
                new Claim("UserId", userId.ToString()),
                new Claim("Username", username)
            };

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: expiryDateTime,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken(int userId)
        {
            byte[] randomNumber = new byte[32];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                string token = Convert.ToBase64String(randomNumber);

                DateTime expiryDateTime = DateTime.UtcNow.AddDays(Convert.ToInt32(_config["RefreshTokenSettings:ExpirationDays"]));

                return new RefreshToken
                {
                    UserId = userId,
                    Token = token,
                    ExpiredDateTime = expiryDateTime
                };
            }
        }
    }
}
