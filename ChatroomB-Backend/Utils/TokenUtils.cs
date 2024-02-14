using ChatroomB_Backend.Service;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ChatroomB_Backend.Utils
{
    public class TokenUtils : ITokenUtils
    {
        private readonly IConfiguration _config;

        public TokenUtils(IConfiguration config)
        {
            _config = config;

        }

        public string GenerateAccessToken(int userId)
        {
            DateTime expiryDateTime = DateTime.Now.AddMinutes(Convert.ToInt32(_config["JwtSettings:ExpirationMinutes"]));
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) },
                expires: expiryDateTime,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[32];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

    }
}
