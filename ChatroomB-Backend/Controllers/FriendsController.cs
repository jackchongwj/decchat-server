using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChatroomB_Backend.Data;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using ChatroomB_Backend.DTO;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Cors;

namespace ChatroomB_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendsController : Controller
    {
        private readonly IFriendService _FriendService ;
        private readonly IChatRoomService _ChatRoomService;
        private readonly IRedisServcie _RedisServcie;


        public FriendsController(IFriendService Fservice, IChatRoomService CService, IRedisServcie redisServcie)
        {
            _FriendService = Fservice;
            _ChatRoomService = CService;
            _RedisServcie = redisServcie;
        }

        //POST: Friends/Create
        [HttpPost("AddFriend")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] Friends friends)
        {
            if (ModelState.IsValid)
            {
                int result  = await _FriendService.AddFriends(friends);
                
            }

            return Ok(friends);
        }

        [HttpPost("UpdateFriendRequest")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult<int>> UpdateFriendRequest(FriendRequest request)
        {
            if (ModelState.IsValid)
            {
               int result =  await _FriendService.UpdateFriendRequest(request);

                if (request.Status == 2)
                {
                    int chatroomId = await _ChatRoomService.AddChatRoom(request);
                    return Ok(chatroomId);
                }
            }
            return Ok(0);
        }


       
        //[HttpGet]
        //public IActionResult Get()
        //{
        //    // 模拟抛出一个异常
        //    throw new ApplicationException("This is a simulated exception.");
        //}

        //[HttpGet("404")]
        //public IActionResult Geterror()
        //{
        //    // 此处不会抛出异常，但返回 404 Not Found
        //    return NotFound("Resource not found.");
        //}
    }
}
