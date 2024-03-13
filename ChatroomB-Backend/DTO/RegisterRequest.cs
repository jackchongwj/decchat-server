using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ChatroomB_Backend.DTO
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(15, ErrorMessage = "Username must not exceed 15 characters.")]
        [RegularExpression(@"^[a-zA-Z\d$@^!%*?&]*$", ErrorMessage = "Username contains invalid characters.")]
        [Remote(action: "DoesUsernameExist", controller: "User", HttpMethod = "GET", ErrorMessage = "Username is already taken.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [PasswordStrength(ErrorMessage = "Password does not meet strength requirements.")]
        public string Password { get; set; }

        [ProfileName(ErrorMessage = "Invalid profile name.")]
        public string? ProfileName { get; set; }
    }
}
