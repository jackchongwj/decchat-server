using System.ComponentModel.DataAnnotations;

namespace ChatroomB_Backend.DTO
{
    public class AuthRequest
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
