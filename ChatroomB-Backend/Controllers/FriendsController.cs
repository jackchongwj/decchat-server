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
    public class FriendsController : Controller
    {
        private readonly IFriendService _FriendService ;
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
            try
            {
                if (ModelState.IsValid)
                {
                    await _FriendService.AddFriends(friends);
                    return Ok(1);
                }
                else
                {
                    return BadRequest(new { Message = "Invalid model. Please check the provided data." });
                }

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in AddFriend method: {ex.ToString()}");

                //return error message to client
                return StatusCode(500, "An error occurred while processing your request.");

                //return StatusCode(500, new { ErrorMessage = ex.Message, StackTrace = ex.StackTrace });
            }
        }

        [HttpPost("UpdateFriendRequest")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult<int>> UpdateFriendRequest([FromBody]FriendRequest request, [FromQuery]int userId)
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

            }catch(Exception ex) 
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
