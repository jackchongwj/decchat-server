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
            int userId = _authUtils.ExtractUserIdFromJWT(HttpContext.User);

            friends.SenderId = userId;

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
                int userId = _authUtils.ExtractUserIdFromJWT(HttpContext.User);

                request.ReceiverId = userId;

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
            int userId = _authUtils.ExtractUserIdFromJWT(HttpContext.User);

            int result = await _FriendService.DeleteFriendRequest(request.ChatRoomId, request.UserId1 = userId, request.UserId2);

            return Ok(result);
        }
    }
}
