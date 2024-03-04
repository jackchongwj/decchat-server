using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatroomB_Backend.DTO
{
    public class PasswordChange
    {

        [Required(ErrorMessage = "Current password is required")]
        [Column(TypeName = "varchar(256)")]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "New password is required")]
        [PasswordStrength(ErrorMessage = "Password does not meet strength requirements.")]
        [Column(TypeName = "varchar(256)")]
        public string NewPassword { get; set; } = null!;
    }
}
