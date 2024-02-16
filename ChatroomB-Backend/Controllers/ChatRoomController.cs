using ChatroomB_Backend.DTO;
using Microsoft.AspNetCore.Mvc;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using ChatroomB_Backend.DTO;
using System.Text.RegularExpressions;
using Azure.Core;


namespace ChatroomB_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatRoomController: Controller 
    {
        private readonly IChatRoomService _ChatRoomService;


        public ChatRoomController(IChatRoomService CService)
        {
            _ChatRoomService = CService;
        }


        [HttpPost("createNewGroup")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupVM createGroupVM)
        {
            try
            {
                _ChatRoomService.CreateGroupWithSelectedUsers(createGroupVM.RoomName, createGroupVM.InitiatedBy, createGroupVM.SelectedUsers);
                return Ok("Group created successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }   
    } 
}