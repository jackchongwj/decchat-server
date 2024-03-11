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
using StackExchange.Redis;
using Microsoft.AspNetCore.Authorization;
using ChatroomB_Backend.Utils;
using Azure.Core;

namespace ChatroomB_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendsController : ControllerBase
    {
        private readonly IFriendService _FriendService;
        private readonly IChatRoomService _ChatRoomService;
        private readonly IAuthUtils _authUtils;

        public FriendsController(IFriendService Fservice, IChatRoomService CService, IAuthUtils authUtils)
        {
            _FriendService = Fservice;
            _ChatRoomService = CService;
            _authUtils = authUtils;
        }

        //POST: Friends/Create
        [HttpPost("AddFriend")]
        [Authorize]
        public async Task<IActionResult> AddFriend([FromBody] Friends friends)
        {
            ActionResult<int> userIdResult = _authUtils.ExtractUserIdFromJWT(HttpContext.User);
            if (userIdResult.Result is not null)
            {
                return userIdResult.Result;
            }

            friends.SenderId = userIdResult.Value;

            int result = await _FriendService.CheckFriendExist(friends);

            if (result == 0)
            {

                await _FriendService.AddFriends(friends);
                return Ok(new { Message = "Friend Request send successfully" });
            }
                return BadRequest(new { ErrorMessage = "Friend Has Added Before" });

        }


        [HttpPost("UpdateFriendRequest")]
        [Authorize]
        public async Task<ActionResult<int>> UpdateFriendRequest([FromBody]FriendRequest request)
        {
            try
            {
                ActionResult<int> userIdResult = _authUtils.ExtractUserIdFromJWT(HttpContext.User);
                if (userIdResult.Result is not null)
                {
                    // If there is an ActionResult, it means there was an error, return it
                    return userIdResult.Result;
                }

                request.ReceiverId = userIdResult.Value;

                int result = await _FriendService.UpdateFriendRequest(request);

                if (request.Status == 2)
                {
                    IEnumerable<ChatlistVM> PrivateChatlist = await _ChatRoomService.AddChatRoom(request);

                    return Ok(PrivateChatlist);
                }

                return Ok(0);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }


        [HttpPost("DeleteFriend")]
        [Authorize]
        public async Task<ActionResult<int>> DeleteFriend([FromBody] DeleteFriendRequest request)
        {
            ActionResult<int> userIdResult = _authUtils.ExtractUserIdFromJWT(HttpContext.User);
            if (userIdResult.Result is not null)
            {
                return userIdResult.Result;
            }


            int result = await _FriendService.DeleteFriendRequest(request.ChatRoomId, request.UserId1 = userIdResult.Value, request.UserId2);

            return Ok(result);

        }

    }
}
