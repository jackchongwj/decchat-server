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
            int result = await _FriendService.CheckFriendExist(friends);

            if (result == 0)
            {

                await _FriendService.AddFriends(friends);
                return Ok(new { Message = "Friend Request send successfully" });
            }
            else
            {
                return BadRequest(new { ErrorMessage = "Friend Has Added Before" });
            }

        }


        [HttpPost("UpdateFriendRequest")]
        [Authorize]
        public async Task<ActionResult<int>> UpdateFriendRequest([FromBody] FriendRequest request)
        {
            int result = await _FriendService.UpdateFriendRequest(request);

            if (request.Status == 2)
            {
                IEnumerable<ChatlistVM> PrivateChatlist = await _ChatRoomService.AddChatRoom(request);

                return Ok(PrivateChatlist);
            }
            else
            {

                return Ok(result);
            }
        }


        [HttpPost("DeleteFriend")]
        [Authorize]
        public async Task<ActionResult<int>> DeleteFriend([FromBody] DeleteFriendRequest request)
        {

            int result = await _FriendService.DeleteFriendRequest(request.ChatRoomId, request.UserId1, request.UserId2);

            return Ok(result);

        }

    }
}
