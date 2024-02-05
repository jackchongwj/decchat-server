using ChatroomB_Backend.DTO;
using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
        public async Task<IActionResult> AddMessage([FromForm] IFormFile file)
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

            FileMessage fileMessage = new FileMessage
            {
                Message = message,
                FileByte = await ConvertToByteArrayAsync(file),
                FileName = file.FileName
            };

            _RabbitMQService.PublishMessage(fileMessage);

            return Ok(1);
        }

        [HttpPost("ClearMessage")]
        public IActionResult ClearQueueMessage()
        {
            return Ok(0);
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
