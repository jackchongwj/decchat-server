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
using Microsoft.AspNetCore.SignalR;
using ChatroomB_Backend.SignalR;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using ChatroomB_Backend.DTO;

namespace ChatroomB_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _UserService;
        private readonly IHubContext<ChatHub> _hubContext;

        public UsersController(IUserService service, IHubContext<ChatHub> hubContext)
        {
            _UserService = service;
            _hubContext = hubContext;
        }

        [HttpGet("Search")]
        public async Task<IActionResult> SearchByProfileName(string profileName, int userId)
        {
            IEnumerable<UserSearch> GetUserByName = await _UserService.GetByName(profileName, userId);

            //await _hubContext.Clients.All.SendAsync("ReceiveSearchResults", GetUserByName);

            return Ok(GetUserByName);
        }


        [HttpGet("FriendRequest")]
        public async Task<IActionResult> GetFriendRequest(int userId)
        {
            IEnumerable<Users> GetFriendRequest = await _UserService.GetFriendRequest(userId);

            return Ok(GetFriendRequest);
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

        //// DELETE: api/Users/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteUsers(int? id)
        //{
        //    var users = await _context.Users.FindAsync(id);
        //    if (users == null)
        //    {
        //        return NotFound();
        //    }

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
