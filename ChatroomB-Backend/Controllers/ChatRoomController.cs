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

        [HttpPost("UpdateGroupPicture")]
        public async Task<IActionResult> UpdateGroupPicture([FromForm] IFormFile file, [FromForm(Name = "id")] string ChatRoomId)
        {
            byte[] filebyte = await ConvertToByteArrayAsync(file);
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is not provided or empty.");
            }

            var success = await _ChatRoomService.UpdateGroupPicture(Convert.ToInt32(ChatRoomId), filebyte, file.FileName);

            if (!success)
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
    } 
}