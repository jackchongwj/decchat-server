using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Controller;
using Newtonsoft.Json;

namespace ChatroomB_Backend.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _MessageService;
        private readonly RabbitMQServices _RabbitMQService;
        private readonly ApplicationServices _ApplicationServices;

        public MessagesController(IMessageService messageService, RabbitMQServices rabbitMQService, ApplicationServices applicationServices)
        {
            _MessageService = messageService;
            _RabbitMQService = rabbitMQService;
            _ApplicationServices = applicationServices;
        }

        [HttpPost("AddMessage")]
        [Authorize]
        public async Task<IActionResult> AddMessage([FromForm] IFormFile? file)
        {
            string? messageJson = Request.Form["message"];

            if (string.IsNullOrEmpty(messageJson))
            {
                // Handle the null or empty scenario, e.g., return an error response
                return BadRequest("The message content is missing.");
            }

            Messages? message = JsonConvert.DeserializeObject<Messages>(messageJson);

            if (message == null)
            {
                // Handle the scenario where deserialization results in a null object
                return BadRequest("The message content could not be parsed.");
            }

            if (file!=null)
            {
                FileMessage fileMessage = new FileMessage
                {
                    Message = message,
                    FileByte = await ConvertToByteArrayAsync(file),
                    FileName = file.FileName,
                    FileType = file.ContentType.Split('/')[0]
                };

                _RabbitMQService.PublishMessage(fileMessage);
            }
            else
            {
                _RabbitMQService.PublishMessage(new FileMessage
                {
                    Message = message
                });
            }

            return Ok();
        }

        [HttpGet("GetMessage")]
        [Authorize]
        public async Task<IActionResult> RetrieveMessage(int ChatRoomId, int MessageId) 
        {
           IEnumerable<ChatRoomMessage> message = await _MessageService.GetMessages(ChatRoomId, MessageId);

            return Ok(message);
        }

        [HttpPost("EditMessage")]
        [Authorize]
        public IActionResult EditMessage([FromBody] EditMessage edittedMessage)
        {
            _RabbitMQService.PublishEditMessage(edittedMessage);

            return Ok();
        }

        [HttpPost("DeleteMessage")]
        [Authorize]
        public async Task<IActionResult> DeleteMessage([FromQuery] int MessageId, [FromQuery] int ChatRoomId)
        {
            int result = await _MessageService.DeleteMessage(MessageId, ChatRoomId);

            return Ok(result);
        }

        [HttpGet("GetTotalSearchMessage")]
        public async Task<int> GetTotalSearchMessage(int ChatRoomId, string searchValue)
        {
            int result = await _MessageService.GetTotalSearchMessage(ChatRoomId, searchValue);

            return result;
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
