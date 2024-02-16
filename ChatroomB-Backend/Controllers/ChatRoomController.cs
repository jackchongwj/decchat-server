using ChatroomB_Backend.DTO;
using Microsoft.AspNetCore.Mvc;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using ChatroomB_Backend.DTO;
using System.Text.RegularExpressions;
using Azure.Core;
using Microsoft.AspNetCore.SignalR;
using ChatroomB_Backend.Hubs;
using Microsoft.EntityFrameworkCore;


namespace ChatroomB_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatRoomController: Controller 
    {
        private readonly IChatRoomService _ChatRoomService;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatRoomController(IHubContext<ChatHub> hubContext, IChatRoomService CService)
        {
            _hubContext = hubContext;
            _ChatRoomService = CService;
        }


        [HttpPost("createNewGroup")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupVM createGroupVM)
        {
            try
            {
                await _ChatRoomService.CreateGroupWithSelectedUsers(createGroupVM.RoomName, createGroupVM.InitiatedBy, createGroupVM.SelectedUsers);
                await _hubContext.Clients.All.SendAsync("NewGroupCreated", createGroupVM.RoomName);
                return Ok("Group created successfully");


            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }   
    } 
}