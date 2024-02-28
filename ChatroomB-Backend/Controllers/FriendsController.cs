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
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFriend([FromBody] Friends friends)
        {

            if (ModelState.IsValid)
            {
                int result = await _FriendService.CheckFriendExit(friends);

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
            else
            {

               return BadRequest(new { ErrorMessage = "Invalid model. Please check the provided data." });
            }

        }

        [HttpPost("UpdateFriendRequest")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult<int>> UpdateFriendRequest([FromBody] FriendRequest request, [FromQuery] int userId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int result = await _FriendService.UpdateFriendRequest(request);

                    if (request.Status == 2)
                    {
                        IEnumerable<ChatlistVM> PrivateChatlist = await _ChatRoomService.AddChatRoom(request, userId);


                        return Ok(PrivateChatlist);
                    }

                    return Ok(0);
                }
                else
                {
                    return BadRequest(new { Message = "Invalid model. Please check the provided data." });
                }

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in UpdateFriendRequest method: {ex.ToString()}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpPost("DeleteFriend")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult<int>> DeleteFriend([FromBody] DeleteFriendRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int result = await _FriendService.DeleteFriendRequest(request.ChatRoomId, request.UserId1, request.UserId2);


                    return Ok(result);
                }
                else
                {
                    return BadRequest(new { Message = "Invalid model. Please check the provided data." });
                }

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in DeleteFriend method: {ex.ToString()}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

    }
}
