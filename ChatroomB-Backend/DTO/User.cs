using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChatroomB_Backend.DTO
{
    public class UserSearchDetails 
    {
        public int? UserId { get; set; }

        public string UserName { get; set; } = null!;

        public string ProfileName { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? ProfilePicture { get; set; }

        public int Status { get; set; } 

        public bool IsDeleted { get; set; }
    }
}
