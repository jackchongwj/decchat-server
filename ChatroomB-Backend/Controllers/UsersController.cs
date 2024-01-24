using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChatroomB_Backend.Data;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using System.Linq;

namespace ChatroomB_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _UserService;

        public UsersController(IUserService service)
        {
            _UserService = service;
        }

        [HttpGet("Search")]
        public async Task<IActionResult> SearchByProfileName(string profileName)
        {
            IEnumerable<Users> GetUserByName = await _UserService.GetByName(profileName);

            return Ok(GetUserByName);
        }

        [HttpGet("GetChatListByUserId")]
        public async Task<IActionResult> GetChatListByUserId(int userId)
        {
            var friendList = await _UserService.GetChatListByUserId(userId);
            return Ok(friendList); //HTTP 200 OK indicates that the request was successful, and the server is returning the requested data.
        }
    }
}
