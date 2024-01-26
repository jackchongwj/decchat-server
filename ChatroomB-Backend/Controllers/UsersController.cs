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
        public async Task<IActionResult> SearchByProfileName(string profileName, int userId)
        {
            IEnumerable<Users> GetUserByName = await _UserService.GetByName(profileName, userId);

            return Ok(GetUserByName);
        }

        [HttpGet("GetChatListByUserId")]
        public async Task<IActionResult> GetChatListByUserId(int userId)
        {
            var friendList = await _UserService.GetChatListByUserId(userId);
            return Ok(friendList); //HTTP 200 OK indicates that the request was successful, and the server is returning the requested data.
        }

        [HttpGet("FriendRequest")]
        public async Task<IActionResult> GetFriendRequest(int userId)
        {
            IEnumerable<Users> GetFriendRequest = await _UserService.GetFriendRequest(userId);

            return Ok(GetFriendRequest);
        }

        [HttpGet("UserDetails")]
        public async Task<ActionResult<Users>> GetUserById(int id)
        {
            var user = await _UserService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPut("UserUpdate")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] Users user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            int updatedUser = await _UserService.UpdateUser(user);
            if (updatedUser == 0)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpDelete("UserDeletion")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            int result = await _UserService.DeleteUser(id);
            if (result == 0) { return BadRequest(); }
            else { return Ok(); }

        }

        [HttpPost("UserDetails/PasswordChange")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] string newPassword)
        {
            await _UserService.ChangePassword(id, newPassword);
            return Ok();
        }

        //// GET: api/Users
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
        //{
        //    return await _context.Users.ToListAsync();
        //}

        //// GET: api/Users/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Users>> GetUsers(int? id)
        //{
        //    var users = await _context.Users.FindAsync(id);

        //    if (users == null)
        //    {
        //        return NotFound();
        //    }

        //    return users;
        //}

        //// PUT: api/Users/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutUsers(int? id, Users users)
        //{
        //    if (id != users.UserId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(users).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!UsersExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/Users
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Users>> PostUsers(Users users)
        //{
        //    _context.Users.Add(users);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetUsers", new { id = users.UserId }, users);
        //}

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Users>> PostUsers(Users users)
        //{
        //    _context.Users.Add(users);
        //    await _context.SaveChangesAsync();

        //    _context.Users.Remove(users);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool UsersExists(int? id)
        //{
        //    return _context.Users.Any(e => e.UserId == id);
        //}
    }
}
