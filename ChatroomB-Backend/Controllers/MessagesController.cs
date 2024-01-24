using ChatroomB_Backend.Models;
using ChatroomB_Backend.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        public IActionResult AddMessage(Messages message)
        {
            _RabbitMQService.PublishMessage(message);
            //int result = await _MessageService.AddMessages(message);

            return Ok(1);
        }

        [HttpPost("ClearMessage")]
        public IActionResult ClearQueueMessage()
        {
            return Ok(0);
        }
    }
}
