﻿using System;
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
using StackExchange.Redis;
using Microsoft.AspNetCore.Authorization;

namespace ChatroomB_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendsController : ControllerBase
    {
        private readonly IFriendService _FriendService;
        private readonly IChatRoomService _ChatRoomService;

        public FriendsController(IFriendService Fservice, IChatRoomService CService)
        {
            _FriendService = Fservice;
            _ChatRoomService = CService;
        }

        //POST: Friends/Create
        [HttpPost("AddFriend")]
        [Authorize]
        public async Task<IActionResult> AddFriend([FromBody] Friends friends)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data");
            }

            try
            {
                await _FriendService.AddFriends(friends);

                return Ok(1);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("UpdateFriendRequest")]
        [Authorize]
        public async Task<ActionResult<int>> UpdateFriendRequest([FromBody]FriendRequest request, [FromQuery]int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data");
            }

            try
            {
                int result = await _FriendService.UpdateFriendRequest(request);

                if (request.Status == 2)
                {
                    IEnumerable<ChatlistVM> PrivateChatlist = await _ChatRoomService.AddChatRoom(request, userId);
                        
                    return Ok(PrivateChatlist);
                }

                return Ok(0);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("DeleteFriend")]
        [Authorize]
        public async Task<ActionResult<int>> DeleteFriend([FromBody] DeleteFriendRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data");
            }

            try
            {
                int result = await _FriendService.DeleteFriendRequest(request.ChatRoomId, request.UserId1, request.UserId2);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
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
