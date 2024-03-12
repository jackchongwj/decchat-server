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
using ChatroomB_Backend.Utils;


namespace ChatroomB_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatRoomController : ControllerBase
    {
        private readonly IChatRoomService _ChatRoomService;
        private readonly IAuthUtils _authUtils;

        public ChatRoomController(IChatRoomService CService, IAuthUtils authUtils)
        {
            _ChatRoomService = CService;
            _authUtils = authUtils;
        }

        [HttpPost("UpdateGroupPicture")]
        [Authorize]
        public async Task<IActionResult> UpdateGroupPicture([FromForm] IFormFile file, [FromForm(Name = "id")] string ChatRoomId)
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

        [HttpGet("groupMembers")]
        [Authorize]
        public async Task<IActionResult> GetGroupMembers(int chatRoomId)
        {
            try
            {
                int userIdResult = _authUtils.ExtractUserIdFromJWT(HttpContext.User);

                IEnumerable<GroupMember> groupMembers = await _ChatRoomService.RetrieveGroupMemberByChatroomId(chatRoomId, userIdResult);

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
        [Authorize]
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
        [Authorize]
        public async Task<ActionResult<int>> RemoveUserFromGroup([FromQuery] int chatRoomId, [FromQuery] int removedUserId, [FromQuery] int InitiatedBy)
        {
            try
            {
                int userIdResult = _authUtils.ExtractUserIdFromJWT(HttpContext.User);

                if (userIdResult == InitiatedBy)
                {
                    int result = await _ChatRoomService.RemoveUserFromGroup(chatRoomId, removedUserId);

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
        public async Task<ActionResult<int>> QuitGroup([FromQuery] int chatRoomId)
        {
            try
            {
                int userIdResult = _authUtils.ExtractUserIdFromJWT(HttpContext.User);

                int result = await _ChatRoomService.QuitGroup(chatRoomId, userIdResult);

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

            int result = await _ChatRoomService.UpdateGroupName(model.chatroomId, model.NewGroupName);

            if (result == 0) return NotFound("Group invalid or not found");

            return Ok();
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