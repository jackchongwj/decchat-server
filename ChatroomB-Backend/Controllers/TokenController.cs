﻿using ChatroomB_Backend.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChatroomB_Backend.Models;
using System.IdentityModel.Tokens.Jwt;
using ChatroomB_Backend.Utils;
using Microsoft.IdentityModel.Tokens;

namespace ChatroomB_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly ITokenUtils _tokenUtil;

        public TokenController(ITokenService tokenService, ITokenUtils tokenUtil)
        {
            _tokenService = tokenService;
            _tokenUtil = tokenUtil;
        }

        [HttpPost("RenewToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RenewToken()
        {
            // Get Refresh Token from custom header
            var refreshToken = Request.Headers["X-Refresh-Token"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ArgumentException("Refresh token is required");
            }

            // Retrieve userId and username from HttpContext, attached by the token middleware
            if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) ||
                !HttpContext.Items.TryGetValue("Username", out var usernameObj))
            {
                throw new UnauthorizedAccessException("User information is missing in the request context");
            }

            int userId = (int)userIdObj!;
            string username = (string)usernameObj!;

            // Validate the refresh token
            await _tokenService.ValidateRefreshToken(refreshToken, userId);

            // Update refresh token expiry
            await _tokenService.UpdateRefreshToken(refreshToken);

            // Generate and pass new access token
            string newAccessToken = _tokenUtil.GenerateAccessToken(userId, username);

            return Ok(new { AccessToken = newAccessToken });
        }
    }
}
