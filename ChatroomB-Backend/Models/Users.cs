using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChatroomB_Backend.Models
{
    [Index(nameof(UserName), IsUnique = true)]
    public class Users
    {
        [Key]
        public int? UserId { get; set; }

        [Required]
        [Column(TypeName = "varchar(15)")]
        public string UserName { get; set; } = null!;

        [Required]
        [Column(TypeName = "nvarchar(15)")]
        public string ProfileName { get; set; } = null!;

        [Required]
        [Column(TypeName = "varchar(256)")]
        public string Password { get; set; } = null!;

        [Column(TypeName = "varchar(256)")]
        public string? ProfilePicture { get; set; }

        public bool IsDelete { get; set; }
    }
}
