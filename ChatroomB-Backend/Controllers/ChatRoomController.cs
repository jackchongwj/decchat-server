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
using Microsoft.AspNetCore.Authorization; 


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
        [Authorize]
        public async Task<IActionResult> UpdateGroupPicture([FromForm] IFormFile file, [FromForm(Name = "id")] string ChatRoomId)
        {
            try
            {
                byte[] filebyte = await ConvertToByteArrayAsync(file);

                if (file == null || file.Length == 0)
                {
                    return BadRequest("File is not provided or empty.");
                }

                int success = await _ChatRoomService.UpdateGroupPicture(Convert.ToInt32(ChatRoomId), filebyte, file.FileName);

                if (success == 0)
                {
                    return NotFound("Failed to update the group picture.");
                }

                return Ok(new { Message = "Profile picture updated successfully." });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet("groupMembers")]
        [Authorize]
        public async Task<IActionResult> GetGroupMembers(int chatRoomId, int userId)
        {
            try
            {
                IEnumerable<GroupMember> groupMembers = await _ChatRoomService.RetrieveGroupMemberByChatroomId(chatRoomId, userId);

                return Ok(groupMembers);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost("createNewGroup")]
        [Authorize]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupVM createGroupVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data");
            }

            try
            {
                IEnumerable<ChatlistVM> chatinfo = await _ChatRoomService.CreateGroupWithSelectedUsers(createGroupVM);
                return Ok(new { Message = "Group created successfully" });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost("AddMembersToGroup")]
        public async Task<ActionResult<int>> AddMembersToGroup([FromBody] AddMemberVM addMemberVM)
        {
            try
            {
                await _ChatRoomService.AddMembersToGroup(addMemberVM);
                
                return Ok(new { Message = "User added successfully" });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost("RemoveFromGroup")]
        
        public async Task<ActionResult<int>> RemoveUserFromGroup([FromQuery] int chatRoomId, [FromQuery] int userId, [FromQuery] int InitiatedBy, [FromQuery] int CurrentUserId)
        {

            try
            {
                if (CurrentUserId  == InitiatedBy)
                {
                    int result = await _ChatRoomService.RemoveUserFromGroup(chatRoomId, userId);

                    return Ok(new { Message = "User removed successfully" });
                }
                else
                {
                    return BadRequest(new { ErrorMessage = "Only admin is allowed to remove user." });
                }
               
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost("QuitGroup")]
        [Authorize]
        public async Task<ActionResult<int>> QuitGroup([FromQuery] int chatRoomId, [FromQuery] int userId)
        {
            try
            {
                int result = await _ChatRoomService.QuitGroup(chatRoomId, userId);

                return Ok(new { Message = "Quit group successfully" });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost("UpdateGroupName")]
        [Authorize]
        public async Task<IActionResult> UpdateGroupName([FromBody] UpdateGroupName model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data");
            }

            try
            {
                int result = await _ChatRoomService.UpdateGroupName(model.chatroomId, model.NewGroupName);

                if (result == 0) return NotFound("Group invalid or not found");

                return Ok();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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