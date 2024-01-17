using Microsoft.AspNetCore.Mvc;

namespace ChatroomB_Backend.Controllers
{
    public class MessagesController : ControllerBase
    {
        [HttpPost]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
