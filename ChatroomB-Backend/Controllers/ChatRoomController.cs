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
    public class ChatRoomController: ControllerBase
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
                ChatlistVM chatinfo = await _ChatRoomService.CreateGroupWithSelectedUsers(createGroupVM);
                await _hubContext.Clients.All.SendAsync("NewGroupCreated", chatinfo);
                /*await _hubContext.Clients.Group(createGroupVM.RoomName).SendAsync("NewGroupCreated", createGroupVM.RoomName);*/
                return Ok("Group created successfully");


            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("UpdateGroupName")]
        public async Task<IActionResult> UpdateGroupName([FromBody] UpdateGroupName model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _ChatRoomService.UpdateGroupName(model.chatroomId, model.NewGroupName);
            if (result == 0) return NotFound();

            return Ok();
        }

        [HttpPost("UpdateGroupPicture")]
        public async Task<IActionResult> UpdateGroupPicture([FromForm] IFormFile file, [FromForm(Name = "id")] string chatroomId)
        {
            byte[] filebyte = await ConvertToByteArrayAsync(file);
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is not provided or empty.");
            }

            var success = await _ChatRoomService.UpdateGroupPicture(Convert.ToInt32(chatroomId), filebyte, file.FileName);

            if ( success == 0 )
            {
                return NotFound("Failed to update the group picture.");
            }

            return Ok(new { Message = "Group picture updated successfully." });
        }

        private async Task<byte[]> ConvertToByteArrayAsync(IFormFile file)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
    } 
}