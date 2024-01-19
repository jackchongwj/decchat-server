﻿using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using System.Security.Cryptography;

namespace ChatroomB_Backend.Service
{
    public interface ITokenService
    {
        string GenerateAccessToken(string username);
        string GenerateRefreshToken();
        ClaimsPrincipal ValidateToken(string token);
    }

    public class TokenService : ITokenService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiryInMinutes;

        public TokenService(IConfiguration config)
        {
            _secretKey = config["JwtSettings:SecretKey"];
            _issuer = config["JwtSettings:Issuer"];
            _audience = config["JwtSettings:Audience"];
            _expiryInMinutes = Convert.ToInt32(config["JwtSettings:ExpirationMinutes"]);
        }

        public string GenerateAccessToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: new[] { new Claim(ClaimTypes.Name, username) },
                expires: DateTime.Now.AddMinutes(_expiryInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            try
            {
                return tokenHandler.ValidateToken(token, validationParameters, out _);
            }
            catch (SecurityTokenException)
            {
                return null;
            }
        }
    }
}
