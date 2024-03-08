﻿using ChatroomB_Backend.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace ChatroomB_Backend.Utils
{
    public interface IAuthUtils
    {
        string GenerateSalt();
        string HashPassword(string password, string salt);
        ActionResult<int> ExtractUserIdFromJWT(ClaimsPrincipal user);
    }
    
    public interface ITokenUtils
    {
        string GenerateAccessToken(int userId, string username);
        RefreshToken GenerateRefreshToken(int userId);
        CookieOptions SetCookieOptions();
    }
}
