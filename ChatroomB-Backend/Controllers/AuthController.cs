using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using Microsoft.EntityFrameworkCore;

namespace ChatroomB_Backend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly IAuthService _authService;
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;

        public AuthController
            (
            IConfiguration configuration,
            ITokenService tokenService,
            IAuthService authService,
            UserManager<Users> userManager,
            SignInManager<Users> signInManager
            )
        {
            _configuration = configuration;
            _tokenService = tokenService;
            _authService = authService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        //[HttpPost("IsUsernameUnique")]
        //public IActionResult IsUsernameUnique([FromBody] string username)
        //{
        //   bool isUnique = !dbContext.Users.Any(u => u.Username == username);

        //    return Json(isUnique);
        //}

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] string username, string password)
        {
            // Validate the model

            var user = new Users { UserName = username };

            _authService.SetPassword(user, password);

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                // Generate and return the JWT token
                var token = _tokenService.GenerateAccessToken(username);
                return Ok(new { Token = token });
            }

            return BadRequest(new { Errors = result.Errors });
        }

        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        //{
        //    // Validate the model

        //    var user = await _userManager.FindByNameAsync(model.Username);
        //    if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        //    {
        //        // Generate and return the JWT token
        //        var token = _tokenService.GenerateToken(user);
        //        return Ok(new { Token = token });
        //    }

        //    return Unauthorized(new { Error = "Invalid username or password" });
        //}

        // Other actions, like logout, refresh token, etc.

        [HttpGet("test")]
        [Authorize]
        public IActionResult Test()
        {
            return Ok("Authorized access!");
        }
    }

}