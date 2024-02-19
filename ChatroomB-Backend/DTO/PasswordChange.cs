using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatroomB_Backend.DTO
{
    public class PasswordChange
    {

        [Required]
        [Column(TypeName = "varchar(256)")]
        public string CurrentPassword { get; set; } = null!;

        [Required]
        [Column(TypeName = "varchar(256)")]
        public string NewPassword { get; set; } = null!;
    }
}
