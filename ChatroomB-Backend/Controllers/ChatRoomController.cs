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


        [HttpPost("UpdateGroupPicture")]
        public async Task<IActionResult> UpdateGroupPicture([FromForm] IFormFile file, [FromForm(Name = "id")] string ChatRoomId)
        {
            byte[] filebyte = await ConvertToByteArrayAsync(file);
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is not provided or empty.");
            }

            var success = await _ChatRoomService.UpdateGroupPicture(Convert.ToInt32(ChatRoomId), filebyte, file.FileName);

            if (success == 0)
            {
                return NotFound("Failed to update the group picture.");
            }

            return Ok(new { Message = "Profile picture updated successfully." });
        }

        private async Task<byte[]> ConvertToByteArrayAsync(IFormFile file)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        [HttpGet("groupMembers")]
        public async Task<IActionResult> GetGroupMembers(int chatRoomId, int userId)
        {
            try
            {
                var groupMembers = await _ChatRoomService.RetrieveGroupMemberByChatroomId(chatRoomId, userId);
                return Ok(groupMembers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("createNewGroup")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupVM createGroupVM)
        {
            try
            {
                ChatlistVM chatinfo = await _ChatRoomService.CreateGroupWithSelectedUsers(createGroupVM);
                await _hubContext.Clients.All.SendAsync("NewGroupCreated", chatinfo);
                return Ok(new { Message = "Group created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("RemoveFromGroup")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult<int>> RemoveUserFromGroup([FromQuery] int chatRoomId, [FromQuery] int userId, [FromQuery] int InitiatedBy)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int result = await _ChatRoomService.RemoveUserFromGroup(chatRoomId, userId);
                   

                    return Ok(new { Message = "User removed successfully" });
                }
                else
                {
                    return BadRequest(new { Message = "Invalid model. Please check the provided data." });
                }

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in RemoveUserFromGroup method: {ex.ToString()}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("QuitGroup")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult<int>> QuitGroup([FromQuery] int chatRoomId, [FromQuery] int userId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int result = await _ChatRoomService.QuitGroup(chatRoomId, userId);

                    return Ok(new { Message = "Quit group successfully" });
                }
                else
                {
                    return BadRequest(new { Message = "Invalid model. Please check the provided data." });
                }

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in QuitGroup method: {ex.ToString()}");
                return StatusCode(500, "An error occurred while processing your request.");
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


    }
}