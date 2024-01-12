using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ChatroomB_Backend.Models
{
    [Index(nameof(userName), IsUnique = true)]
    public class Users
    {
        [Key]
        public int? UserId { get; set; }

        [Required]
        public string userName { get; set; } = null!;

        [Required]
        public string profileName { get; set; } = null!;

        [Required]
        public string password { get; set; } = null!;

        [Required]
        public string profilePicture { get; set; } = null!;

        public bool isDelete { get; set; }
    }
}
